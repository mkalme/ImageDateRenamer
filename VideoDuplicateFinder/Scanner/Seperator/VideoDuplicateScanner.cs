using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VideoDuplicateFinder {
    public class VideoDuplicateScanner {
        public string Folder { get; set; }
        public int Buffer { get; set; } = 2;

        public VideoDuplicateScanner(string folder) {
            Folder = folder;
        }

        public IList<VideoDuplicate> Scan() {
            return ScanVideos(Directory.GetFiles(Folder));
        }

        private IList<VideoDuplicate> ScanVideos(string[] allVideos) {
            HashSet<string> videosTaken = new HashSet<string>();

            var output = new List<VideoDuplicate>();

            FileComparer comparer = new FileComparer();
            comparer.LoadBitmapCache(allVideos);

            for(int i = 0; i <  allVideos.Length; i++) {
                string main = allVideos[i];

                if (!VideoHelper.VideoExtensions.Contains(Path.GetExtension(main))) continue;
                if (videosTaken.Contains(main)) continue;

                HashSet<string> duplicates = new HashSet<string>();

                for (int j = 0; j < allVideos.Length; j += Buffer) {
                    string[] videos = allVideos.CreateSubset(j, j + Buffer > allVideos.Length ? allVideos.Length - j : Buffer);

                    bool[] results = VideoHelper.CompareVideos(main, videos, videosTaken, comparer);
                    for (int k = 0; k < results.Length; k++) {
                        if (!results[k]) continue;

                        duplicates.Add(videos[k]);
                        videosTaken.Add(videos[k]);
                    }
                }

                if (duplicates.Count > 0) {
                    duplicates.Add(main);
                    output.Add(new VideoDuplicate(duplicates));
                }

                videosTaken.Add(main);
            }

            comparer.Dispose();

            return output;
        }
    }
}
