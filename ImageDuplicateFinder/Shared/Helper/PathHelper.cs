using System;
using System.IO;

namespace ImageDuplicateFinder {
    public static class PathHelper {
        public static string GetRandomTempPath() { 
            return $"{Path.GetTempPath()}\\{Path.GetRandomFileName()}";
        }
    }
}
