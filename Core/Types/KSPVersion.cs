using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CKAN
{
    [JsonConverter(typeof (JsonSimpleStringConverter))]
    public class KSPVersion : IComparable<KSPVersion>,IEquatable<KSPVersion>
    {
        private readonly string version;
        private Version cached_version_object;
        private readonly bool is_short;

        private static readonly Dictionary<string, Tuple<string, bool>> NormalCache =
            new Dictionary<string, Tuple<string, bool>>();

        public KSPVersion(string v)
        {
            Tuple<string, bool> normalized;

            if (v != null && NormalCache.TryGetValue(v, out normalized))
            {
                version = normalized.Item1;
                is_short = version != null && normalized.Item2;
            }
            else
            {
                version = Normalise(v);
                version = AnyToNull(version);
                is_short = version != null && Regex.IsMatch(version, @"^\d+\.\d+$");
                Validate(); // Throws on error.
                if (v != null)
                {
                    NormalCache.Add(v, new Tuple<string, bool>(version, is_short));
                }
            }
        }

        // Casting function
        public static explicit operator KSPVersion(string v)
        {
            return new KSPVersion(v);
        }

        /// <summary>
        /// Normalises a version number. Currently this adds
        /// a leading zero if it's missing one.
        /// </summary>
        /// <param name="v">V.</param>
        private static string Normalise(string v)
        {
            if (v == null)
            {
                return null;
            }

            if (Regex.IsMatch(v, @"^\."))
            {
                return "0" + v;
            }

            return v;
        }

        /// <summary>
        /// If our KSP version is already long (x.y.z) returns itself.  If our
        /// KSP version is short (x.y) returns the minimum full release that
        /// would match (x.y.0).
        ///
        /// It is a mistake to call this method without using its return value.
        /// </summary>
        public KSPVersion ToLongMin()
        {
            return IsShortVersion() ? new KSPVersion(version + ".0") : this;
        }

        /// <summary>
        /// If our KSP version is already long (x.y.z) returns itself. If our
        /// KSP version is short (x.y) returns the maximum full release that
        /// would match (x.y.99).
        ///
        /// It is a mistake to call this method without using its return value.
        /// </summary>
        public KSPVersion ToLongMax()
        {
            return IsShortVersion() ? new KSPVersion(version + ".99") : this;
        }

        /// <summary>
        /// Returns true if our version object is short (ie: missing a patch number,
        /// such as `0.23`).
        /// </summary>
        public bool IsShortVersion()
        {
            return is_short;
        }

        /// <summary>
        /// Returns true if our version is long (ie: includes a patch number, such
        /// as `0.23.5`).
        /// </summary>
        public bool IsLongVersion()
        {
            return !is_short;
        }

        public bool IsAny()
        {
            return version == null;
        }

        public bool IsNotAny()
        {
            return !IsAny();
        }

        public string Version()
        {
            return version;
        }

        // Private for now, since we can't guarnatee public code will only call
        // us with long versions.
        private Version VersionObject()
        {
            return cached_version_object ?? (cached_version_object = new Version(version));
        }

        private static readonly Dictionary<Tuple<KSPVersion, KSPVersion>, int> CompareCache
            = new Dictionary<Tuple<KSPVersion, KSPVersion>, int>();


        public int CompareTo(KSPVersion that)
        {
            if ((object)that == null)
                return 1;
            int ret;
            var tuple = new Tuple<KSPVersion, KSPVersion>(this, that);
            if (CompareCache.TryGetValue(tuple, out ret))
                return ret;


            // We need two long versions to be able to compare properly.
            if ((!IsLongVersion()) && (!that.IsLongVersion()))
            {
                throw new KSPVersionIncomparableException(this, that, "CompareTo");
            }

            // Hooray, we can hook the regular Version code here.

            Version v1 = VersionObject();
            Version v2 = that.VersionObject();
            ret = v1.CompareTo(v2);
            CompareCache.Add(tuple, ret);
            return ret;
        }

        // Returns true if this targets that version of KSP.
        // That must be a long (actual) version.
        // Eg: 0.25 targets 0.25.2

        public bool Targets(KSPVersion that)
        {
            // Cry if we're not looking at a long version to compare to.
            if (!that.IsLongVersion())
            {
                throw new KSPVersionIncomparableException(this, that, "Targets");
            }

            // If we target any, then yes, it's a match.
            if (IsAny())
            {
                return true;
            }
            else if (IsLongVersion())
            {
                return IsEqualTo(that);
            }

            // We've got a short version, so split it into two separate versions,
            // and compare each.

            KSPVersion min = ToLongMin();

            KSPVersion max = ToLongMax();

            return (that >= min && that <= max);
        }

        // "any" -> null
        private static string AnyToNull(string v)
        {
            if (v != null && v == "any")
            {
                return null;
            }
            return v;
        }

        // Throws on error.
        private void Validate()
        {
            if (version == null || IsShortVersion() || IsLongVersion())
            {
                return;
            }
            throw new BadKSPVersionException(version);
        }

        public override string ToString ()
        {
            return Version();
        }

        public override int GetHashCode()
        {
            return (version != null ? version.GetHashCode() : 0);
        }

        public static int CompareTo(KSPVersion a, KSPVersion b)
        {
            if((object) a == null)
                return ((object)b == null) ? 0 : -1;
            if ((object) b == null)
                return 1;
            return a.CompareTo(b);
        }

        public bool Equals(KSPVersion x)
        {
            if ((object) x == null) return false;
            return version.Equals(x.version);
        }
        public static bool operator <(KSPVersion x, KSPVersion y) { return CompareTo(x, y) < 0; }
        public static bool operator >(KSPVersion x, KSPVersion y) { return CompareTo(x, y) > 0; }
        public static bool operator <=(KSPVersion x, KSPVersion y) { return CompareTo(x, y) <= 0; }
        public static bool operator >=(KSPVersion x, KSPVersion y) { return CompareTo(x, y) >= 0; }
        public static bool operator ==(KSPVersion x, KSPVersion y) { return CompareTo(x, y) == 0; }
        public static bool operator !=(KSPVersion x, KSPVersion y) { return CompareTo(x, y) != 0; }
        public override bool Equals(object obj)
        {
            return (obj is KSPVersion) && Equals((KSPVersion)obj);
        }

        public bool IsGreaterThan(KSPVersion other) { return this > other; }
        public bool IsLessThan(KSPVersion other) { return this < other; }
        public bool IsEqualTo(KSPVersion other) { return this == other; }
    }

    public class BadKSPVersionException : Exception
    {
        private readonly string version;

        public BadKSPVersionException(string v)
        {
            version = v;
        }

        public override string ToString()
        {
            return string.Format("[BadKSPVersionException] {0} is not a valid KSP version", version);
        }
    }

    public class KSPVersionIncomparableException : Exception
    {
        private readonly string version1;
        private readonly string version2;
        private readonly string method;

        public KSPVersionIncomparableException(KSPVersion v1, KSPVersion v2, string m)
        {
            version1 = v1.Version();
            version2 = v2.Version();
            method = m;
        }

        public override string ToString()
        {
            return string.Format("[KSPVersionIncomparableException] {0} and {1} cannot be compared by {2}", version1,
                version2, method);
        }
    }
}
