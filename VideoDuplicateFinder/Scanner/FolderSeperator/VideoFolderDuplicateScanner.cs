using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideoDuplicateFinder {
    public class VideoFolderDuplicateScanner {
        public string[] MainVideos { get; set; }
        public string[] Vidoes { get; set; }

        public static readonly int Buffer = 2;

        public VideoFolderDuplicateScanner(string mainFolder, string folder) {
            MainVideos = Directory.GetFiles(mainFolder);
            Vidoes = Directory.GetFiles(folder);
        }
        public VideoFolderDuplicateScanner(string[] mainVideos, string[] videos) {
            MainVideos = mainVideos;
            Vidoes = videos;
        }

        public IList<FolderVideoDuplicate> Scan() {
            return ScanVideos(MainVideos, Vidoes);
        }

        private IList<FolderVideoDuplicate> ScanVideos(string[] mainVideos, string[] otherVideos) {
            HashSet<string> videosTaken = new HashSet<string>();

            var output = new List<FolderVideoDuplicate>();

            int count = 0;

            FileComparer comparer = new FileComparer();
            comparer.LoadBitmapCache(mainVideos);
            comparer.LoadBitmapCache(otherVideos);

            Parallel.ForEach(mainVideos, main => {
                if (!VideoHelper.VideoExtensions.Contains(Path.GetExtension(main))) return;

                HashSet<string> duplicates = new HashSet<string>();

                for (int i = 0; i < otherVideos.Length; i += Buffer) {
                    string[] videos = otherVideos.CreateSubset(i, i + Buffer > otherVideos.Length ? otherVideos.Length - i : Buffer);

                    bool[] results = VideoHelper.CompareVideos(main, videos, videosTaken, comparer);
                    for (int k = 0; k < results.Length; k++) {
                        if (!results[k]) continue;

                        duplicates.Add(videos[k]);
                        videosTaken.Add(videos[k]);
                    }
                }

                if (duplicates.Count > 0) output.Add(new FolderVideoDuplicate(main, duplicates));

                videosTaken.Add(main);

                Interlocked.Increment(ref count);
                Console.WriteLine($"{count}/{mainVideos.Length}");
            });

            comparer.Dispose();

            return output;
        }
    }
}
