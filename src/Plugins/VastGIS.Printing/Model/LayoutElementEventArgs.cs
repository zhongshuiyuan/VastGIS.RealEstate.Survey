// -------------------------------------------------------------------------------------------
// <copyright file="LayoutElementEventArgs.cs" company="MapWindow OSS Team - www.mapwindow.org">
//  MapWindow OSS Team - 2015
// </copyright>
// -------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using VastGIS.Plugins.Printing.Model.Elements;

namespace VastGIS.Plugins.Printing.Model
{
    public class LayoutElementEventArgs : CancelEventArgs
    {
        public LayoutElementEventArgs(LayoutElement element)
        {
            if (element == null) throw new ArgumentNullException("element");

            Element = element;
        }

        public LayoutElement Element { get; private set; }
    }
}