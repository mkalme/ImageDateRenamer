using System;
using System.Collections.Generic;

namespace VideoDuplicateFinder {
    public class VideoDuplicate {
        public HashSet<string> Videos { get; set; }

        public VideoDuplicate(HashSet<string> videos) {
            Videos = videos;
        }
    }
}
