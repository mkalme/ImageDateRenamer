using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MetadataExtractor;
using System.Text.RegularExpressions;
using System.Linq;

namespace ImageDateRenamer {
    public static class ImageHelper {
        public static readonly string Format = "yyyyMMdd_HHmmss";

        private static readonly Regex _r = new Regex(":");
        private static readonly int[] _exifData = new int[] { 306, 36867 };

        public static readonly IReadOnlySet<string> VideoExtensions = new HashSet<string>(new string[] { ".gif", ".mp4", ".3gp", ".avi", ".mkv" });
        public static readonly IReadOnlySet<string> ImageExtensions = new HashSet<string>(new string[] { ".jpg", ".tif", ".bmp", ".png", ".jpeg" });

        public static DateTime GetCreationDate(string image) {
            bool propertyIsValid = false;
            bool isAnException = false;

            DateTime creationDate = DateTime.MinValue;

            try {
                propertyIsValid = GetImageCreationDate(image, out creationDate) && IsDateValid(creationDate);
            } catch {
                isAnException = true;
            }

            if (!propertyIsValid) {
                creationDate = File.GetLastWriteTime(image);

                if (!IsDateValid(creationDate)) {
                    creationDate = DateTime.MinValue;
                }
            }

            if (isAnException) Console.WriteLine($"{image}\t{creationDate}");

            return creationDate;
        }
        public static string GetCamera(string image) {
            HashSet<string> cameras = new HashSet<string>();

            try {
                IReadOnlyList<MetadataExtractor.Directory> metadata = ImageMetadataReader.ReadMetadata(image);

                foreach (var data in metadata) {
                    string camera = data.GetString(272);

                    if (camera != null) cameras.Add(camera);
                }
            } catch {}

            if (cameras.Count < 1) return string.Empty;
            return cameras.First();
        }
        public static bool IsDateValid(DateTime dateTime) {
            return dateTime.Year > 2004 && (dateTime.Year < 2021) && ((dateTime - new DateTime(dateTime.Year, dateTime.Month, dateTime.Day)).Ticks != 0);
        }
        public static bool IsNameValid(string name) {
            if (name.Length > Format.Length) name = name.Substring(0, Format.Length);

            return DateTime.TryParseExact(name, Format, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime time);
        }
        public static bool IsFileAnImage(string file) {
            string extension = Path.GetExtension(file).ToLower();

            if (VideoExtensions.Contains(extension) || ImageExtensions.Contains(extension)) return true;
            return false;
        }
        public static DateTime ParseDateTimeFromFileName(string name) {
            if (name.Length > Format.Length) name = name.Substring(0, Format.Length);

            return DateTime.ParseExact(name, Format, new CultureInfo("en-US"), DateTimeStyles.None);
        }

        public static IList<string> GetInvalidDates(IList<string> images) {
            List<string> output = new List<string>();

            foreach (var image in images) {
                if (!IsNameValid(Path.GetFileName(image))) continue;

                output.Add(image);
            }

            return output;
        }

        private static bool GetImageCreationDate(string image, out DateTime time) {
            IReadOnlyList<MetadataExtractor.Directory> metadata = ImageMetadataReader.ReadMetadata(image);

            time = DateTime.MaxValue;

            List<DateTime> times = new List<DateTime>(16);

            foreach (var data in metadata) {
                foreach (int exif in _exifData) {
                    data.TryGetDateTime(exif, out DateTime thisTime);

                    times.Add(thisTime);
                }
            }

            times.AddRange(ShellInfo.GetMediaProperties(image));

            foreach (var thisTime in times) {
                if (IsDateValid(thisTime)) {
                    if (thisTime < time) time = thisTime;
                }
            }

            return time != DateTime.MaxValue;
        }
    }
}
