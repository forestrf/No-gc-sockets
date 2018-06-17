using BBuffer;
using System;
using System.Net;
using System.Text;

namespace NoGcSockets {
	public struct IPv4Holder : IEquatable<IPv4Holder>, IEquatable<SocketAddress>, IEquatable<IPAddress> {
		/// <summary>
		/// Holds the bytes that represent the ip
		/// </summary>
		public uint bits;

		public const int Length = 4;

		public IPv4Holder(IPAddress address) {
			bits = (uint) address.Address; // Obsolete, but avoids generating Garbage
			if (BitConverter.IsLittleEndian) {
				// Reverse byte order
				bits = bits << 24 | (bits & 0xff00) << 8 | (bits & 0xff0000) >> 8 | bits >> 24;
			}
		}
		public IPv4Holder(SocketAddress socketAddress) {
			if (System.Net.Sockets.AddressFamily.InterNetwork != socketAddress.Family) throw new Exception();
			bits =
				((uint) socketAddress[4] << 24) |
				((uint) socketAddress[5] << 16) |
				((uint) socketAddress[6] << 8) |
				((uint) socketAddress[7]);
		}
		public void Write(ref ByteBuffer buffer) {
			buffer.Put(bits);
		}
		public void Read(ref ByteBuffer buffer) {
			bits = buffer.GetUInt();
		}

		public IPAddress ToIPAddress() {
			var address = new byte[4];
			var buffer = new ByteBuffer(address);
			buffer.Put(bits);

			return new IPAddress(address);
		}

		public bool Equals(IPv4Holder other) {
			return bits == other.bits;
		}

		public bool Equals(SocketAddress other) {
			return Equals(new IPv4Holder(other));
		}

		public bool Equals(IPAddress other) {
			return Equals(new IPv4Holder(other));
		}

		public byte this[int i] {
			get {
				return (byte) (0xffu & (bits >> ((3 - i) * 8)));
			}
			set {
				bits = (bits & (~(0xffu << ((3 - i) * 8)))) | ((uint) value << ((3 - i) * 8));
			}
		}

		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder(15);
			stringBuilder.Append(this[0]);
			stringBuilder.Append('.');
			stringBuilder.Append(this[1]);
			stringBuilder.Append('.');
			stringBuilder.Append(this[2]);
			stringBuilder.Append('.');
			stringBuilder.Append(this[3]);
			return stringBuilder.ToString();
		}
	}
}
