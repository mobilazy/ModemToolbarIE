using System;


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
        GantTools,
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
