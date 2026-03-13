using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModemMergerWinFormsApp
{
    public static class ModemHistory
    {
        private static readonly List<string> _history = new List<string>();
        private static readonly string _filePath;
        private static bool _loaded;
        private const int MaxItems = 5;

        static ModemHistory()
        {
            _filePath = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                "modem_history.txt");
        }

        public static List<string> GetHistory()
        {
            EnsureLoaded();
            return new List<string>(_history);
        }

        public static void Add(string modemNo)
        {
            if (string.IsNullOrWhiteSpace(modemNo) || modemNo.Length != 7) return;
            EnsureLoaded();
            _history.Remove(modemNo);
            _history.Insert(0, modemNo);
            if (_history.Count > MaxItems) _history.RemoveRange(MaxItems, _history.Count - MaxItems);
            Save();
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;
            try
            {
                if (File.Exists(_filePath))
                {
                    var lines = File.ReadAllLines(_filePath)
                        .Where(l => l.Trim().Length == 7)
                        .Take(MaxItems)
                        .ToList();
                    _history.Clear();
                    _history.AddRange(lines);
                }
            }
            catch { }
        }

        private static void Save()
        {
            try { File.WriteAllLines(_filePath, _history); }
            catch { }
        }
    }
}
