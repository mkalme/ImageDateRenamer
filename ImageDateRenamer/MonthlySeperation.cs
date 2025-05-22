using System;
using System.Globalization;
using System.IO;

namespace ImageDateRenamer {
    public static class MonthlySeperation {
        public static void SeperateImages(string folder, string destination) {
            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++) {
                SeperateFile(files[i], destination);

                Console.WriteLine($"{i + 1}/{files.Length}");
            }
        }
        public static void SeperateFile(string file, string destination) {
            string folderName = "unsorted";

            string date = Path.GetFileNameWithoutExtension(file);
            if (date.Length > ImageHelper.Format.Length) date = date.Substring(0, ImageHelper.Format.Length);

            if (DateTime.TryParseExact(date, ImageHelper.Format, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime time)) {
                folderName = $"{time.Year}-{time.Month.ToString("00")}";
            }

            Directory.CreateDirectory($"{destination}\\{folderName}");

            string fileOutput = $"{destination}\\{folderName}\\{Path.GetFileName(file)}";
            if (!File.Exists(fileOutput)) File.Move(file, fileOutput);
        }
    }
}
