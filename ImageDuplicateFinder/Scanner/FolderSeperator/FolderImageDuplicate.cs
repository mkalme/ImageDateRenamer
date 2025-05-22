using System;
using System.Collections.Generic;

namespace ImageDuplicateFinder {
    public class FolderImageDuplicate {
        public string MainImage { get; set; }
        public HashSet<string> Duplicates { get; set; }

        public FolderImageDuplicate(string mainImage, HashSet<string> duplicates) {
            MainImage = mainImage;
            Duplicates = duplicates;
        }
    }
}
