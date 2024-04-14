﻿using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace ModelConfEditor
{
    public class MainWindow : Window
    {
        [UI] private Label _label1 = null;
        [UI] private Button _button1 = null;

        private int _counter;

        public MainWindow() : this(new Builder("MdlConfWin.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MdlConfWin"))
        {
            builder.Autoconnect(this);

            _button1.Clicked += Button1_Clicked;
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            _counter++;
            _label1.Text = "Hello World! This button has been clicked " + _counter + " time(s).";
        }
    }
}