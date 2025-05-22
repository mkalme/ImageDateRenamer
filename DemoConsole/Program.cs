using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImageDateRenamer;
using ImageDuplicateFinder;
using VideoDuplicateFinder;

namespace DemoConsole {
    class Program {
        private static readonly string OriginalFolder = "D:OldArtursPC";
        private static readonly string MediaFolder = "D:NewBackup\\Family\\Media";
        
        private static readonly string SeperatedDuplicateImages = "D:NewBackup\\Family\\SeperatedImages";
        private static readonly string SeperatedPhoneImages = "D:NewBackup\\Family\\PhoneSeperatedImages";
        private static readonly string TotalSeperatedPhoneImages = "D:NewBackup\\Family\\PhoneTotalSeperatedImages";

        private static readonly string SeperatedDuplicateVideos = "D:NewBackup\\Family\\SeperatedVideos";
        private static readonly string SeperatedPhoneVideos = "D:NewBackup\\Family\\PhoneSeperatedVideos";
        private static readonly string TotalSeperatedPhoneVideos = "D:NewBackup\\Family\\PhoneTotalSeperatedVideos";

        private static readonly string BackupFolder = "D:\\Backup\\Pictures & Videos on Phone";

        [STAThread]
        static void Main(string[] args) {
            //RenameMedia(OriginalFolder);
            //SeperateMediaMonthly(OriginalFolder, MediaFolder);

            //SeperateDuplicateImages(MediaFolder, SeperatedDuplicateImages);
            //SeperateImagesFromPhone(BackupFolder, MediaFolder, SeperatedPhoneImages);
            //SeperateTotalDuplicateImagesFromPhone(BackupFolder, MediaFolder, TotalSeperatedPhoneImages);

            //SeperateDuplicateVideos(MediaFolder, SeperatedDuplicateVideos);
            //SeperateVideosFromPhone(BackupFolder, MediaFolder, SeperatedPhoneVideos);
            //SeperateTotalDuplicateVideosFromPhone(BackupFolder, MediaFolder, TotalSeperatedPhoneVideos);

            ReverseDates(MediaFolder, $"D:\\imageReverseDates.txt");

            Console.WriteLine("Done");

            Console.ReadLine();
        }

        private static void RenameMedia(string folder) {
            DateRenamer renamer = new DateRenamer(folder);
            renamer.Rename();
        }
        private static void SeperateMediaMonthly(string folder, string destinationFolder) {
            MonthlySeperation.SeperateImages(folder, destinationFolder);
        }

        private static void SeperateDuplicateImages(string folder, string destinationFolder) {
            DuplicateImageSeperator seperator = new DuplicateImageSeperator(Directory.GetDirectories(folder));
            seperator.Seperate(destinationFolder);
        }
        private static void SeperateImagesFromPhone(string mainFolder, string folder, string destinationFolder) {
            DuplicateFolderImageSeperator folderSeperator = new DuplicateFolderImageSeperator(mainFolder, folder);
            folderSeperator.Seperate(destinationFolder);
        }
        private static void SeperateTotalDuplicateImagesFromPhone(string mainFolder, string folder, string destinationFolder) {
            TotalFolderImageSeperator folderSeperator = new TotalFolderImageSeperator(mainFolder, folder);
            folderSeperator.Seperate(destinationFolder);
        }

        private static void SeperateDuplicateVideos(string folder, string destinationFolder) {
            DuplicateVideoSeperator seperator = new DuplicateVideoSeperator(Directory.GetDirectories(folder));
            seperator.Seperate(destinationFolder);
        }
        private static void SeperateVideosFromPhone(string mainFolder, string folder, string destinationFolder) {
            DuplicateFolderVideoSeperator folderSeperator = new DuplicateFolderVideoSeperator(mainFolder, folder);
            folderSeperator.Seperate(destinationFolder);
        }
        private static void SeperateTotalDuplicateVideosFromPhone(string mainFolder, string folder, string destinationFolder) {
            TotalFolderVideoSeperator folderSeperator = new TotalFolderVideoSeperator(mainFolder, folder);
            folderSeperator.Seperate(destinationFolder);
        }

        private static void GetImagesByCamera(string folder, string camera) {
            CameraFinder cameraFinder = new CameraFinder();

            IList<string> images = cameraFinder.FindPhotosTakenBySomeCamera(MediaFolder, camera);
            Console.WriteLine(string.Join("\n", ImageDateRenamer.ImageHelper.GetInvalidDates(images)));
        }
        private static void ReverseDates(string folder, string file) {
            string[] allFiles = File.ReadAllLines(file);
            string[] newFiles = new string[allFiles.Length];

            NameController nameController = new NameController();

            for (int i = 0; i < allFiles.Length; i++) {
                string thisFile = allFiles[i];
                if (!File.Exists(thisFile)) continue;

                nameController.AddName(thisFile);

                DateTime date = ImageDateRenamer.ImageHelper.ParseDateTimeFromFileName(Path.GetFileName(thisFile));
                date = new DateTime(date.Year, date.Day, date.Month, date.Hour, date.Minute, date.Second, date.Millisecond);

                newFiles[i] = $"{Path.GetDirectoryName(thisFile)}\\{date.ToString(ImageDateRenamer.ImageHelper.Format)}";
                newFiles[i] = $"{nameController.CreateUniqueName(newFiles[i])}{Path.GetExtension(thisFile)}";

                File.Move(thisFile, newFiles[i]);
                MonthlySeperation.SeperateFile(newFiles[i], folder);
            }
        }

        private static IEnumerable<string> GetUniqueFileExtensions(string folder) {
            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);

            return new HashSet<string>(files.Select(x => Path.GetExtension(x).ToLower()));
        }
    }
}
