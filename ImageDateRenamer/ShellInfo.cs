using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace ImageDateRenamer {
    public class ShellInfo {
        private static readonly int[] _flags = new int[] { 3, 4, 5, 12, 27, 136, 190, 208 };
        private static readonly char[] _charactersToRemove = new char[] { (char)8206, (char)8207 };

        private static Shell32.Shell _shell = new Shell32.Shell();
        private static IDictionary<string, Shell32.Folder> _folderCache = new ConcurrentDictionary<string, Shell32.Folder>();

        public static IList<DateTime> GetMediaProperties(string file) {
            Shell32.Folder folder = GetFolder(file);
            Shell32.FolderItem imageItem = folder.ParseName(Path.GetFileName(file));

            List<DateTime> output = new List<DateTime>(16);

            for (int i = 0; i < _flags.Length; i++) {
                var details = folder.GetDetailsOf(imageItem, _flags[i]);
                if (details == string.Empty) continue;

                foreach (var c in _charactersToRemove) {
                    details = details.Replace(c.ToString(), "").Trim();
                }

                if (DateTime.TryParse(details, out DateTime time)) {
                    output.Add(time);
                }
            }

            return output;
        }

        private static Shell32.Folder GetFolder(string file) {
            string folderPath = Path.GetDirectoryName(file);

            Shell32.Folder folder;

            if (!_folderCache.TryGetValue(folderPath, out folder)) { 
                folder = _shell.NameSpace(new DirectoryInfo(folderPath).FullName);

                _folderCache.Add(folderPath, folder);
            }

            return folder;
        }
    }
}
