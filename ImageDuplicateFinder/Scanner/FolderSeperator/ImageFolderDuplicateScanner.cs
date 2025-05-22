using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageDuplicateFinder {
    public class ImageFolderDuplicateScanner {
        public string[] MainImages { get; set; }
        public string[] Images { get; set; }

        public static readonly int Buffer = 2;

        private static string _mainPath = PathHelper.GetRandomTempPath();
        private static string _path = PathHelper.GetRandomTempPath();

        public IImageController Controller { get; set; }
        public IImageController ResizeController { get; set; }

        public ImageFolderDuplicateScanner(string mainFolder, string folder) {
            MainImages = Directory.GetFiles(mainFolder);
            Images = Directory.GetFiles(folder);

            Controller = new ImageController();
            ResizeController = new ResizedImageController();
        }
        public ImageFolderDuplicateScanner(string[] mainImages, string[] images) {
            MainImages = mainImages;
            Images = images;

            Controller = new ImageController();
            ResizeController = new ResizedImageController();
        }

        public IList<FolderImageDuplicate> Scan() {
            Resize.ResizeImages(MainImages, _mainPath, 512);
            Resize.ResizeImages(Images, _path, 512);

            var mainImagesDictionary = GetPathDictionary(MainImages);
            var imagesDictionary = GetPathDictionary(Images);

            IList<FolderImageDuplicate> resizedDuplicates = ScanImages(Directory.GetFiles(_mainPath), Directory.GetFiles(_path), ResizeController);
            ResizeController.Dispose();

            var output = new List<FolderImageDuplicate>();
            IList<FolderImageDuplicate>[] duplicates = new IList<FolderImageDuplicate>[resizedDuplicates.Count];

            ParallelHelper.For(resizedDuplicates, 5, i => {
                string[] realPaths = new string[resizedDuplicates[i].Duplicates.Count];

                for (int j = 0; j < resizedDuplicates[i].Duplicates.Count; j++) {
                    realPaths[j] = GetRealPath(resizedDuplicates[i].Duplicates.ElementAt(j), imagesDictionary);
                }

                using (ImageController imageController = new ImageController()) {
                    duplicates[i] = ScanImages(new string[] { GetRealPath(resizedDuplicates[i].MainImage, mainImagesDictionary) }, realPaths, imageController);
                }
            });

            for (int i = 0; i < duplicates.Length; i++) {
                output.AddRange(duplicates[i]);
            }

            Directory.Delete(_mainPath, true);
            Directory.Delete(_path, true);

            return output;
        }

        private static string GetRealPath(string path, IDictionary<string, string> dictionary) { 
            return dictionary[Path.GetFileName(path)];
        }
        public static IList<FolderImageDuplicate> ScanImages(string[] mainImages, string[] otherImages, IImageController controller) {
            HashSet<string> imagesTaken = new HashSet<string>();

            var output = new List<FolderImageDuplicate>();

            foreach (var main in mainImages) {
                if (!ImageHelper.ImageExtensions.Contains(Path.GetExtension(main))) continue;

                HashSet<string> duplicates = new HashSet<string>();

                LockedBitmap mainImage = controller.GetBitmap(main);
                for (int i = 0; i < otherImages.Length; i += Buffer) {
                    string[] images = otherImages.CreateSubset(i, i + Buffer > otherImages.Length ? otherImages.Length - i : Buffer);

                    bool[] results = ImageHelper.CompareImages(mainImage, images, controller, imagesTaken);
                    for (int k = 0; k < results.Length; k++) {
                        if (!results[k]) continue;

                        duplicates.Add(images[k]);
                        imagesTaken.Add(images[k]);
                    }
                }

                if (duplicates.Count > 0) output.Add(new FolderImageDuplicate(main, duplicates));

                imagesTaken.Add(main);
            }

            return output;
        }
        private static IDictionary<string, string> GetPathDictionary(string[] files) {
            Dictionary<string, string> output = new Dictionary<string, string>(files.Length);

            foreach (var file in files) {
                output.Add(Path.GetFileName(file), file);
            }

            return output;
        }
    }
}
