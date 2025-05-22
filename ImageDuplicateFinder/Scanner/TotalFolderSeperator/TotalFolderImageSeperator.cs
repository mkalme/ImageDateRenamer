using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageDuplicateFinder {
    public class TotalFolderImageSeperator {
        public string MainFolder { get; set; }
        public string Folder { get; set; }

        public TotalFolderImageSeperator(string mainFolder, string folder) {
            MainFolder = mainFolder;
            Folder = folder;
        }

        public void Seperate(string destination) {
            string[] mainFolders = GetValidFolderNames(MainFolder).ToArray();

            List<string> mainImages = new List<string>();
            List<string> images = new List<string>();

            foreach (var main in mainFolders) {
                mainImages.AddRange(Directory.GetFiles($"{MainFolder}\\{main}"));

                string otherDirectory = $"{Folder}\\{main}";
                if (Directory.Exists(otherDirectory)) {
                    images.AddRange(Directory.GetFiles(otherDirectory));
                }
            }

            var scanner = new ImageFolderDuplicateScanner(mainImages.ToArray(), images.ToArray());
            IList<FolderImageDuplicate> duplicates = scanner.Scan();

            int count = 0;
            foreach (var duplicate in duplicates) {
                foreach (var image in duplicate.Duplicates) {
                    string newFile = $"{destination}\\{Path.GetFileName(image)}";

                    if (!File.Exists(newFile)) {
                        File.Move(image, newFile);
                    }
                }

                count++;
                Console.WriteLine($"{count}/{duplicates.Count}");
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
