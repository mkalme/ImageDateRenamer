using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDateRenamer {
    public class DateRenamer {
        public string Folder { get; set; }

        public DateRenamer(string folder) {
            Folder = folder;
        }

        public void Rename() {
            string[] files = Directory.GetFiles(Folder, "*.*", SearchOption.AllDirectories);
            IDictionary<string, DateTime> creationDates = GetCreationDates(files);

            NameController nameController = new NameController();

            int count = 0;
            foreach (var file in files) {
                string name = Path.GetFileNameWithoutExtension(file);
                string newName = name;

                if (!ImageHelper.IsNameValid(name) && creationDates.TryGetValue(file, out DateTime creationDate)) {
                    newName = "unknown_date";

                    if (creationDate > DateTime.MinValue) newName = creationDate.ToString(ImageHelper.Format);
                }

                newName = nameController.CreateUniqueName(newName);

                string newFile = $"{Path.GetDirectoryName(file)}\\{newName}{Path.GetExtension(file).ToLower()}";
                if (!File.Exists(newFile)) {
                    File.Move(file, newFile);
                }

                Interlocked.Increment(ref count);
                Console.WriteLine($"Moving files. At: {count}/{files.Length}");
            }
        }

        private static IDictionary<string, DateTime> GetCreationDates(string[] files) {
            ConcurrentDictionary<string, DateTime> dates = new ConcurrentDictionary<string, DateTime>();

            int count = 0;
            foreach (var file in files) {
                if (ImageHelper.IsFileAnImage(file) && !ImageHelper.IsNameValid(Path.GetFileName(file))) {
                    dates.TryAdd(file, ImageHelper.GetCreationDate(file));
                }

                count++;
                Console.WriteLine($"{count}/{files.Length}");
            }

            return dates;
        }
    }
}
