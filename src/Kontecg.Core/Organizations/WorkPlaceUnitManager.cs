using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using Kontecg.UI;

namespace Kontecg.Organizations
{
    public class WorkPlaceUnitManager : DomainService, IDomainService
    {
        public WorkPlaceUnitManager(
            IRepository<WorkPlaceUnit, long> workPlaceUnitRepository)
        {
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
            WorkPlaceUnitRepository = workPlaceUnitRepository;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual async Task<WorkPlaceUnit> GetByCodeAsync(string code)
        {
            var wp = (await WorkPlaceUnitRepository.GetAllIncludingAsync(c => c.WorkPlacePayment, c => c.Classification,
                c => c.Parent)).SingleOrDefault(ou => ou.Code == code);
            return wp;
        }

        public virtual WorkPlaceUnit GetByCode(string code)
        {
            var wp = WorkPlaceUnitRepository.GetAllIncluding(c => c.WorkPlacePayment, c => c.Classification, c => c.Parent).SingleOrDefault(ou => ou.Code == code);
            return wp;
        }

        protected async Task ValidateWorkPlaceUnitAsync(WorkPlaceUnit workPlaceUnit)
        {
            List<WorkPlaceUnit> siblings = (await FindChildrenAsync(workPlaceUnit.ParentId))
                                           .Where(ou => ou.Id != workPlaceUnit.Id)
                                           .ToList();

            if (siblings.Any(ou => ou.DisplayName == workPlaceUnit.DisplayName))
            {
                throw new UserFriendlyException(L("OrganizationUnitDuplicateDisplayNameWarning",
                    workPlaceUnit.DisplayName));
            }

            if (workPlaceUnit is { ClassificationId: <= 0 })
                throw new UserFriendlyException(L("OrganizationUnitClassificationMissingWarning",
                    workPlaceUnit.DisplayName));
        }

        protected void ValidateWorkPlaceUnit(WorkPlaceUnit workPlaceUnit)
        {
            List<WorkPlaceUnit> siblings = FindChildren(workPlaceUnit.ParentId)
                                           .Where(ou => ou.Id != workPlaceUnit.Id)
                                           .ToList();

            if (siblings.Any(ou => ou.DisplayName == workPlaceUnit.DisplayName))
            {
                throw new UserFriendlyException(L("OrganizationUnitDuplicateDisplayNameWarning",
                    workPlaceUnit.DisplayName));
            }

            if (workPlaceUnit is { ClassificationId: <= 0 })
                throw new UserFriendlyException(L("OrganizationUnitClassificationMissingWarning",
                    workPlaceUnit.DisplayName));
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        protected IRepository<WorkPlaceUnit, long> WorkPlaceUnitRepository { get; }

        public virtual async Task<long> CreateAsync(WorkPlaceUnit workPlaceUnit)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            workPlaceUnit.Code = await GetNextChildCodeAsync(workPlaceUnit.ParentId);
            workPlaceUnit.Order = await CalculateOrderAsync(workPlaceUnit.ParentId);
            await ValidateWorkPlaceUnitAsync(workPlaceUnit);
            long ouId = await WorkPlaceUnitRepository.InsertAndGetIdAsync(workPlaceUnit);

            await uow.CompleteAsync();
            return await Task.FromResult(ouId);
        }

        public virtual long Create(WorkPlaceUnit workPlaceUnit)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            workPlaceUnit.Code = GetNextChildCode(workPlaceUnit.ParentId);
            workPlaceUnit.Order = CalculateOrder(workPlaceUnit.ParentId);
            ValidateWorkPlaceUnit(workPlaceUnit);
            long ouId = WorkPlaceUnitRepository.InsertAndGetId(workPlaceUnit);

            uow.Complete();
            return ouId;
        }

        public virtual async Task UpdateAsync(WorkPlaceUnit workPlaceUnit)
        {
            await ValidateWorkPlaceUnitAsync(workPlaceUnit);
            await WorkPlaceUnitRepository.UpdateAsync(workPlaceUnit);
        }

        public virtual void Update(WorkPlaceUnit workPlaceUnit)
        {
            ValidateWorkPlaceUnit(workPlaceUnit);
            WorkPlaceUnitRepository.Update(workPlaceUnit);
        }

        public virtual async Task<string> GetNextChildCodeAsync(long? parentId)
        {
            WorkPlaceUnit lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                string parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return OrganizationUnit.AppendCode(parentCode, OrganizationUnit.CreateCode(1));
            }

            return OrganizationUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual string GetNextChildCode(long? parentId)
        {
            WorkPlaceUnit lastChild = GetLastChildOrNull(parentId);
            if (lastChild == null)
            {
                string parentCode = parentId != null ? GetCode(parentId.Value) : null;
                return OrganizationUnit.AppendCode(parentCode, OrganizationUnit.CreateCode(1));
            }

            return OrganizationUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual async Task<WorkPlaceUnit> GetLastChildOrNullAsync(long? parentId)
        {
            IOrderedQueryable<WorkPlaceUnit> query = WorkPlaceUnitRepository.GetAll()
                                                                               .Where(ou => ou.ParentId == parentId)
                                                                               .OrderByDescending(ou => ou.Order).ThenByDescending(ou => ou.Code);
            return await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        public virtual WorkPlaceUnit GetLastChildOrNull(long? parentId)
        {
            IOrderedQueryable<WorkPlaceUnit> query = WorkPlaceUnitRepository.GetAll()
                                                                            .Where(ou => ou.ParentId == parentId)
                                                                            .OrderByDescending(ou => ou.Order).ThenByDescending(ou => ou.Code);
            return query.FirstOrDefault();
        }

        protected virtual async Task<int> CalculateOrderAsync(long? parentId)
        {
            IQueryable<WorkPlaceUnit> query = WorkPlaceUnitRepository.GetAll()
                                                                        .Where(ou => ou.ParentId == parentId);
            return await AsyncQueryableExecuter.CountAsync(query);
        }

        protected virtual int CalculateOrder(long? parentId)
        {
            IQueryable<WorkPlaceUnit> query = WorkPlaceUnitRepository.GetAll()
                                                                     .Where(ou => ou.ParentId == parentId);
            return query.Count();
        }

        public virtual async Task<string> GetCodeAsync(long id)
        {
            return (await WorkPlaceUnitRepository.GetAsync(id)).Code;
        }

        public virtual string GetCode(long id)
        {
            return WorkPlaceUnitRepository.Get(id).Code;
        }

        public virtual async Task DeleteAsync(long id)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            List<WorkPlaceUnit> children = await FindChildrenAsync(id, true);

            foreach (WorkPlaceUnit child in children)
            {
                await WorkPlaceUnitRepository.DeleteAsync(child);
            }

            await WorkPlaceUnitRepository.DeleteAsync(id);

            await uow.CompleteAsync();
        }

        public virtual void Delete(long id)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            List<WorkPlaceUnit> children = FindChildren(id, true);

            foreach (WorkPlaceUnit child in children)
            {
                WorkPlaceUnitRepository.Delete(child);
            }

            WorkPlaceUnitRepository.Delete(id);

            uow.Complete();
        }

        public virtual async Task MoveAsync(long id, long? parentId)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            WorkPlaceUnit workPlaceUnit = await WorkPlaceUnitRepository.GetAsync(id);
            if (workPlaceUnit.ParentId == parentId)
            {
                await uow.CompleteAsync();
                return;
            }

            //Should find children before Code change
            List<WorkPlaceUnit> children = await FindChildrenAsync(id, true);

            //Store old code of OU
            string oldCode = workPlaceUnit.Code;

            //Move OU
            workPlaceUnit.Code = await GetNextChildCodeAsync(parentId);
            workPlaceUnit.Order = await CalculateOrderAsync(parentId);
            workPlaceUnit.ParentId = parentId;

            await ValidateWorkPlaceUnitAsync(workPlaceUnit);

            //Update Children Codes
            foreach (WorkPlaceUnit child in children)
            {
                child.Code = OrganizationUnit.AppendCode(workPlaceUnit.Code,
                    OrganizationUnit.GetRelativeCode(child.Code, oldCode));
            }

            await uow.CompleteAsync();
        }

        public virtual void Move(long id, long? parentId)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                WorkPlaceUnit workPlaceUnit = WorkPlaceUnitRepository.Get(id);
                if (workPlaceUnit.ParentId == parentId)
                {
                    return;
                }

                //Should find children before Code change
                List<WorkPlaceUnit> children = FindChildren(id, true);

                //Store old code of OU
                string oldCode = workPlaceUnit.Code;

                //Move OU
                workPlaceUnit.Code = GetNextChildCode(parentId);
                workPlaceUnit.Order = CalculateOrder(parentId);
                workPlaceUnit.ParentId = parentId;

                ValidateWorkPlaceUnit(workPlaceUnit);

                //Update Children Codes
                foreach (WorkPlaceUnit child in children)
                {
                    child.Code = OrganizationUnit.AppendCode(workPlaceUnit.Code,
                        OrganizationUnit.GetRelativeCode(child.Code, oldCode));
                }
            });
        }

        public virtual void UpdateMaxMembersApproved(long? parentId = null)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                List<WorkPlaceUnit> allNodes = FindChildren(parentId, true);
                UpdateWorkPlaceUnitsMaxMembersApproved(allNodes);
            });
        }

        public virtual async Task UpdateMaxMembersApprovedAsync(long? parentId = null)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                List<WorkPlaceUnit> allNodes = await FindChildrenAsync(parentId, true);
                UpdateWorkPlaceUnitsMaxMembersApproved(allNodes);
            });
        }

        public virtual void Reorder(long? parentId = null)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                List<WorkPlaceUnit> allNodes = FindChildren(parentId, true);
                ReassignOrderBfs(allNodes);
            });
        }

        public virtual async Task ReorderAsync(long? parentId = null)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                List<WorkPlaceUnit> allNodes = await FindChildrenAsync(parentId, true);
                ReassignOrderBfs(allNodes);
            });
        }

        private void UpdateWorkPlaceUnitsMaxMembersApproved(List<WorkPlaceUnit> allWorkPlaces)
        {
            if (allWorkPlaces == null || allWorkPlaces.Count == 0)
                return;

            // Diccionario para acceso rápido por ID
            var idToNode = allWorkPlaces.ToDictionary(w => w.Id, w => w);

            // Agrupar hijos por ParentId
            var childrenByParentId = allWorkPlaces
                                     .Where(w => w.ParentId.HasValue)
                                     .GroupBy(w => w.ParentId.Value)
                                     .ToDictionary(g => g.Key, g => g.ToList());

            // Resetear a cero los nodos que tienen hijos
            foreach (var parentId in childrenByParentId.Keys)
            {
                if (idToNode.TryGetValue(parentId, out var parentNode))
                {
                    parentNode.MaxMembersApproved = 0;
                }
            }

            // Cola inicial de hojas (nodos sin hijos)
            var queue = new Queue<WorkPlaceUnit>(
                allWorkPlaces.Where(w => !childrenByParentId.ContainsKey(w.Id))
            );

            // Procesar la cola
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                if (!currentNode.ParentId.HasValue)
                    continue;

                if (idToNode.TryGetValue(currentNode.ParentId.Value, out var parent))
                {
                    // Sumar el valor del hijo al padre
                    parent.MaxMembersApproved += currentNode.MaxMembersApproved;

                    // Eliminar el hijo procesado de la lista del padre
                    if (childrenByParentId.TryGetValue(parent.Id, out var childrenList))
                    {
                        childrenList.Remove(currentNode);
                        if (childrenList.Count == 0)
                        {
                            // Todos los hijos procesados, encolar el padre
                            childrenByParentId.Remove(parent.Id);
                            queue.Enqueue(parent);
                        }
                    }
                }
            }
        }

        private void ReassignOrderBfs(List<WorkPlaceUnit> allUnits)
        {
            if (allUnits == null || allUnits.Count == 0)
                return;

            // Construir estructuras de datos
            var childrenByParentId = allUnits
                                     .Where(u => u.ParentId.HasValue)
                                     .GroupBy(u => u.ParentId.Value)
                                     .ToDictionary(g => g.Key, g => g.ToList());

            // Obtener y ordenar raíces
            var roots = allUnits.Where(u => !u.ParentId.HasValue)
                                .OrderBy(u => u.Order)
                                .ToList();

            // Asignar orden a raíces
            for (int i = 0; i < roots.Count; i++)
            {
                roots[i].Order = i;
            }

            // Procesar niveles usando BFS
            var queue = new Queue<WorkPlaceUnit>(roots);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (childrenByParentId.ContainsKey(current.Id))
                {
                    // Obtener y ordenar hijos
                    var children = childrenByParentId[current.Id]
                                   .OrderBy(c => c.Order)
                                   .ToList();

                    // Asignar nuevo orden
                    for (int i = 0; i < children.Count; i++)
                    {
                        children[i].Order = i;
                        queue.Enqueue(children[i]);
                    }
                }
            }
        }

        public async Task<List<WorkPlaceUnit>> FindChildrenAsync(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return await WorkPlaceUnitRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return await WorkPlaceUnitRepository.GetAllListAsync();
            }

            string code = await GetCodeAsync(parentId.Value);

            return await WorkPlaceUnitRepository.GetAllListAsync(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        public List<WorkPlaceUnit> FindChildren(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return WorkPlaceUnitRepository.GetAllList(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return WorkPlaceUnitRepository.GetAllList();
            }

            string code = GetCode(parentId.Value);

            return WorkPlaceUnitRepository.GetAllList(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }
    }
}
