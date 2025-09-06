using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using Itenso.TimePeriod;

namespace Kontecg.Runtime
{
    public static class WinFormsRuntimeContext
    {
        private static string _display;

        public static event EventHandler DisplayChanged;

        public static IServiceProvider ServiceProvider { get; set; }

        public static ITimeCalendar Calendar { get; set; }

        public static string Display
        {
            get => _display;
            set
            {
                _display = value;
                DisplayChanged?.Invoke(new object(), EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a key/value collection that can be used to share data within the scope of this context.
        /// </summary>
        public static IDictionary<object, object> Items { get; set; }

        static WinFormsRuntimeContext()
        {
            Items = new ConcurrentDictionary<object, object>();
        }
    }
}
