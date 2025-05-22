using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDateRenamer {
    public class CameraFinder {
        public IList<string> FindPhotosTakenBySomeCamera(string folder, string camera) {
            string[] allFiles = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            string[] allCameras = new string[allFiles.Length];

            int count = 0;
            Parallel.For(0, allFiles.Length, i => {
                if (!ImageHelper.IsFileAnImage(allFiles[i])) return;

                allCameras[i] = ImageHelper.GetCamera(allFiles[i]);

                Interlocked.Increment(ref count);
                Console.WriteLine($"{count}/{allFiles.Length}");
            });

            List<string> output = new List<string>();
            for (int i = 0; i < allCameras.Length; i++) {
                if (allCameras[i] == null) continue;
                if (allCameras[i] == camera) output.Add(allFiles[i]);
            }

            return output;
        }
    }
}
