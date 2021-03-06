using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VastGIS.Plugins.Concrete;
using VastGIS.Plugins.Enums;
using VastGIS.Plugins.Helpers;
using VastGIS.Plugins.Interfaces;
using VastGIS.Plugins.Mvp;
using VastGIS.Plugins.Services;
using VastGIS.Shared;

namespace VastGIS.Plugins
{
    internal static class CompositionRoot
    {
        public static void Compose(IApplicationContainer container)
        {
            container.RegisterSingleton<IBroadcasterService, PluginBroadcaster>()
                .RegisterSingleton<IPluginManager, PluginManager>()
                .RegisterSingleton<MainPlugin>();

            EnumHelper.RegisterConverter(new GdalDriverMetadataConverter());
            EnumHelper.RegisterConverter(new ZoomBoxStyleConverter());
            EnumHelper.RegisterConverter(new ZoombarVerbosityConverter());
            EnumHelper.RegisterConverter(new MouseWheelDirectionConverter());
            EnumHelper.RegisterConverter(new ZoomBehaviorConverter());
            EnumHelper.RegisterConverter(new ScalebarUnitsConverter());
            EnumHelper.RegisterConverter(new ResizeBehaviorConverter());
            EnumHelper.RegisterConverter(new AutoToggleConverter());
            EnumHelper.RegisterConverter(new ProjectionAbsenceConverter());
            EnumHelper.RegisterConverter(new ProjectionMistmatchConverter());
            EnumHelper.RegisterConverter(new SymbologyStorageConverter());
            EnumHelper.RegisterConverter(new ColorInterpretationConverter());
            EnumHelper.RegisterConverter(new UnitsOfMeasureConverter());
            EnumHelper.RegisterConverter(new GisTaskStatusConverter());
            EnumHelper.RegisterConverter(new GroupOperationConverter());
            EnumHelper.RegisterConverter(new ClipOperationConverter());
            EnumHelper.RegisterConverter(new TilesMaxAgeConverter());
            EnumHelper.RegisterConverter(new TileProjectionConverter());
        }
    }
}
