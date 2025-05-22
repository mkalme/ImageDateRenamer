using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageDateRenamer {
    public class NameController {
        private Dictionary<string, int> _uniqueNames = new Dictionary<string, int>();

        public void AddName(string name) {
            var parsedName = ParseName(name);

            if (_uniqueNames.ContainsKey(parsedName.Key)) {
                _uniqueNames[parsedName.Key]++;
            } else {
                _uniqueNames.Add(parsedName.Key, 1);
            }
        }
        public string CreateUniqueName(string name) {
            var parsedName = ParseName(name);

            if (_uniqueNames.ContainsKey(parsedName.Key)) {
                _uniqueNames[parsedName.Key]++;

                name = $"{name}({_uniqueNames[parsedName.Key]})";
            } else {
                _uniqueNames.Add(parsedName.Key, 1);
                name = parsedName.Key;
            }

            return name;
        }


        private static KeyValuePair<string, int> ParseName(string name) {
            var defaultPair = new KeyValuePair<string, int>(name, 1);

            if (name.Length < 3 || name.Last() != ')') return defaultPair;

            bool lastCharValid = false;

            int index = name.Length - 2;
            int value = 0, pow = 1;

            for (int i = index; i > -1; i--) {
                int difference = name[i] - '0';
                index--;

                if (difference < 0 || difference > 9) {
                    if (name[i] == '-') value = -value;
                    else index++;

                    break;
                } else {
                    lastCharValid = true;

                    value += difference * pow;
                    pow *= 10;
                }
            }

            if (!lastCharValid || name[index] != '(') return defaultPair;

            return new KeyValuePair<string, int>(name.Substring(0, index), value);
        }
    }
}
