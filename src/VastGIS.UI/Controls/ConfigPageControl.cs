using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VastGIS.Plugins.Interfaces;
using Syncfusion.Windows.Forms.Tools;

namespace VastGIS.UI.Controls
{
    public partial class ConfigPageControl : UserControl
    {
        public ConfigPageControl()
        {
            InitializeComponent();
        }

        public GradientPanel Panel
        {
            get { return gradientPanel1; }
        }

        public Image Icon
        {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }

        public string Description
        {
            get { return lblDescription.Text; }
            set { lblDescription.Text = value; }
        }

        public Control ConfigPage
        {
            get { return panelContent.Controls.Count > 0 ? panelContent.Controls[0] : null; }
            set
            {
                panelContent.Controls.Clear();

                var page = value as IConfigPage;
                if (page == null) return;

                panelContent.Padding = new Padding(page.VariableHeight ? 0 : 10, 10, 0, 0);
                panelContent.Height = page.VariableHeight ? AvailableHeight : page.OriginalSize.Height;
                panelContent.Width = page.VariableHeight ? Width : Width - 20;

                panelContent.Controls.Add(value);
            }
        }

        private int AvailableHeight
        {
            get { return gradientPanel1.Height - lblDescription.Height - 10; }
        }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        }
    }
}
