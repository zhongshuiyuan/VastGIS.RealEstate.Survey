using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VastGIS.Api.Interfaces;
using VastGIS.Api.Legend;
using VastGIS.Api.Legend.Abstract;
using VastGIS.Api.Legend.Events;
using VastGIS.Api.Static;
using VastGIS.Plugins;
using VastGIS.Plugins.Enums;
using VastGIS.Plugins.Events;
using VastGIS.Plugins.Helpers;
using VastGIS.Plugins.Interfaces;
using VastGIS.Plugins.Mvp;
using VastGIS.Plugins.Services;
using VastGIS.Services.Helpers;
using VastGIS.Services.Serialization;
using VastGIS.Views;

namespace VastGIS.Controls
{
    public class LegendPresenter: CommandDispatcher<LegendDockPanel, LegendCommand>
    {
        private readonly IAppContext _context;
        private readonly ILayerService _layerService;
        private readonly IBroadcasterService _broadcaster;
        private readonly LegendDockPanel _legendDockPanel;

        public LegendPresenter(IAppContext context, ILayerService layerService, IBroadcasterService broadcaster, 
                               LegendDockPanel legendDockPanel)
             : base(legendDockPanel)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (layerService == null) throw new ArgumentNullException("layerService");
            if (broadcaster == null) throw new ArgumentNullException("broadcaster");
            if (legendDockPanel == null) throw new ArgumentNullException("legendDockPanel");

            _context = context;
            _layerService = layerService;
            _broadcaster = broadcaster;
            _legendDockPanel = legendDockPanel;

            View.LegendKeyDown += OnLegendKeyDown;
        }

        private void OnLegendKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _legendDockPanel.Legend.SelectedLayer != null)
            {
                RunCommand(LegendCommand.RemoveLayer);
            }
        }

        public IMuteLegend Legend
        {
            get { return _legendDockPanel.Legend; }
        }

        public override void RunCommand(LegendCommand command)
        {
            switch (command)
            {
                case LegendCommand.AddGroup:
                    Legend.Groups.Add();
                    break;
                case LegendCommand.AddLayer:
                    {
                        int groupHandle = _legendDockPanel.SelectedGroupHandle;
                        if (_layerService.AddLayer(DataSourceType.All) && groupHandle != -1)
                        {
                            int layerHandle = _layerService.LastLayerHandle;
                            Legend.Layers.MoveLayer(layerHandle, groupHandle);
                        }
                        break;
                    }
                case LegendCommand.GroupProperties:
                    {
                        int groupHandle = _legendDockPanel.SelectedGroupHandle;
                        var group = Legend.Groups.ItemByHandle(groupHandle);
                        if (group != null)
                        {
                            if (_context.Container.Run<LegendGroupPresenter, ILegendGroup>(group))
                            {
                                Legend.Redraw();
                            }
                        }
                        break;
                    }
                case LegendCommand.ZoomToGroup:
                    {
                        var group = Legend.Groups.ItemByHandle(_legendDockPanel.SelectedGroupHandle);
                        if (group != null)
                        {
                            var box = group.Envelope;
                            _context.Map.ZoomToExtents(box);
                        }
                        break;
                    }
                case LegendCommand.RemoveGroup:
                    {
                        var group = Legend.Groups.ItemByHandle(_legendDockPanel.SelectedGroupHandle);
                        if (group != null)
                        {
                            if (MessageService.Current.Ask("Do you want to remove group: " + group.Text + "?"))
                            {
                                Legend.Groups.Remove(group.Handle);
                            }
                        }
                        break;
                    }
                case LegendCommand.Labels:
                    {
                        var layer = _context.Legend.Layers.Current;
                        if (layer != null && layer.IsVector)
                        {
                            _broadcaster.BroadcastEvent(p => p.LayerLabelsClicked_, _context.Legend, new LayerEventArgs(layer.Handle));
                        }
                    }
                    break;
                case LegendCommand.TableEditor:
                    {
                        var layer = _context.Legend.Layers.Current;
                        if (layer != null && layer.IsVector)
                        {
                            var args = new PluginMessageEventArgs(PluginMessages.ShowAttributeTable);
                            _broadcaster.BroadcastEvent(t => t.MessageBroadcasted_, _context.Legend, args);
                        }
                    }
                    break;
                case LegendCommand.ZoomToLayer:
                    _context.Map.ZoomToLayer(_context.Legend.SelectedLayerHandle);
                    break;
                case LegendCommand.RemoveLayer:
                    _layerService.RemoveSelectedLayer();
                    break;
                case LegendCommand.Properties:
                    _broadcaster.BroadcastEvent(p => p.LayerDoubleClicked_, Legend,
                        new LayerEventArgs(Legend.SelectedLayerHandle));
                    break;
                case LegendCommand.SaveStyle:
                    {
                        var layer = Legend.Layers.Current;

                        if (layer != null)
                        {
                            if (!LayerSerializationHelper.CheckFilename(layer.Filename))
                            {
                                MessageService.Current.Info("Can not save settings for a non-disk based layer.");
                                return;
                            }

                            LayerSerializationHelper.SaveSettings(layer);
                        }
                        break;
                    }
                case LegendCommand.LoadStyle:
                    {
                        var layer = Legend.Layers.Current;

                        if (layer != null)
                        {
                            if (!LayerSerializationHelper.CheckFilename(layer.Filename))
                            {
                                MessageService.Current.Info("Can not load settings for a non-disk based layer.");
                                return;
                            }

                            LayerSerializationHelper.LoadSettings(layer, _broadcaster, false);
                        }
                        
                        _context.Legend.Redraw(LegendRedraw.LegendAndMap);
                    }
                    break;
                case LegendCommand.OpenFileLocation:
                    {
                        var layer = Legend.Layers.Current;
                        if (layer != null && File.Exists(layer.Filename))
                        {
                            Shared.PathHelper.OpenFolderWithExplorer(layer.Filename);
                        }
                        else
                        {
                            MessageService.Current.Warn("Failed to find file for the layer.");
                        }
                        break;
                    }
            }
        }
    }
}
