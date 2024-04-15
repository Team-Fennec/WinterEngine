using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace ModelConfEditor
{
    public class MainWindow : Window
    {
        [UI] private Entry entry_ModelName = null;
        [UI] private Entry entry_ReferenceName = null;
        [UI] private Button animList_AddRowButton = null;
        [UI] private Button animList_RemRowButton = null;
        [UI] private ListBox animList_ListBox = null;

        public MainWindow() : this(new Builder("MdlConfWin.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MdlConfWin"))
        {
            builder.Autoconnect(this);
            Icon = Gdk.Pixbuf.LoadFromResource("icons/Icon.png");
        }

        private void OnAddPressed()
        {
            // todo: button pressed code
        }
    }
}