using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemToolbarIE
{
    /// <summary>
    /// Element type
    /// </summary>
    public enum ToolbarItemType
    {
        Unknown = 0,
        Link,
        LinkList,
        RssTicker,
        SearchBox,
        Widget,
        MainMenu,
    };

    /// <summary>
    /// Element interface.
    /// </summary>
    public interface IToolbarItem : IDisposable
    {
        /// <summary>
        /// Visibylity.
        /// </summary>
        bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Type identifier
        /// </summary>
        ToolbarItemType TypeID
        {
            get;
        }

        /// <summary>
        /// State clearing.
        /// </summary>
        void Reset();
    }
}
