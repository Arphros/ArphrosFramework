using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// A class that represents the version format of Arphros
    /// </summary>
    [Serializable]
    public class ArphrosVersion {
        public int MainVersion;
        public int SubVersion;
        public VersionType Type;
        public int RevisionNumber;

        public ArphrosVersion(int mainVersion, int subVersion, VersionType type, int revisionNumber) {
            MainVersion = mainVersion;
            SubVersion = subVersion;
            Type = type;
            RevisionNumber = revisionNumber;
        }

        public ArphrosVersion(string versionString) {
            string pattern = @"^(\d+)\.(\d+)([abufdx])?(\d*)$";
            Regex regex = new(pattern);

            Match match = regex.Match(versionString);

            if (!match.Success) {
                throw new ArgumentException("Invalid version string format.");
            }

            MainVersion = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            SubVersion = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

            if (match.Groups[3].Success) {
                Type = GetTypeFromChar(match.Groups[3].Value[0]);
            }
            else {
                Type = VersionType.Release; // Default to Release (stable)
            }

            if (match.Groups[4].Success && !string.IsNullOrEmpty(match.Groups[4].Value)) {
                RevisionNumber = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
            }
            else {
                RevisionNumber = 1; // Default revision number is 1
            }
        }

        private VersionType GetTypeFromChar(char typeChar) {
            switch (typeChar) {
                case 'b': return VersionType.Beta;
                case 'f': return VersionType.Release;
                case 'a': return VersionType.Alpha;
                case 'd': return VersionType.Developer;
                case 'x': return VersionType.Admin;
                case 'u': return VersionType.Unreleased;
                default: throw new ArgumentException($"Invalid version type character: {typeChar}");
            }
        }

        private char GetCharFromType(VersionType type) {
            switch (type) {
                case VersionType.Beta: return 'b';
                case VersionType.Release: return 'f';
                case VersionType.Alpha: return 'a';
                case VersionType.Developer: return 'd';
                case VersionType.Admin: return 'x';
                case VersionType.Unreleased: return 'u';
                default: throw new ArgumentException($"Invalid version type: {type}");
            }
        }

        public static ArphrosVersion Parse(string version) => new ArphrosVersion(version);
        public static ArphrosVersion Current => new ArphrosVersion(Application.version);

        public override string ToString() {
            return $"{MainVersion}.{SubVersion}{GetCharFromType(Type)}{RevisionNumber}";
        }

        public bool IsGreaterOrSame(ArphrosVersion other, bool ignoreType = false) {
            if (!ignoreType && Type != other.Type)
                return false; // Different types are NOT comparable, so return false

            if (MainVersion > other.MainVersion)
                return true;
            if (MainVersion < other.MainVersion)
                return false;

            if (SubVersion > other.SubVersion)
                return true;
            if (SubVersion < other.SubVersion)
                return false;

            return RevisionNumber >= other.RevisionNumber;
        }
    }

    public enum VersionType {
        Beta,
        Release,
        Unreleased,
        Alpha,
        Developer,
        Admin
    }
}