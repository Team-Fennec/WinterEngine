using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using log4net;

namespace MaterialEditor
{
    public class MainWindow : Window
    {
        private static readonly ILog m_Log = LogManager.GetLogger("MaterialEditor");

        [UI] private Image matedit_StatusImage = null;
        [UI] private Label matedit_StatusText = null;
        [UI] private Image matedit_RenderImage = null;
        [UI] private ToolButton matedit_tb_NewMaterial = null;
        [UI] private ToolButton matedit_tb_OpenMaterial = null;
        [UI] private ToolButton matedit_tb_SaveMaterial = null;
        [UI] private ToolButton matedit_tb_SaveAsMaterial = null;
        [UI] private Notebook mat_OpenMaterialTabs = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MaterialEditorWindow"))
        {
            builder.Autoconnect(this);

            Icon = Gdk.Pixbuf.LoadFromResource("icons/Icon.png");
            SetStatusIndicator("Ready", "status-ok");
        }

        private void SetStatusIndicator(string text, string icon)
        {
            matedit_StatusImage.Pixbuf = Gdk.Pixbuf.LoadFromResource($"icons/{icon}.png");
            matedit_StatusText.Text = text;
        }

        private void CreateNewMaterialTab()
        {
            // todo: button pressed code
        }
    }
}