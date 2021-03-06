// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionRoot.cs" company="MapWindow OSS Team - www.mapwindow.org">
//   MapWindow OSS Team - 2015
// </copyright>
// <summary>
//   The composition root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VastGIS.Plugins.Mvp;
using VastGIS.Plugins.Repository.Views;

namespace VastGIS.Plugins.Repository
{
    internal static class CompositionRoot
    {
        public static void Compose(IApplicationContainer container)
        {
            container.RegisterSingleton<RepositoryDockPanel>();
        }
    }
}