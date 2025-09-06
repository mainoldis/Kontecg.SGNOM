using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kontecg.Linq;

namespace Kontecg.Desktop.Linq
{
    /// <summary>
    /// <see cref="FakeAsyncQueryableExecuter"/> <see langword="await"/>s an asynchronous <see cref="Task"/> before executing an operation synchronously.
    /// This differs from <see cref="NullAsyncQueryableExecuter"/> a.k.a. SyncQueryableExecuter, which actually only executes an operation synchronously.
    /// This can be used with tests to catch code that does not properly <see langword="await"/> a <see cref="Task"/> in a <see langword="using"/> block.
    /// </summary>
    public class FakeAsyncQueryableExecuter : IAsyncQueryableExecuter
    {
        private Task AsyncTask()
        {
            return Task.Delay(1); // Task.Delay(0) and Task.CompletedTask are synchronous
        }

        public async Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = new CancellationToken())
        {
            await AsyncTask();
            return queryable.Count();
        }

        public async Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = new CancellationToken())
        {
            await AsyncTask();
            return queryable.ToList();
        }

        public async Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = new CancellationToken())
        {
            await AsyncTask();
            return queryable.FirstOrDefault();
        }

        public async Task<bool> AnyAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = new CancellationToken())
        {
            await AsyncTask();
            return queryable.Any();
        }
    }
}
