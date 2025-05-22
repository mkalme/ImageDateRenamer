using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace VideoDuplicateFinder {
    public class FileComparer : IDisposable {
        public int Buffer { get; set; } = 1024 * 1024 * 100;

        private ConcurrentDictionary<string, LockedBitmap> _bitmapCache = new ConcurrentDictionary<string, LockedBitmap>();
        private ConcurrentDictionary<string, TimeSpan> _durationCache = new ConcurrentDictionary<string, TimeSpan>();
        private bool disposedValue;

        public FileComparer() {
            FFmpeg.SetExecutablesPath($"C:\\ffmpeg\\bin");
        }

        public bool Compare(string left, string right) {
            if (left == right) return false;

            try {
                return CompareSimilarities(left, right);
            } catch {
                return false;
            }

            //bool equals = true;

            //try {
                //using (FileStream leftFile = File.Open(left, FileMode.Open))
                //using (FileStream rightFile = File.Open(right, FileMode.Open)) {
                //    if (leftFile.Length != rightFile.Length) equals = false;

                //    while (equals) {
                //        byte[] leftArray = new byte[Math.Min(Buffer, leftFile.Length - leftFile.Position)];
                //        byte[] rightArray = new byte[leftArray.Length];

                //        leftFile.Read(leftArray, 0, leftArray.Length);
                //        rightFile.Read(rightArray, 0, rightArray.Length);

                //        if (!leftArray.Compare(rightArray)) equals = false;

                //        if (leftFile.Position >= leftFile.Length) break;
                //    }

                //    leftFile.Close();
                //    rightFile.Close();
                //}

            //    if (!equals) return CompareSimilarities(left, right);
            //} catch { 
            
            //}

            //return equals;
        }
        public bool CompareSimilarities(string left, string right) {
            if (GetVideoDuration(left) != GetVideoDuration(right)) return false;

            LockedBitmap leftThumbnail = GetThumbnail(left);
            LockedBitmap rightThumbnail = GetThumbnail(right);

            return leftThumbnail.RGBValues.Compare(rightThumbnail.RGBValues);
        }

        public TimeSpan GetVideoDuration(string video) {
            TimeSpan span;

            if (!_durationCache.TryGetValue(video, out span)) {
                Task<IMediaInfo> mediaInfo = FFmpeg.GetMediaInfo(video);

                span = mediaInfo.Result.VideoStreams.First().Duration;

                _durationCache.TryAdd(video, span);
            }

            return span;
        }
        public LockedBitmap GetThumbnail(string video) {
            LockedBitmap thumbnail;
            if (!_bitmapCache.TryGetValue(video, out thumbnail)) {
                string output = $"{Path.GetTempPath()}\\{Path.GetRandomFileName()}_thumbnail.png";

                IConversion conversion = FFmpeg.Conversions.FromSnippet.Snapshot(video, output, TimeSpan.FromSeconds(0)).Result;
                conversion.Start().Wait();

                using (FileStream file = File.OpenRead(output)) {
                    thumbnail = new LockedBitmap(new Bitmap(Image.FromStream(file)), video);
                }

                File.Delete(output);

                thumbnail.RGBValues = ZLib.Compress(thumbnail.RGBValues).ToArray();

                _bitmapCache.TryAdd(video, thumbnail);
            }

            return thumbnail;
        }

        public void LoadBitmapCache(IEnumerable<string> videos) {
            int count = 0;

            Parallel.ForEach(videos, video => {
                if (!VideoHelper.VideoExtensions.Contains(Path.GetExtension(video))) return;

                if (!_bitmapCache.ContainsKey(video)) {
                    try {
                        GetThumbnail(video);
                    } catch {

                    }
                }

                Interlocked.Increment(ref count);
                Console.WriteLine($"Load cache, at: {count}");
            });
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    _bitmapCache.Clear();
                    _durationCache.Clear();
                }
                disposedValue = true;
            }
        }
    }
}
