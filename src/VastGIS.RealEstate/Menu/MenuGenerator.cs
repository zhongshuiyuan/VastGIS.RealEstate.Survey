// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuGenerator.cs" company="MapWindow OSS Team - www.mapwindow.org">
//   MapWindow OSS Team - 2015
// </copyright>
// <summary>
//   The menu generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VastGIS.Plugins.Concrete;
using VastGIS.Plugins.Enums;
using VastGIS.Plugins.Interfaces;
using VastGIS.Plugins.ShapeEditor;

namespace VastGIS.Plugins.RealEstate.Menu
{
    #region

    #endregion

    /// <summary>
    /// The menu generator.
    /// </summary>
    public class MenuGenerator
    {
        #region Fields

        /// <summary>
        /// The menu commands.
        /// </summary>
        private readonly MenuCommands _commands;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuGenerator"/> class.
        /// </summary>
        /// <param name="context">
        /// The application context.
        /// </param>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        public MenuGenerator(IAppContext context, RealEstateEditor plugin)
        {
            _commands = new MenuCommands(context,plugin);
            InitMenu(context, plugin.Identity);
        }
        #endregion

        public MenuCommands MenuCommands { get { return _commands; } }
        #region Methods

        /// <summary>
        /// Initialize the new menu.
        /// </summary>
        /// <param name="context">
        /// The application context.
        /// </param>
        /// <param name="identity">
        /// The plug-in identity.
        /// </param>
        private void InitMenu(IAppContext context, PluginIdentity identity)
        {
            var menu = context.Menu.Items.AddDropDown("不动产", "_", identity);
            foreach (ICommand menuCommand in _commands.GetCommands())
            {
                menu.SubItems.AddButton(menuCommand.Text, menuCommand.Key,menuCommand.Icon, identity);
            }
            //menu.SubItems.AddButton("打开不动产数据库", MenuKeys.OpenRealEstateDatabase, identity);
            //menu.SubItems.AddButton("启动编辑", MenuKeys.StartEditingRealEstateDatabase, identity);
        }

        /// <summary>
        /// Initialize the toolbar.
        /// </summary>
        /// <param name="context">
        /// The application context.
        /// </param>
        /// <param name="identity">
        /// The plug-in identity.
        /// </param>
        private void InitToolbar(IAppContext context, PluginIdentity identity)
        {
            // Create a new toolbar
            var bar = context.Toolbars.Add("Template Plugin toolbar", identity);
            bar.DockState = ToolbarDockState.Top;

            // Add toolbar buttons, use MenuKeys to identify the buttons and add the command in MenuCommands:
            bar.Items.AddButton(_commands[MenuKeys.ShowDockableWindow]);
        }

        #endregion
    }
}