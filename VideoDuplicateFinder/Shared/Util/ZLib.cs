using System;
using System.IO;
using ZLibNet;

namespace VideoDuplicateFinder {
    public static class ZLib {
        public static MemoryStream Compress(byte[] input) {
            var mso = new MemoryStream();
            using (var msi = new MemoryStream(input))
            using (var zs = new ZLibStream(mso, CompressionMode.Compress, CompressionLevel.Default, true)) {
                msi.CopyTo(zs);

                return mso;
            }
        }
    }
}
