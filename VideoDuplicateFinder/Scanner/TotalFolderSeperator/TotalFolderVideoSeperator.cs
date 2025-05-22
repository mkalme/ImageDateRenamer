using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace VideoDuplicateFinder {
    public class TotalFolderVideoSeperator {
        public string MainFolder { get; set; }
        public string Folder { get; set; }

        public TotalFolderVideoSeperator(string mainFolder, string folder) {
            MainFolder = mainFolder;
            Folder = folder;
        }

        public void Seperate(string destination) {
            string[] mainFolders = GetValidFolderNames(MainFolder).ToArray();

            List<string> mainVideos = new List<string>();
            List<string> videos = new List<string>();

            foreach (var main in mainFolders) {
                mainVideos.AddRange(Directory.GetFiles($"{MainFolder}\\{main}"));

                string otherDirectory = $"{Folder}\\{main}";
                if (Directory.Exists(otherDirectory)) {
                    videos.AddRange(Directory.GetFiles(otherDirectory));
                }
            }

            var scanner = new VideoFolderDuplicateScanner(mainVideos.ToArray(), videos.ToArray());
            IList<FolderVideoDuplicate> duplicates = scanner.Scan();

            int count = 0;
            foreach (var duplicate in duplicates) {
                foreach (var video in duplicate.Duplicates) {
                    string newFile = $"{destination}\\{Path.GetFileName(video)}";

                    if (!File.Exists(newFile)) {
                        File.Move(video, newFile);
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
