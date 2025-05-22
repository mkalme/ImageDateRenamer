using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageDuplicateFinder {
    public class DuplicateFolderImageSeperator {
        public string MainFolder { get; set; }
        public string Folder { get; set; }

        public DuplicateFolderImageSeperator(string mainFolder, string folder) {
            MainFolder = mainFolder;
            Folder = folder;
        }

        public void Seperate(string destination) {
            string[] mainFolders = GetValidFolderNames(MainFolder).ToArray();
            HashSet<string> folders = GetValidFolderNames(Folder).ToHashSet();

            foreach (var main in mainFolders) {
                if (!folders.Contains(main)) continue;

                string mainPath = $"{MainFolder}\\{main}";
                string path = $"{Folder}\\{main}";

                var scanner = new ImageFolderDuplicateScanner(mainPath, path);
                IList<FolderImageDuplicate> duplicates = scanner.Scan();

                foreach (var duplicate in duplicates) {
                    foreach (var image in duplicate.Duplicates) {
                        string newFile = $"{destination}\\{Path.GetFileName(image)}";
                        
                        File.Move(image, newFile);
                    }
                }

                Console.WriteLine(main + ", " + duplicates.Count);
            }
        }

        private static IEnumerable<string> GetValidFolderNames(string folder) {
            return Directory.GetDirectories(folder).Select(x => Path.GetFileName(x)).Where(x => IsNameValid(x));
        }
        private static bool IsNameValid(string name) {
            return DateTime.TryParseExact(name, "yyyy-MM", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime time);
        }
    }
}
