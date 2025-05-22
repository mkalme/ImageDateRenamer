using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VideoDuplicateFinder {
    public class DuplicateVideoSeperator {
        public string[] Folders { get; set; }

        public DuplicateVideoSeperator(string[] folders) {
            Folders = folders;
        }

        public void Seperate(string destination) {
            int count = 0;

            Parallel.ForEach(Folders, folder => {
                var scanner = new VideoDuplicateScanner(folder);

                Console.WriteLine($"{count}/{Folders.Length} At: {folder}");

                IList<VideoDuplicate> duplicates = scanner.Scan();
                for (int i = 0; i < duplicates.Count; i++) {
                    var duplicate = duplicates[i];

                    string stay = DetermineWhichImageToStay(duplicate.Videos);
                    foreach (var video in duplicate.Videos) {
                        if (stay == video) continue;

                        string newFile = $"{destination}\\{Path.GetFileName(video)}";
                        File.Move(video, newFile);
                    }
                }

                Interlocked.Increment(ref count);
            });
        }

        private string DetermineWhichImageToStay(IEnumerable<string> videos) {
            string[] original = videos.Where(x => !Path.GetFileNameWithoutExtension(x).EndsWith(")")).ToArray();

            if (original.Length == 1) return original[0];
            return videos.First();
        }
    }
}
