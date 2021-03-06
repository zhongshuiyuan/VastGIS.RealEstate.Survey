// -------------------------------------------------------------------------------------------
// <copyright file="PageSelectionEventArgs.cs" company="MapWindow OSS Team - www.mapwindow.org">
//  MapWindow OSS Team - 2015
// </copyright>
// -------------------------------------------------------------------------------------------

using System;

namespace VastGIS.Plugins.Printing.Model
{
    public class PageSelectionEventArgs : EventArgs
    {
        public int Count;
        public int Selected;
    }
}