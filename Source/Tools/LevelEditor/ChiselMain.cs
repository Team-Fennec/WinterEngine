using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using log4net;

namespace Chisel
{
    public class ChiselMain : Window
    {
        private static readonly ILog m_Log = LogManager.GetLogger("ChiselMain");

        [UI] private Image Chisel_StatusImage = null;
        [UI] private Label Chisel_StatusText = null;

        public ChiselMain() : this(new Builder("ChiselMain.glade")) { }

        private ChiselMain(Builder builder) : base(builder.GetRawOwnedObject("ChiselMainWindow"))
        {
            builder.Autoconnect(this);

            Icon = Gdk.Pixbuf.LoadFromResource("icons/Icon.png");
            SetStatusIndicator("Ready", "status-ok");
        }

        private void SetStatusIndicator(string text, string icon)
        {
            Chisel_StatusImage.Pixbuf = Gdk.Pixbuf.LoadFromResource($"icons/{icon}.png");
            Chisel_StatusText.Text = text;
        }
    }
}