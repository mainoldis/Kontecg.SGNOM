using DevExpress.Utils.MVVM;
using Kontecg.Runtime.Events;
using Kontecg.Timing;
using Kontecg.ViewModels;
using System;
using System.ComponentModel;
using System.Drawing;
using Kontecg.Presenters;
using Kontecg.Behaviors;
using DevExpress.XtraTreeList;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Kontecg.Organizations.Dto;

namespace Kontecg.Views
{
    public partial class MigratingOrganizationUnits : BaseForm, ISupportViewModel
    {
        private int _loading = 0;
        private readonly MVVMContext _context;
        private readonly AppTimes _appTimes;

        private Point _dragStartPoint;
        private bool _isDragging = false;

        public MigratingOrganizationUnits()
        {
            InitializeComponent();
        }

        public MigratingOrganizationUnits(AppTimes appTimes)
        {
            InitializeComponent();

            _context = new MVVMContext(components);
            _context.ContainerControl = this;
            _context.ViewModelType = typeof(MigratingOrganizationUnitsViewModel);
            _context.AttachBehavior<FormCloseBehavior>(this);

            _appTimes = appTimes;
            ViewModel.Owner = this;
            ViewModel.TreeDataLoadCompleted += (sender, e) => treeOU.ExpandToLevel(0);


            ConfigureDataBindings();
            ConfigureManualDragDrop();
            ConfigureContextMenu();
        }

        public MigratingOrganizationUnitsViewModel ViewModel => _context.GetViewModel<MigratingOrganizationUnitsViewModel>();

        object ISupportViewModel.ViewModel => ViewModel;

        /// <inheritdoc />
        void ISupportViewModel.ParentViewModelAttached()
        {
        }

        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode)
                return;

            _loading++;
            try
            {
                EventBus.Trigger(this, new StartupProcessChanged(L("PopulatingUI")));
                base.OnLoad(e);

                FiltersTreeListAppearancesHelper.Apply(treeOU);
                GridHelper.SetFindControlImages(gridMaster);

                ViewModel.LoadTree();
            }
            catch (KontecgException ex)
            {
                Logger.Warn("An error was happened loading MainWindow", ex);
            }
            finally
            {
                EventBus.Trigger(this, new StartupProcessChanged(L("InitializationCompleted")));
                _loading--;
            }
        }

        /// <inheritdoc />
        protected override void LocalizeIsolatedItems()
        {
            Text = L("OrganizationUnits");
        }

        /// <inheritdoc />
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            EventBus.Trigger(this, new StartupProcessCompleted(Clock.Now - _appTimes.StartupTime));
        }

        /// <inheritdoc />
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_loading > 0) e.Cancel = true;

            // Limpiar eventos
            gridView.MouseDown -= OnGridMouseDown;
            gridView.MouseMove -= OnGridMouseMove;
            gridView.MouseUp -= OnGridMouseUp;

            treeOU.CustomDrawNodeCell -= TreeCustomDrawNodeCell;
            treeOU.MouseDown -= OnTreeMouseDown;
            treeOU.MouseMove -= OnTreeMouseMove;
            treeOU.MouseUp -= OnTreeMouseUp;
            treeOU.DragEnter -= TreeDragEnter;
            treeOU.DragOver -= TreeDragOver;
            treeOU.DragDrop -= TreeDragDrop;

            base.OnClosing(e);
        }

        private void ConfigureDataBindings()
        {
            var bindings = _context.OfType<MigratingOrganizationUnitsViewModel>();
            
            bindings.BindCommand(btnRefreshTree, m => m.LoadTree);
            bindings.BindCommand(btnRefreshDocuments, m => m.LoadDocuments, m => m.SelectedWorkplacePayment);
            bindings.BindCommand(btnNewNode, m => m.New);
            

            bindings.SetBinding(treeOU, t => t.DataSource, x => x.AllNodes);
            bindings.SetBinding(gridMaster, t => t.DataSource, x => x.EmploymentDocuments);
        }

        private void ConfigureManualDragDrop()
        {
            // Configurar GridView para drag
            gridView.MouseDown += OnGridMouseDown;
            gridView.MouseMove += OnGridMouseMove;
            gridView.MouseUp += OnGridMouseUp;

            // Configurar TreeList para drop
            treeOU.AllowDrop = true;
            treeOU.DragEnter += TreeDragEnter;
            treeOU.DragOver += TreeDragOver;
            treeOU.DragDrop += TreeDragDrop;
            treeOU.MouseDown += OnTreeMouseDown;
            treeOU.MouseMove += OnTreeMouseMove;
            treeOU.MouseUp += OnTreeMouseUp;
            treeOU.CustomDrawNodeCell += TreeCustomDrawNodeCell;

            // Configurar ImageLists para iconos
            treeOU.SelectImageList = imageCollection;
            treeOU.StateImageList = imageCollection;

            // Asignar iconos basados en el tipo de nodo
            treeOU.GetSelectImage += (sender, e) =>
            {
                var nodeType = (MigratingOrganizationUnitsViewModel.NodeType)e.Node.GetValue("NodeType");
                switch (nodeType)
                {
                    case MigratingOrganizationUnitsViewModel.NodeType.OrganizationUnit:
                        e.NodeImageIndex = 0;
                        break;
                    case MigratingOrganizationUnitsViewModel.NodeType.Level:
                        e.NodeImageIndex = 1;
                        break;
                    case MigratingOrganizationUnitsViewModel.NodeType.WorkPlace:
                        e.NodeImageIndex = 2;
                        break;
                    case MigratingOrganizationUnitsViewModel.NodeType.Document:
                        e.NodeImageIndex = 3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            treeOU.FocusedNodeChanged += (sender, e) =>
            {
                if (e.Node != null)
                {
                    var dataRecord = treeOU.GetDataRecordByNode(e.Node);
                    if (dataRecord is MigratingOrganizationUnitsViewModel.InfoNode selectNode)
                    {
                        ViewModel.SelectedWorkplacePayment = selectNode.WorkPlacePaymentCode;
                    }
                }
            };

            // Asegurar que los paneles del SplitContainer permitan el paso de eventos
            splitContainerControl1.Panel1.AllowDrop = true;
            splitContainerControl1.Panel2.AllowDrop = true;

            // En el método ConfigureManualDragDrop o después de inicializar el splitContainerControl1
            splitContainerControl1.Panel1.Click += (s, e) => { };
            splitContainerControl1.Panel2.Click += (s, e) => { };

            // Asegurar que los eventos de ratón se propaguen correctamente
            splitContainerControl1.Panel1.MouseMove += (s, e) => { };
            splitContainerControl1.Panel2.MouseMove += (s, e) => { };
        }

        private void ConfigureContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var expandAll = new ToolStripMenuItem("Expandir Todo");
            expandAll.Click += (s, e) => treeOU.ExpandAll();

            var collapseAll = new ToolStripMenuItem("Colapsar Todo");
            collapseAll.Click += (s, e) => treeOU.CollapseAll();

            contextMenu.Items.Add(expandAll);
            contextMenu.Items.Add(collapseAll);

            treeOU.ContextMenuStrip = contextMenu;
        }

        private void OnGridMouseDown(object sender, MouseEventArgs e)
        {
            _dragStartPoint = e.Location;
            _isDragging = false;
        }

        private void OnGridMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_isDragging)
            {
                int deltaX = Math.Abs(e.X - _dragStartPoint.X);
                int deltaY = Math.Abs(e.Y - _dragStartPoint.Y);

                if (deltaX >= SystemInformation.DragSize.Width ||
                    deltaY >= SystemInformation.DragSize.Height)
                {
                    _isDragging = true;
                    StartDragOperation(sender is GridView);
                }
            }
        }

        private void OnGridMouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void OnTreeMouseDown(object sender, MouseEventArgs e)
        {
            _dragStartPoint = e.Location;
            _isDragging = false;
        }

        private void OnTreeMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_isDragging)
            {
                int deltaX = Math.Abs(e.X - _dragStartPoint.X);
                int deltaY = Math.Abs(e.Y - _dragStartPoint.Y);

                if (deltaX >= SystemInformation.DragSize.Width ||
                    deltaY >= SystemInformation.DragSize.Height)
                {
                    _isDragging = true;
                    StartDragOperation(sender is GridView);
                }
            }
        }

        private void OnTreeMouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void StartDragOperation(bool fromGridView = true)
        {
            MigratingOrganizationUnitsViewModel.InfoNode[] draggedData;

            if (fromGridView)
            {
                var selectedRows = gridView.GetSelectedRows();
                if (selectedRows.Length == 0) return;

                draggedData = selectedRows
                              .Select(rowHandle =>
                                  gridView.GetRow(rowHandle) as MigratingOrganizationUnitsViewModel.InfoNode)
                              .Where(node => node is {Document: not null})
                              .ToArray();
            }
            else
            {
                var selectedNode = treeOU.GetFocusedRow() as MigratingOrganizationUnitsViewModel.InfoNode;
                if (selectedNode == null) return;

                draggedData = [selectedNode];
            }

            if (draggedData.Length > 0)
            {
                // Usar DataObject para asegurar la compatibilidad
                var dataObject = new DataObject(draggedData);
                DoDragDrop(dataObject, DragDropEffects.Move);
            }
        }

        private void TreeCustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            var node = (MigratingOrganizationUnitsViewModel.NodeType)e.Node.GetValue("NodeType");
            if (node == MigratingOrganizationUnitsViewModel.NodeType.Document)
            {
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
        }

        private void TreeDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(MigratingOrganizationUnitsViewModel.InfoNode[])) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void TreeDragDrop(object sender, DragEventArgs e)
        {
            var draggedNodes = e.Data.GetData(typeof(MigratingOrganizationUnitsViewModel.InfoNode[])) as MigratingOrganizationUnitsViewModel.InfoNode[];
            if (draggedNodes == null) return;

            var treeList = (TreeList)sender;
            var hitInfo = treeList.CalcHitInfo(treeList.PointToClient(new Point(e.X, e.Y)));
            var targetNode = hitInfo.Node;

            if (targetNode == null) return;

            var targetWorkPlace = targetNode.GetValue("WorkPlace") as WorkPlaceUnitDto;
            if (targetWorkPlace == null) return;

            try
            {
                var workplaces = draggedNodes.Where(n => n.WorkPlace != null).ToArray();
                if (workplaces.Length == 1 && (workplaces[0].Level > targetWorkPlace.Level ||
                                               (workplaces[0].Level == targetWorkPlace.Level &&
                                                workplaces[0].ParentId != targetWorkPlace.Id)))
                {
                    ViewModel.MoveOrganizationUnit(workplaces[0].OrganizationUnitId, targetWorkPlace.Id);
                    workplaces[0].ParentId = targetWorkPlace.Id;
                    workplaces[0].WorkPlace.ParentId = targetWorkPlace.Id;
                }

                var documents = draggedNodes.Where(n => n.Document != null).ToArray();
                if (documents.Length > 0)
                {
                    var documentIds = documents.Select(d => d.Document.Id).ToList();
                    ViewModel.UpdateMultipleDocumentsOrganizationUnit(documentIds, targetWorkPlace.Id);

                    // Actualizar los nodos en la lista
                    foreach (var docNode in documents)
                    {
                        docNode.ParentId = targetWorkPlace.Id;
                        docNode.Document.OrganizationUnitId = targetWorkPlace.Id;
                    }
                }

                // Forzar la actualización del TreeList
                treeOU.RefreshDataSource();
                gridView.RefreshData();

                // Expandir el nodo de destino para mostrar los documentos
                targetNode.Expanded = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error al asociar documentos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isDragging = false;
            }
        }

        private void TreeDragOver(object sender, DragEventArgs e)
        {
            var draggedNodes = e.Data.GetData(typeof(MigratingOrganizationUnitsViewModel.InfoNode[])) as MigratingOrganizationUnitsViewModel.InfoNode[];

            if (draggedNodes == null || draggedNodes.Length == 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            var treeList = (TreeList)sender;
            var hitInfo = treeList.CalcHitInfo(treeList.PointToClient(new Point(e.X, e.Y)));
            
            if (hitInfo.Node?.GetValue("WorkPlace") == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;
        }
    }
}