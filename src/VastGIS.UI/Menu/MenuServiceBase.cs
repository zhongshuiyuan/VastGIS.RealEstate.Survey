using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VastGIS.Plugins.Concrete;
using VastGIS.Plugins.Interfaces;

namespace VastGIS.UI.Menu
{
    public abstract class MenuServiceBase
    {
        protected readonly IAppContext _context;
        protected readonly PluginIdentity _identity;
        protected IToolbarCollectionBase _toolbars;
        protected IMenuBase _menu;

        public MenuServiceBase(IAppContext context, PluginIdentity identity)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (identity == null) throw new ArgumentNullException("identity");

            _context = context;
            _identity = identity;
            _toolbars = context.Toolbars;
            _menu = context.Menu;
        }

        protected IMenuItem FindToolbarItem(string itemKey)
        {
            return _toolbars.FindItem(itemKey, _identity);
        }

        protected IMenuItem FindMenuItem(string itemKey)
        {
            return _menu.FindItem(itemKey, _identity);
        }

        protected ToolStripItem FindToolStripItem(string itemKey)
        {
            return _context.RibbonMenu.FindToolStripItem(itemKey, _identity);
        }
    }
}
