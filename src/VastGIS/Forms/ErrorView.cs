// -------------------------------------------------------------------------------------------
// <copyright file="ErrorView.cs" company="MapWindow OSS Team - www.mapwindow.org">
//  MapWindow OSS Team - 2016
// </copyright>
// -------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using VastGIS.Plugins.Services;
using VastGIS.Services.Helpers;
using VastGIS.Shared;
using VastGIS.UI.Forms;
using Syncfusion.Windows.Forms.Tools;

namespace VastGIS.Forms
{
    public partial class ErrorView : MapWindowForm
    {
        private const string ReportIssueUrl = "http://mapwindow5.codeplex.com/workitem/list/basic";
        private readonly Exception _exception;

        public ErrorView(Exception ex, bool needClose)
        {
            InitializeComponent();

            Text = needClose ? "Ooops, we are down" : "Something went wrong";

            _exception = ex;

            treeViewAdv1.HScroll = false;
            treeViewAdv1.AutoScrolling = ScrollBars.Vertical;
            treeViewAdv1.HideSelection = false;

            ShowMessage(needClose);

            ShowError(ex);
        }

        private void AddExceptionNodesToTree(Exception ex, TreeNodeAdv parent)
        {
            var node = new TreeNodeAdv { Text = ex.Message, MultiLine = true, Tag = ex, Expanded = true };

            parent.Nodes.Add(node);

            if (ex.InnerException != null)
            {
                AddExceptionNodesToTree(ex.InnerException, parent);
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(_exception.ExceptionToString());
                PathHelper.OpenUrl(ReportIssueUrl);
            }
            catch (Exception ex)
            {
                MessageService.Current.Info("Sorry, failed to do this either.");
            }
        }

        private void ErrorView_Load(object sender, EventArgs e)
        {
            // Fixing CORE-160
            CaptionFont = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(_exception.ExceptionToString());
            }
            catch (Exception ex)
            {
                MessageService.Current.Info("Sorry, failed to do this either.");
            }
        }

        private void OntreeViewAdvAfterSelect(object sender, EventArgs e)
        {
            var node = treeViewAdv1.SelectedNode;
            if (node != null)
            {
                var ex = node.Tag as Exception;
                if (ex != null)
                {
                    textBoxExt1.Text = ex.StackTrace;
                }
            }
        }

        private void ShowError(Exception ex)
        {
            if (ex == null)
            {
                textBoxExt1.Text = @"No information about the error was provided.";
                return;
            }

            var node = new TreeNodeAdv { Text = @"Details", Expanded = true };

            treeViewAdv1.Nodes.Add(node);

            AddExceptionNodesToTree(ex, node);

            if (node.Nodes.Count > 0)
            {
                treeViewAdv1.SelectedNode = node.Nodes[0];
            }
        }

        private void ShowMessage(bool needClose)
        {
            string s = "An unhandled exception has occured in the application. ";

            if (needClose)
            {
                s += "There is no way to recover from it. The application will be closed.";
            }

            s += " Please report the issue at " + ReportIssueUrl + ".";

            lblMessage.Text = s;

            btnContinue.Visible = !needClose;
        }
    }
}