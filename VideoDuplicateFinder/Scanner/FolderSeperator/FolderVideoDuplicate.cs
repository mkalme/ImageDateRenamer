using System;
using System.Collections.Generic;

namespace VideoDuplicateFinder {
    public class FolderVideoDuplicate {
        public string MainVideo { get; set; }
        public HashSet<string> Duplicates { get; set; }

        public FolderVideoDuplicate(string mainVideo, HashSet<string> duplicates) {
            MainVideo = mainVideo;
            Duplicates = duplicates;
        }
    }
}
