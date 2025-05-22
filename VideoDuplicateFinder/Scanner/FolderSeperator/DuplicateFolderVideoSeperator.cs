using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VideoDuplicateFinder {
    public class DuplicateFolderVideoSeperator {
        public string MainFolder { get; set; }
        public string Folder { get; set; }

        public DuplicateFolderVideoSeperator(string mainFolder, string folder) {
            MainFolder = mainFolder;
            Folder = folder;
        }

        public void Seperate(string destination) {
            string[] mainFolders = GetValidFolderNames(MainFolder).ToArray();
            HashSet<string> folders = GetValidFolderNames(Folder).ToHashSet();

            folders.IntersectWith(mainFolders);

            int count = 0;

            Parallel.ForEach(mainFolders, main => {
                if (!folders.Contains(main)) return;

                string mainPath = $"{MainFolder}\\{main}";
                string path = $"{Folder}\\{main}";

                var scanner = new VideoFolderDuplicateScanner(mainPath, path);
                IList<FolderVideoDuplicate> duplicates = scanner.Scan();

                foreach (var duplicate in duplicates) {
                    foreach (var video in duplicate.Duplicates) {
                        string newFile = $"{destination}\\{Path.GetFileName(video)}";

                        if (!File.Exists(newFile)){
                            File.Move(video, newFile);
                        }
                    }
                }

                Interlocked.Increment(ref count);
                Console.WriteLine($"{count}/{folders.Count} {duplicates.Count}");
            });
        }

        private static IEnumerable<string> GetValidFolderNames(string folder) {
            return Directory.GetDirectories(folder).Select(x => Path.GetFileName(x)).Where(x => IsNameValid(x));
        }
        private static bool IsNameValid(string name) {
            return DateTime.TryParseExact(name, "yyyy-MM", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime time);
        }
    }
}
