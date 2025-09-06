using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Kontecg.Domain.Uow;
using Kontecg.Organizations;
using Kontecg.Organizations.Dto;
using System.Windows.Forms;
using DevExpress.Mvvm.DataAnnotations;
using Kontecg.WorkRelations;
using Kontecg.WorkRelations.Dto;
using DevExpress.Mvvm.POCO;

namespace Kontecg.ViewModels
{
    public class MigratingOrganizationUnitsViewModel : KontecgViewModelBase
    {
        private readonly IWorkPlaceUnitAppService _placeUnitAppService;
        private readonly IWorkRelationsAppService _workRelationsAppService;
        private BindingList<InfoNode> _allNodes = new BindingList<InfoNode>();
        private BindingList<InfoNode> _employmentDocumentInfoDtos;
        private string _selectedWorkplacePayment = null;

        /// <inheritdoc />
        public MigratingOrganizationUnitsViewModel(
            IWorkPlaceUnitAppService placeUnitAppService,
            IWorkRelationsAppService workRelationsAppService)
        {
            LocalizationSourceName = "Launcher";
            _placeUnitAppService = placeUnitAppService;
            _workRelationsAppService = workRelationsAppService;
        }

        public Form Owner { get; set; }

        public event EventHandler TreeDataLoadCompleted;
        public event EventHandler DocsDataLoadCompleted;

        public string SelectedWorkplacePayment
        {
            get => _selectedWorkplacePayment;
            set
            {
                if (_selectedWorkplacePayment != value)
                {
                    _selectedWorkplacePayment = value;
                    this.RaisePropertyChanged(p => p.SelectedWorkplacePayment);
                }
            }
        }

        [Command]
        public void New()
        {
            // Lógica para crear nueva unidad organizativa
        }

        [Command]
        public void LoadTree()
        {
            try
            {
                var ous = UnitOfWorkManager.WithUnitOfWork(_placeUnitAppService.GetWorkPlaceUnits);

                // Convertir todas las unidades organizativas a InfoNode
                var ouNodes = ous.Items.Select(ConvertWorkPlaceUnitToInfoNode).ToList();

                var docs = UnitOfWorkManager.WithUnitOfWork(() =>
                    _workRelationsAppService.GetEmploymentDocuments(new FilterRequest
                    {
                        ExcludedOrganizationUnitIds = [1],
                        MaxResultCount = int.MaxValue
                    }));

                ouNodes = ouNodes.Concat(docs.Items.Select(ConvertDocumentToInfoNode).ToList()).ToList();

                // Limpiar y agregar nodos existentes
                AllNodes = new BindingList<InfoNode>(ouNodes);
                TreeDataLoadCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Logger.Warn("Error cargando la estructura!", e);
            }
        }

        [Command]
        public void LoadDocuments(string workPlaceUnit = null)
        {
            try
            {
                var docs = UnitOfWorkManager.WithUnitOfWork(() =>
                    _workRelationsAppService.GetEmploymentDocuments(new FilterRequest
                    {
                        OrganizationUnitIds = [1],
                        WorkPlacePaymentCode = workPlaceUnit, 
                        MaxResultCount = int.MaxValue
                    }));

                var nodes = docs.Items.Select(ConvertDocumentToInfoNode).ToList();
                EmploymentDocuments = new BindingList<InfoNode>(nodes);

                DocsDataLoadCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Logger.Warn("Error cargando documentos!", e);
            }
        }

        public BindingList<InfoNode> AllNodes
        {
            get => _allNodes;
            set
            {
                _allNodes = value;
                this.RaisePropertyChanged(m => m.AllNodes);
            }
        }

        public BindingList<InfoNode> EmploymentDocuments
        {
            get => _employmentDocumentInfoDtos;
            set
            {
                _employmentDocumentInfoDtos = value;
                this.RaisePropertyChanged(m => m.EmploymentDocuments);
            }
        }

        #region Info node

        private InfoNode ConvertDocumentToInfoNode(EmploymentDocumentInfoDto documentInfo)
        {
            return new InfoNode
            {
                Id = 1000 + documentInfo.Id,
                Document = documentInfo,
                ParentId = documentInfo.OrganizationUnitId,
                Code = documentInfo.ThirdLevelCode,
                DisplayName = $"{documentInfo.Exp} - {documentInfo.Person?.FullName}",
                Description = string.Empty,
                Classification = documentInfo.ThirdLevelDisplayName,
                Level = 4,
                WorkPlacePaymentCode = documentInfo.WorkPlacePaymentCode
            };
        }

        private InfoNode ConvertWorkPlaceUnitToInfoNode(WorkPlaceUnitDto workPlaceUnit)
        {
            return new InfoNode
            {
                WorkPlace = workPlaceUnit,
                Id = workPlaceUnit.Id,
                ParentId = workPlaceUnit.ParentId ?? 0,
                Code = workPlaceUnit.Code,
                DisplayName = workPlaceUnit.DisplayName,
                Classification = workPlaceUnit.Classification,
                Description = workPlaceUnit.Description,
                Level = workPlaceUnit.Level,
                WorkPlacePaymentCode = workPlaceUnit.WorkPlacePaymentCode
            };
        }

        public enum NodeType
        {
            OrganizationUnit = 0,
            Level = 1,
            WorkPlace = 2,
            Document = 3,
        }

        public class InfoNode
        {
            public long Id { get; set; }

            public long ParentId { get; set; }

            public string Code { get; set; }

            public string DisplayName { get; set; }

            public string Description { get; set; }

            public string Classification { get; set; }

            public int Level { get; set; }

            public string WorkPlacePaymentCode { get; set; }

            public WorkPlaceUnitDto WorkPlace { get; set; }

            public EmploymentDocumentInfoDto Document { get; set; }

            public NodeType NodeType => Document != null ? NodeType.Document :
                WorkPlace?.Level == 0 ? NodeType.OrganizationUnit :
                WorkPlace?.Level is 1 or 2 ? NodeType.Level : NodeType.WorkPlace;

            // Propiedad para fácil acceso al ID de la unidad organizativa
            public long OrganizationUnitId => Document?.OrganizationUnitId ?? Id;

            public InfoNode()
            {
            }
        }

        #endregion

        public void UpdateMultipleDocumentsOrganizationUnit(List<long> documentIds, long organizationUnitId)
        {
            try
            {
                UnitOfWorkManager.WithUnitOfWork(() =>
                {
                    _workRelationsAppService.UpdateDocumentOrganizationUnit(new UpdateWorkPlaceUnitInputDto
                    {
                        DocumentIds = documentIds, 
                        OrganizationUnitId = organizationUnitId
                    });
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating multiple documents organization unit", ex);
                throw;
            }
        }

        public void MoveOrganizationUnit(long sourceId, long targetId)
        {
            try
            {
                UnitOfWorkManager.WithUnitOfWork(() =>
                {
                    _placeUnitAppService.MoveWorkPlaceUnit(new MoveOrganizationUnitInput(){Id = sourceId, NewParentId = targetId});
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Error moving organization unit", ex);
                throw;
            }
        }

        public void CreateOrganizationUnit()
        {
            try
            {
                UnitOfWorkManager.WithUnitOfWork(() =>
                {
                    _placeUnitAppService.CreateWorkPlaceUnit(new CreateWorkPlaceUnitInput());
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating multiple documents organization unit", ex);
                throw;
            }
        }
    }
}