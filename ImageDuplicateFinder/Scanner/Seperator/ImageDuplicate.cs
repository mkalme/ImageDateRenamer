using System;
using System.Collections.Generic;

namespace ImageDuplicateFinder {
    public class ImageDuplicate {
        public HashSet<string> Images { get; set; }

        public ImageDuplicate(HashSet<string> images) {
            Images = images;
        }
    }
}
