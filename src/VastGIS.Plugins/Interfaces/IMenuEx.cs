using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VastGIS.Plugins.Events;

namespace VastGIS.Plugins.Interfaces
{
    public interface IMenuEx: IMenuBase
    {
        event EventHandler<MenuItemEventArgs> ItemClicked;
    }
}
