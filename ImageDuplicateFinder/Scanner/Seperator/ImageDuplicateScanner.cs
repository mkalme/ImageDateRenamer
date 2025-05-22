using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageDuplicateFinder {
    public class ImageDuplicateScanner {
        public string Folder { get; set; }
        public int Buffer { get; set; } = 2;

        public IImageController Controller { get; set; }
        public IImageController ResizeController { get; set; }

        private static string _path = PathHelper.GetRandomTempPath();

        public ImageDuplicateScanner(string folder) {
            Folder = folder;

            Controller = new ImageController();
            ResizeController = new ResizedImageController();
        }

        public IList<ImageDuplicate> Scan() {
            Resize.ResizeImages(Directory.GetFiles(Folder), _path, 512);

            IList<ImageDuplicate> resizedDuplicates = ScanImages(Directory.GetFiles(_path), ResizeController);
            ResizeController.Dispose();

            var output = new List<ImageDuplicate>();
            IList<ImageDuplicate>[] duplicates = new IList<ImageDuplicate>[resizedDuplicates.Count];

            ParallelHelper.For(resizedDuplicates, 5, i => {
                string[] realPaths = new string[resizedDuplicates[i].Images.Count];

                for (int j = 0; j < resizedDuplicates[i].Images.Count; j++) {
                    realPaths[j] = $"{Folder}\\{Path.GetFileName(resizedDuplicates[i].Images.ElementAt(j))}";
                }

                using (ImageController imageController = new ImageController()) {
                    duplicates[i] = ScanImages(realPaths, imageController);
                }
            });

            for (int i = 0; i < duplicates.Length; i++) {
                output.AddRange(duplicates[i]);
            }

            Directory.Delete(_path, true);

            return output;
        }

        private IList<ImageDuplicate> ScanImages(string[] allImages, IImageController controller) {
            HashSet<string> imagesTaken = new HashSet<string>();

            var output = new List<ImageDuplicate>();

            for (int i = 0; i < allImages.Length; i++) {
                string main = allImages[i];

                if (!ImageHelper.ImageExtensions.Contains(Path.GetExtension(main))) continue;
                if (imagesTaken.Contains(main)) continue;

                HashSet<string> duplicates = new HashSet<string>();

                LockedBitmap mainImage = controller.GetBitmap(main);
                for (int j = 0; j < allImages.Length; j += Buffer) {
                    string[] images = allImages.CreateSubset(j, j + Buffer > allImages.Length ? allImages.Length - j : Buffer);

                    bool[] results = ImageHelper.CompareImages(mainImage, images, controller, imagesTaken);
                    for (int k = 0; k < results.Length; k++) {
                        if (!results[k]) continue;

                        duplicates.Add(images[k]);
                        imagesTaken.Add(images[k]);
                    }
                }

                if (duplicates.Count > 0) {
                    duplicates.Add(main);
                    output.Add(new ImageDuplicate(duplicates));
                }

                imagesTaken.Add(main);
            }

            return output;
        }
    }
}
