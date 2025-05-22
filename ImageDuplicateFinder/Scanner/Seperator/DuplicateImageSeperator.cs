using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageDuplicateFinder {
    public class DuplicateImageSeperator {
        public string[] Folders { get; set; }

        public DuplicateImageSeperator(string[] folders) {
            Folders = folders;
        }

        public void Seperate(string destination) {
            foreach (var folder in Folders) {
                var scanner = new ImageDuplicateScanner(folder);

                IList<ImageDuplicate> duplicates = scanner.Scan();
                for (int i = 0; i < duplicates.Count; i++) {
                    var duplicate = duplicates[i];

                    string stay = DetermineWhichImageToStay(duplicate.Images);
                    foreach (var image in duplicate.Images) {
                        if (stay == image) continue;

                        string newFile = $"{destination}\\{Path.GetFileName(image)}";
                        File.Move(image, newFile);
                    }
                }

                Console.WriteLine(folder);
            }
        }

        private string DetermineWhichImageToStay(IEnumerable<string> images) {
            string[] original = images.Where(x => !Path.GetFileNameWithoutExtension(x).EndsWith(")")).ToArray();

            if (original.Length == 1) return original[0];
            return images.First();
        }
    }
}
