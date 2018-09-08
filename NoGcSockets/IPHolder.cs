using BBuffer;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NoGcSockets {
	[StructLayout(LayoutKind.Explicit)]
	public struct IPHolder : IEquatable<IPHolder>, IEquatable<SocketAddress>, IEquatable<IPAddress> {
		private static FieldInfo field = typeof(IPAddress).GetField("m_Numbers", BindingFlags.NonPublic | BindingFlags.Instance);

		[FieldOffset(0)]
		public readonly bool isIPv4;

		/// <summary>Holds the bytes that represent the ip</summary>
		[FieldOffset(1)]
		public uint bits;

		/// <summary>Holds the bytes that represent the ip</summary>
		[FieldOffset(1)]
		public ulong msb;

		/// <summary>Holds the bytes that represent the ip</summary>
		[FieldOffset(1 + 8)]
		public ulong lsb;

		public int Length {
			get {
				return isIPv4 ? 4 : 16;
			}
		}

		public IPHolder(AddressFamily addressFamily) : this() {
			switch (addressFamily) {
				case AddressFamily.InterNetwork:
					isIPv4 = true;
					break;
				case AddressFamily.InterNetworkV6:
					isIPv4 = false;
					break;
				default:
					throw new Exception("Address Family not supported: " + addressFamily);
			}
		}

		public IPHolder(IPAddress address, bool tryUsingReflection = true) : this() {
			switch (address.AddressFamily) {
				case AddressFamily.InterNetwork: {
					isIPv4 = true;
					bits = (uint) address.Address; // Obsolete, but avoids generating Garbage
					if (BitConverter.IsLittleEndian) {
						// Reverse byte order
						bits = bits << 24 | (bits & 0xff00) << 8 | (bits & 0xff0000) >> 8 | bits >> 24;
					}
					break;
				}
				case AddressFamily.InterNetworkV6: {
					isIPv4 = false;

					if (tryUsingReflection && null != field) {
						var m_Numbers = (ushort[]) field.GetValue(address);
						msb = ((ulong) m_Numbers[0] << 48) | ((ulong) m_Numbers[1] << 32) | (uint) (m_Numbers[2] << 16) | m_Numbers[3];
						lsb = ((ulong) m_Numbers[4] << 48) | ((ulong) m_Numbers[5] << 32) | (uint) (m_Numbers[6] << 16) | m_Numbers[7];
					}
					else {
						var buffer = new ByteBuffer(address.GetAddressBytes(), Endianness.Big);
						msb = buffer.GetULong();
						lsb = buffer.GetULong();
					}

					break;
				}
				default:
					throw new Exception("Address Family not supported: " + address.AddressFamily);
			}
		}

		public IPHolder(SocketAddress socketAddress) : this() {
			switch (socketAddress.Family) {
				case AddressFamily.InterNetwork: {
					isIPv4 = true;
					bits =
						((uint) socketAddress[4] << 24) |
						((uint) socketAddress[5] << 16) |
						((uint) socketAddress[6] << 8) |
						((uint) socketAddress[7]);
					break;
				}
				case AddressFamily.InterNetworkV6: {
					msb =
						((ulong) socketAddress[8] << 56) |
						((ulong) socketAddress[9] << 48) |
						((ulong) socketAddress[10] << 40) |
						((ulong) socketAddress[11] << 32) |
						((ulong) socketAddress[12] << 24) |
						((ulong) socketAddress[13] << 16) |
						((ulong) socketAddress[14] << 8) |
						((ulong) socketAddress[15]);
					lsb =
						((ulong) socketAddress[16] << 56) |
						((ulong) socketAddress[17] << 48) |
						((ulong) socketAddress[18] << 40) |
						((ulong) socketAddress[19] << 32) |
						((ulong) socketAddress[20] << 24) |
						((ulong) socketAddress[21] << 16) |
						((ulong) socketAddress[22] << 8) |
						((ulong) socketAddress[23]);
					break;
				}
				default:
					throw new Exception("Socket Address Family not supported: " + socketAddress.Family);
			}
		}

		public static implicit operator IPHolder(IPAddress v) {
			return new IPHolder(v);
		}

		public void Write(ref ByteBuffer buffer) {
			if (isIPv4) {
				buffer.Put(bits);
			}
			else {
				buffer.Put(msb);
				buffer.Put(lsb);
			}
		}
		public void Read(ref ByteBuffer buffer, bool isIPv4) {
			if (isIPv4) {
				this = new IPHolder(AddressFamily.InterNetwork);
				bits = buffer.GetUInt();
			}
			else {
				this = new IPHolder(AddressFamily.InterNetworkV6);
				msb = buffer.GetULong();
				lsb = buffer.GetULong();
			}
		}

		public IPAddress ToIPAddress() {
			byte[] address;
			if (isIPv4) {
				address = new byte[4];
				var buffer = new ByteBuffer(address);
				buffer.Put(bits);
			}
			else {
				address = new byte[16];
				var buffer = new ByteBuffer(address);
				buffer.Put(msb);
				buffer.Put(lsb);
			}

			return new IPAddress(address);
		}

		public bool Equals(IPHolder other) {
			if (isIPv4 != other.isIPv4) return false;
			if (isIPv4) {
				return bits == other.bits;
			}
			else {
				return msb == other.msb && lsb == other.lsb;
			}
		}

		public bool Equals(SocketAddress other) {
			return Equals(new IPHolder(other));
		}

		public bool Equals(IPAddress other) {
			return Equals(new IPHolder(other));
		}

		public byte this[int i] {
			get {
				if (isIPv4) {
					return (byte) (0xffu & (bits >> ((3 - i) * 8)));
				}
				else {
					if (i < 8) {
						return (byte) (msb >> ((7 - i - 8) * 8));
					}
					else {
						return (byte) (lsb >> ((7 - i) * 8));
					}
				}
			}
			set {
				if (isIPv4) {
					bits = (bits & (~(0xffu << ((3 - i) * 8)))) | ((uint) value << ((3 - i) * 8));
				}
				else {
					if (i < 8) {
						msb = (msb & (~(0xfful << ((7 - i) * 8)))) | ((ulong) value << ((7 - i) * 8));
					}
					else {
						lsb = (lsb & (~(0xfful << ((7 - i - 8) * 8)))) | ((ulong) value << ((7 - i - 8) * 8));
					}
				}
			}
		}

		public override int GetHashCode() {
			var xor = msb ^ lsb;
			return (int) ((uint) (xor >> 32) ^ (uint) (0xffffffffu & xor));
		}

		public override string ToString() {
			if (isIPv4) {
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
			else {
				const int capacity = 256;
				StringBuilder stringBuilder = new StringBuilder(capacity);
				stringBuilder.Append('[');

				int timesClean = 0;
				for (int i = 0; i < 8; i++) {
					if (0 != this[i * 2]) {
						stringBuilder.Append(this[i * 2].ToString("x2", CultureInfo.InvariantCulture));
						stringBuilder.Append(this[i * 2 + 1].ToString("x2", CultureInfo.InvariantCulture));
						timesClean = 0;
					}
					else if (0 != this[i * 2 + 1]) {
						stringBuilder.Append(this[i * 2 + 1].ToString("x2", CultureInfo.InvariantCulture));
						timesClean = 0;
					}
					else {
						timesClean++;
					}
					if (timesClean < 2 && i != 7) {
						stringBuilder.Append(':');
					}
				}

				/*
				if (this.m_ScopeId != 0L) {
					stringBuilder.Append('%').Append((uint) this.m_ScopeId);
				}
				*/
				stringBuilder.Append(']');
				return stringBuilder.ToString();
			}
		}
	}
}
