using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ImageDuplicateFinder {
    public static class ArrayExtensions {
        public static T[] CreateSubset<T>(this T[] array, int index, int length) {
            T[] output = new T[length];

            for (int i = 0; i < length; i++) {
                output[i] = array[i + index];
            }

            return output;
        }
        public static bool Compare(this byte[] array, byte[] other) {
            if (array.Length != other.Length) return false;

            for (int i = 0; i < array.Length; i++) {
                if (array[i] != other[i]) return false;
            }

            return true;
        }
    }
}
