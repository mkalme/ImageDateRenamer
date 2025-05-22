using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VideoDuplicateFinder {
    public static class VideoHelper {
        public static readonly IReadOnlySet<string> VideoExtensions = new HashSet<string>(new string[] { ".gif", ".mp4", ".3gp", ".avi", ".mkv" });

        public static bool[] CompareVideos(string main, string[] videos, HashSet<string> videosTaken, FileComparer comparer) {
            bool[] duplicates = new bool[videos.Length];

            for (int i = 0; i < videos.Length; i++) {
                if (!VideoExtensions.Contains(Path.GetExtension(videos[i])) || main == videos[i]) continue;
                if (videosTaken.Contains(videos[i])) continue;

                duplicates[i] = comparer.Compare(main, videos[i]);
            }

            return duplicates;
        }
    }
}
