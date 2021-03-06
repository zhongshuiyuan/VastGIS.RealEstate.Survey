// -------------------------------------------------------------------------------------------
// <copyright file="MainView.cs" company="MapWindow OSS Team - www.mapwindow.org">
//  MapWindow OSS Team - 2016
// </copyright>
// -------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using VastGIS.Api.Enums;
using VastGIS.Api.Interfaces;
using VastGIS.Helpers;
using VastGIS.Menu;
using VastGIS.Plugins.Concrete;
using VastGIS.Plugins.Events;
using VastGIS.Plugins.Interfaces;
using VastGIS.Plugins.Mvp;
using VastGIS.Shared;
using VastGIS.UI.Docking;
using VastGIS.UI.Forms;
using VastGIS.UI.Helpers;
using VastGIS.UI.Menu;

namespace VastGIS.Views
{
    /// <summary>
    /// Represents the main view of the application with the map, docking windows, toolbars and menu.
    /// </summary>
    public partial class MainView : MapWindowView, IMainView
    {
        public const string SerializationKey = ""; // intentionally empty
        private const string WindowTitle = "MapWindow 5";
        private readonly IAppContext _context;
        private bool _locked;
        private bool _rendered;

        public MainView(IAppContext context)
        {
            Logger.Current.Trace("Start MainView");
            _context = context;

            Logger.Current.Trace("Start MainView.InitializeComponent");
            InitializeComponent();
            Logger.Current.Trace("End MainView.InitializeComponent");

            statusStripEx1.Items.Clear();
            statusStripEx1.Refresh();

            ToolTipHelper.Init(superToolTip1);

            FormClosing += MainView_FormClosing;

            // setting bar item text before form is shown results in creation of duplicated bar item;
            // it seems it's a bug in Syncfusion's XpMenus
            Shown += (s, e) =>
                {
                    _rendered = true;

                    UpdateView();

                    ForceTaskBarDisplay();
                };
            Logger.Current.Trace("End MainView");
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00040000; // Turn on WS_EX_APPWINDOW
                return cp;
            }
        }

        public event EventHandler<CancelEventArgs> ViewClosing;

        public event EventHandler<RenderedEventArgs> ViewUpdating;

        public event Action BeforeShow;

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
            UpdateView();
        }

        private bool FireViewClosing()
        {
            var handler = ViewClosing;
            if (handler != null)
            {
                var args = new CancelEventArgs();
                handler(this, args);
                if (args.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private void FireViewUpdating(bool rendered)
        {
            var handler = ViewUpdating;
            if (handler != null)
            {
                handler(this, new RenderedEventArgs { Rendered = rendered });
            }
        }

        private void ForceTaskBarDisplay()
        {
            // I found no better solution. There several similar issues reported 
            // but no explanation as to why it happens or any clean way to fix it.
            // http ://stackoverflow.com/questions/18701186/form-is-not-visible-on-taskbar
            // http ://stackoverflow.com/questions/6937183/application-not-visible-in-taskbar-when-using-application-run
            // http ://stackoverflow.com/questions/4183809/main-form-not-shown-in-taskbar
            // In theory form should appear in task bar when WS_EX_APPWINDOW is set or it's top level unowned form plus ShowInTaskBar 
            // is not set to false manually. All these conditions are met in our case.
            // http ://stackoverflow.com/questions/8204397/what-does-ws-ex-appwindow-do
            using (
                var form = new Form
                               {
                                   Width = 0,
                                   Height = 0,
                                   Left = -500,
                                   Top = 0,
                                   StartPosition = FormStartPosition.Manual
                               })
            {
                form.Show(this);
                form.Close();
            }
        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if there are hidden child forms which override FormClosing
            // the initial value is set to true: https ://msdn.microsoft.com/en-us/library/system.windows.forms.form.formclosing%28v=vs.110%29.aspx
            // currently it may be the case with non-modal Find / Replace form in the table editor
            e.Cancel = false;

            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                return;
            }

            if (!FireViewClosing())
            {
                e.Cancel = true;
            }
            else
            {
                _dockingManager1.SaveLayout(SerializationKey, false);
                _mainFrameBarManager1.SaveLayout(SerializationKey, false);
            }
        }

        #region IView implementation

        private void RestorePreviousState()
        {
            _dockingManager1.TryRestoreLayout(SerializationKey);
            _mainFrameBarManager1.TryRestoreLayout(SerializationKey);
        }

        public override void ShowView(IWin32Window parent = null)
        {
            RestorePreviousState();

            DockPanelHelper.ClosePanel(_context, DockPanelKeys.TableEditor);
            DockPanelHelper.ClosePanel(_context, DockPanelKeys.Tasks);

            Program.Timer.Stop();
            Logger.Current.Info("Loading time: " + Program.Timer.Elapsed);

            SplashView.Instance.Close();

            _context.DockPanels.Unlock();

            // don't set it initially or it will cause a lot of resizing
            // with reallocation of buffer when panels / toolbars are loaded
            _mapControl1.Dock = DockStyle.Fill;

            statusStripEx1.ContextMenuStrip.Opening += OnStatusBarCustomizationMenuOpening;

            Invoke(BeforeShow);

            AppConfig.Instance.FirstRun = false;

            Show();

            Activate();

            Application.Run(this);
        }

        private void UpdateStatusBarCustomizationMenu()
        {
            var hash = new HashSet<string>
                           {
                               StatusBarKeys.TileProvider,
                               StatusBarKeys.MapScale,
                               StatusBarKeys.MapUnits,
                               StatusBarKeys.ProjectionDropDown
                           };
            try
            {
                foreach (ToolStripItem item in statusStripEx1.ContextMenuStrip.Items)
                {
                    var status = ReflectionHelper.GetInstanceField(item, "m_item") as ToolStripItem;
                    if (status == null)
                    {
                        item.Visible = false;
                        continue;
                    }

                    var metadata = status.Tag as MenuItemMetadata;

                    if (metadata != null && hash.Contains(metadata.Key))
                    {
                        item.Text = StatusBarKeys.GetStatusItemName(metadata.Key);
                        item.Visible = true;
                        continue;
                    }

                    item.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Current.Warn("Failed to update status bar customization menu.", ex);
            }
        }

        private void OnStatusBarCustomizationMenuOpening(object sender, CancelEventArgs e)
        {
            UpdateStatusBarCustomizationMenu();
        }

        private string GetCaption()
        {
            string caption = WindowTitle;

            if (!_context.Project.IsEmpty)
            {
                caption += @" - " + _context.Project.Filename;
            }

            return caption;
        }

        public void DoUpdateView(bool focusMap = true)
        {
            if (_locked) return;

            Text = GetCaption();

            // broadcast to plugins
            if (_rendered)
            {
                FireViewUpdating(_rendered);
            }

            if (ActiveForm == _mapControl1.ParentForm && focusMap)
            {
                _mapControl1.Focus();
            }
        }

        public override void UpdateView()
        {
            DoUpdateView();
        }

        public ButtonBase OkButton
        {
            get { return null; }
        }

        #endregion

        #region IMainView implementation

        public object DockingManager
        {
            get { return _dockingManager1; }
        }

        public object MenuManager
        {
            get { return _mainFrameBarManager1; }
        }

        public object StatusBar
        {
            get { return statusStripEx1; }
        }

        public IMap Map
        {
            get { return _mapControl1; }
        }

        public IView View
        {
            get { return this; }
        }

        public object RibbonControlAdv
        {
            get { return null; }
        }
        public MainViewType ViewType
        {
            get { return MainViewType.Normal; }
        }

        #endregion
    }
}