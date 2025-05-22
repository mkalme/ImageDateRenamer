using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageDateRenamer {
    public static class ParallelHelper {
        public static void For<T>(IList<T> source, int buffer, Action<int> action) {
            for (int i = 0; i < source.Count; i += buffer) {
                int to = i + buffer;
                if (to > source.Count) to = source.Count;

                Parallel.For(i, to, action);
            }
        }
    }
}
