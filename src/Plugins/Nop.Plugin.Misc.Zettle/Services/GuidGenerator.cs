using System;

namespace Nop.Plugin.Misc.Zettle.Services
{
    /// <summary>
    /// Used for generating UUID based on RFC 4122.
    /// </summary>
    /// <remarks>Source: https://github.com/fluentcassandra/fluentcassandra/blob/master/src/GuidGenerator.cs </remarks>
    /// <seealso href="http://www.ietf.org/rfc/rfc4122.txt">RFC 4122 - A Universally Unique IDentifier (UUID) URN Namespace</seealso>
    public static class GuidGenerator
    {
        #region Constants

        // number of bytes in uuid
        private const int BYTE_ARRAY_SIZE = 16;

        // multiplex variant info
        private const int VARIANT_BYTE = 8;
        private const int VARIANT_BYTE_MASK = 0x3f;
        private const int VARIANT_BYTE_SHIFT = 0x80;

        // multiplex version info
        private const int VERSION_BYTE = 7;
        private const int VERSION_BYTE_MASK = 0x0f;
        private const int VERSION_BYTE_SHIFT = 4;

        // indexes within the uuid array for certain boundaries
        private const byte TIMESTAMP_BYTE = 0;
        private const byte GUID_CLOCK_SEQUENCE_BYTE = 8;
        private const byte NODE_BYTE = 10;

        #endregion

        #region Fields

        // offset to move from 1/1/0001, which is 0-time for .NET, to gregorian 0-time of 10/15/1582
        private static readonly DateTimeOffset _gregorianCalendarStart = new(1582, 10, 15, 0, 0, 0, TimeSpan.Zero);

        private static readonly Random _random;
        private static readonly byte[] _nodeBytes;
        private static readonly byte[] _clockSequenceBytes;

        #endregion

        #region Ctor

        static GuidGenerator()
        {
            _random = new Random();
            _nodeBytes = GenerateNodeBytes();
            _clockSequenceBytes = GenerateClockSequenceBytes();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generates a random value for the node.
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateNodeBytes()
        {
            var node = new byte[6];

            _random.NextBytes(node);
            return node;
        }

        /// <summary>
        /// Generates a random clock sequence.
        /// </summary>
        private static byte[] GenerateClockSequenceBytes()
        {
            var bytes = new byte[2];
            _random.NextBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// In order to maintain a constant value we need to get a two byte hash from the DateTime.
        /// </summary>
        private static byte[] GenerateClockSequenceBytes(DateTime dt)
        {
            var utc = dt.ToUniversalTime();
            var bytes = BitConverter.GetBytes(utc.Ticks);

            if (bytes.Length == 0)
                return new byte[] { 0x0, 0x0 };

            if (bytes.Length == 1)
                return new byte[] { 0x0, bytes[0] };

            return new byte[] { bytes[0], bytes[1] };
        }

        private static Guid GenerateTimeBasedGuid(DateTimeOffset dateTime, byte[] clockSequence, byte[] node)
        {
            if (clockSequence == null)
                throw new ArgumentNullException(nameof(clockSequence));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (clockSequence.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(clockSequence), "The clockSequence must be 2 bytes.");

            if (node.Length != 6)
                throw new ArgumentOutOfRangeException(nameof(node), "The node must be 6 bytes.");

            var ticks = (dateTime - _gregorianCalendarStart).Ticks;
            var guid = new byte[BYTE_ARRAY_SIZE];
            var timestamp = BitConverter.GetBytes(ticks);

            // copy node
            Array.Copy(node, 0, guid, NODE_BYTE, Math.Min(6, node.Length));

            // copy clock sequence
            Array.Copy(clockSequence, 0, guid, GUID_CLOCK_SEQUENCE_BYTE, Math.Min(2, clockSequence.Length));

            // copy timestamp
            Array.Copy(timestamp, 0, guid, TIMESTAMP_BYTE, Math.Min(8, timestamp.Length));

            // set the variant
            guid[VARIANT_BYTE] &= (byte)VARIANT_BYTE_MASK;
            guid[VARIANT_BYTE] |= (byte)VARIANT_BYTE_SHIFT;

            // set the version
            guid[VERSION_BYTE] &= (byte)VERSION_BYTE_MASK;
            guid[VERSION_BYTE] |= (byte)((byte)GuidVersion.TimeBased << VERSION_BYTE_SHIFT);

            return new Guid(guid);
        }
        #endregion

        #region Methods

        public static GuidVersion GetUuidVersion(this Guid guid)
        {
            var bytes = guid.ToByteArray();
            return (GuidVersion)((bytes[VERSION_BYTE] & 0xFF) >> VERSION_BYTE_SHIFT);
        }

        public static Guid GenerateTimeBasedGuid(DateTime? dateTime = null)
        {
            return GenerateTimeBasedGuid(dateTime ?? DateTime.UtcNow,
                dateTime.HasValue ? GenerateClockSequenceBytes(dateTime.Value) : _clockSequenceBytes,
                _nodeBytes);
        }

        #endregion

        #region Nested class

        public enum GuidVersion
        {
            TimeBased = 0x01,
            Reserved = 0x02,
            NameBased = 0x03,
            Random = 0x04
        }

        #endregion
    }
}