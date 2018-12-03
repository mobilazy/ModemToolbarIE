using Microsoft.Win32;
using mshtml;
using SHDocVw;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BandObjectLib
{
    public class MiscClasses
    {
    }

    public class ModemEventArgs : EventArgs
    {
        public ModemEvents ModemEvent { get; set; }
        public string ModemNo { get; set; }
    }

    public enum ModemEvents
    {

        None,
        Gant,
        View,
        BhaView,
        DdView,
        GpView,
        Edit,
        BhaEdit,
        DdEdit,
        GpEdit

    }
}
