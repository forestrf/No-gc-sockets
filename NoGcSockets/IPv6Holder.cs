using BBuffer;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace NoGcSockets {
	public struct IPv6Holder : IEquatable<IPv6Holder>, IEquatable<SocketAddress>, IEquatable<IPAddress> {
		private static FieldInfo field = typeof(IPAddress).GetField("m_Numbers", BindingFlags.NonPublic | BindingFlags.Instance);

		/// <summary>
		/// Holds the bytes that represent the ip
		/// </summary>
		public ulong msb, lsb;

		public const int Length = 16;

		public IPv6Holder(IPAddress address, bool tryUsingReflection = true) {
			if (tryUsingReflection) {
				if (field != null) {
					var m_Numbers = (ushort[]) field.GetValue(address);
					msb = ((ulong) m_Numbers[0] << 48) | ((ulong) m_Numbers[1] << 32) | (uint) (m_Numbers[2] << 16) | m_Numbers[3];
					lsb = ((ulong) m_Numbers[4] << 48) | ((ulong) m_Numbers[5] << 32) | (uint) (m_Numbers[6] << 16) | m_Numbers[7];
					return;
				}
			}

			var buffer = new ByteBuffer(address.GetAddressBytes(), Endianness.Big);
			msb = buffer.GetULong();
			lsb = buffer.GetULong();
		}
		public IPv6Holder(SocketAddress socketAddress) {
			if (System.Net.Sockets.AddressFamily.InterNetworkV6 != socketAddress.Family) throw new Exception();
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
		}
		public void Write(ref ByteBuffer buffer) {
			buffer.Put(msb);
			buffer.Put(lsb);
		}
		public void Read(ref ByteBuffer buffer) {
			msb = buffer.GetULong();
			lsb = buffer.GetULong();
		}

		public IPAddress ToIPAddress() {
			var address = new byte[16];
			var buffer = new ByteBuffer(address);
			buffer.Put(msb);
			buffer.Put(lsb);

			return new IPAddress(address);
		}

		public bool Equals(IPv6Holder other) {
			return msb == other.msb && lsb == other.lsb;
		}

		public bool Equals(SocketAddress other) {
			return Equals(new IPv6Holder(other));
		}

		public bool Equals(IPAddress other) {
			return Equals(new IPv6Holder(other));
		}

		public byte this[int i] {
			get {
				if (i < 8) {
					return (byte) (0xff & (msb >> ((7 - i - 8) * 8)));
				}
				else {
					return (byte) (0xff & (lsb >> ((7 - i) * 8)));
				}
			}
			set {
				if (i < 8) {
					msb = (msb & (~(0xfful << ((7 - i) * 8)))) | ((ulong) value << ((7 - i) * 8));
				}
				else {
					lsb = (lsb & (~(0xfful << ((7 - i - 8) * 8)))) | ((ulong) value << ((7 - i - 8) * 8));
				}
			}
		}

		public override string ToString() {
			int capacity = 256;
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
