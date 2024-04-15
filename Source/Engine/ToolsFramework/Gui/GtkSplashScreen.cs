using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using WinterEngine.Resource;

namespace WinterEngine.ToolsFramework.Gui
{
    public class GtkSplashWindow : Window
    {
        [UI] private Label splash_StatusText = null;
        [UI] private Image splash_Image = null;

        private Gdk.Pixbuf img_Norm;

        public GtkSplashWindow() : this(new Builder("YieldSplash.glade")) { }

        private GtkSplashWindow(Builder builder) : base(builder.GetRawOwnedObject("YieldSplashWindow"))
        {
            builder.Autoconnect(this);

            // Load images
            Stream normSplash = ResourceManager.GetData("resource/tools/splash_base.png");
            BinaryReader splashReader = new BinaryReader(normSplash);
            Gdk.PixbufLoader pixbufLoader = new Gdk.PixbufLoader();

            while (splashReader.BaseStream.Position != splashReader.BaseStream.Length)
            {
                pixbufLoader.Write(splashReader.ReadBytes((int)splashReader.BaseStream.Length));
            }
            splashReader.Close();

            splash_Image.Pixbuf = pixbufLoader.Pixbuf;
        }

        public void SetStatusText(string text)
        {
            splash_StatusText.Text = text;
        }
    }
}
