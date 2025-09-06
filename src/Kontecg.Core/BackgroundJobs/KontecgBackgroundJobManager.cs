using System;
using System.Reflection;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus;
using Kontecg.Events.Bus.Exceptions;
using Kontecg.Json;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;
using Kontecg.Timing;
using Newtonsoft.Json;

namespace Kontecg.BackgroundJobs
{
    public class KontecgBackgroundJobManager : AsyncPeriodicBackgroundWorkerBase, IBackgroundJobManager,
        ISingletonDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IBackgroundJobStore _store;

        static KontecgBackgroundJobManager()
        {
            JobPollPeriod = 5000;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackgroundJobManager" /> class.
        /// </summary>
        public KontecgBackgroundJobManager(
            IIocResolver iocResolver,
            IBackgroundJobStore store,
            KontecgAsyncTimer timer)
            : base(timer)
        {
            _store = store;
            _iocResolver = iocResolver;

            EventBus = NullEventBus.Instance;

            Timer.Period = JobPollPeriod;
        }

        public IEventBus EventBus { get; set; }

        /// <summary>
        ///     Interval between polling jobs from <see cref="IBackgroundJobStore" />.
        ///     Default value: 5000 (5 seconds).
        /// </summary>
        public static int JobPollPeriod { get; set; }

        public override void Start()
        {
            base.Start();
        }

        [UnitOfWork]
        public virtual async Task<string> EnqueueAsync<TJob, TArgs>(TArgs args,
            BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJobBase<TArgs>
        {
            var jobInfo = new BackgroundJobInfo
            {
                JobType = typeof(TJob).AssemblyQualifiedName,
                JobArgs = args.ToJsonString(),
                Priority = priority
            };

            if (delay.HasValue) jobInfo.NextTryTime = Clock.Now.Add(delay.Value);

            await _store.InsertAsync(jobInfo);
            await CurrentUnitOfWork.SaveChangesAsync();

            return jobInfo.Id.ToString();
        }

        [UnitOfWork]
        public virtual string Enqueue<TJob, TArgs>(TArgs args,
            BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJobBase<TArgs>
        {
            var jobInfo = new BackgroundJobInfo
            {
                JobType = typeof(TJob).AssemblyQualifiedName,
                JobArgs = args.ToJsonString(),
                Priority = priority
            };

            if (delay.HasValue) jobInfo.NextTryTime = Clock.Now.Add(delay.Value);

            _store.Insert(jobInfo);
            CurrentUnitOfWork.SaveChanges();

            return jobInfo.Id.ToString();
        }

        public async Task<bool> DeleteAsync(string jobId)
        {
            if (long.TryParse(jobId, out var finalJobId) == false)
                throw new ArgumentException($"The jobId '{jobId}' should be a number.", nameof(jobId));

            var jobInfo = await _store.GetAsync(finalJobId);

            await _store.DeleteAsync(jobInfo);
            return true;
        }

        public bool Delete(string jobId)
        {
            if (long.TryParse(jobId, out var finalJobId) == false)
                throw new ArgumentException($"The jobId '{jobId}' should be a number.", nameof(jobId));

            var jobInfo = _store.Get(finalJobId);

            _store.Delete(jobInfo);
            return true;
        }

        protected override async Task DoWorkAsync()
        {
            var waitingJobs = await _store.GetWaitingJobsAsync(1000);

            foreach (var job in waitingJobs) await TryProcessJobAsync(job);
        }

        private async Task TryProcessJobAsync(BackgroundJobInfo jobInfo)
        {
            try
            {
                jobInfo.TryCount++;
                jobInfo.LastTryTime = Clock.Now;

                var jobType = Type.GetType(jobInfo.JobType);
                using var job = _iocResolver.ResolveAsDisposable(jobType);
                try
                {
                    var jobExecuteMethod = job.Object.GetType().GetTypeInfo()
                                               .GetMethod(nameof(IBackgroundJob<object>.Execute)) ??
                                           job.Object.GetType().GetTypeInfo()
                                               .GetMethod(nameof(IAsyncBackgroundJob<object>.ExecuteAsync));

                    if (jobExecuteMethod == null)
                        throw new KontecgException(
                            $"Given job type does not implement {typeof(IBackgroundJob<>).Name} or {typeof(IAsyncBackgroundJob<>).Name}. " +
                            "The job type was: " + job.Object.GetType());

                    var argsType = jobExecuteMethod.GetParameters()[0].ParameterType;
                    var argsObj = JsonConvert.DeserializeObject(jobInfo.JobArgs, argsType);

                    if (jobExecuteMethod.Name == nameof(IAsyncBackgroundJob<object>.ExecuteAsync))
                    {
                        var result = (Task) jobExecuteMethod.Invoke(job.Object, new[] {argsObj});
                        await result;
                    }
                    else
                    {
                        jobExecuteMethod.Invoke(job.Object, new[] {argsObj});
                    }

                    await _store.DeleteAsync(jobInfo);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);

                    var nextTryTime = jobInfo.CalculateNextTryTime();
                    if (nextTryTime.HasValue)
                        jobInfo.NextTryTime = nextTryTime.Value;
                    else
                        jobInfo.IsAbandoned = true;

                    await TryUpdateAsync(jobInfo);

                    await EventBus.TriggerAsync(
                        this,
                        new KontecgHandledExceptionData(
                            new BackgroundJobException(
                                "A background job execution is failed. See inner exception for details. See BackgroundJob property to get information on the background job.",
                                ex
                            )
                            {
                                BackgroundJob = jobInfo,
                                JobObject = job.Object
                            }
                        )
                    );
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);

                jobInfo.IsAbandoned = true;

                await TryUpdateAsync(jobInfo);
            }
        }

        private async Task TryUpdateAsync(BackgroundJobInfo jobInfo)
        {
            try
            {
                await _store.UpdateAsync(jobInfo);
            }
            catch (Exception updateEx)
            {
                Logger.Warn(updateEx.ToString(), updateEx);
            }
        }
    }
}
