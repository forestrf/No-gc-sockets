using System;
using System.Net;
using System.Net.Sockets;

namespace NoGcSockets {
	internal class MutableIPEndPoint : EndPoint {
		public SocketAddress socketAddress;

		public override AddressFamily AddressFamily {
			get {
				return socketAddress.Family;
			}
		}

		public ushort Port {
			get {
				return (ushort) ((socketAddress[2] << 8) | socketAddress[3]);
			}
			set {
				socketAddress[2] = (byte) (0xff & (value >> 8));
				socketAddress[3] = (byte) (0xff & (value));
			}
		}

		public MutableIPEndPoint() { }
		public MutableIPEndPoint(IPAddress address, ushort port) {
			socketAddress = new IPEndPoint(address, port).Serialize();
		}

		public MutableIPEndPoint(AddressFamily addressFamily) {
			if (AddressFamily.InterNetworkV6 == addressFamily) {
				socketAddress = new SocketAddress(addressFamily, 28);
			}
			else {
				socketAddress = new SocketAddress(addressFamily, 16);
			}
		}

		public override EndPoint Create(SocketAddress socketAddress) {
			this.socketAddress = socketAddress;
			return this;
		}

		public override SocketAddress Serialize() {
			return socketAddress;
		}

		public override string ToString() {
			return Address.ToString() + ":" + Port;
		}

		/// <summary>
		/// Allocates GC each call
		/// </summary>
		public IPAddress Address {
			get {
				if (AddressFamily.InterNetwork == socketAddress.Family) {
					byte[] bytes = new byte[4];
					for (int i = 0; i < bytes.Length; i++) {
						bytes[i] = socketAddress[i + 4];
					}
					return new IPAddress(bytes);
				}
				else {
					byte[] bytes = new byte[16];
					for (int i = 0; i < bytes.Length; i++) {
						bytes[i] = socketAddress[i + 8];
					}
					return new IPAddress(bytes);
				}
			}
			set {
				if (AddressFamily.InterNetwork == socketAddress.Family) {
					var ipv4 = new IPv4Holder(value);
					
					for (int i = 0; i < IPv4Holder.Length; i++) {
						socketAddress[i + 4] = ipv4[i];
					}
				}
				else {
					var ipv6 = new IPv6Holder(value);

					for (int i = 0; i < IPv6Holder.Length; i++) {
						socketAddress[i + 8] = ipv6[i];
					}
				}
			}
		}

		internal void Set(IPEndPointStruct ipEndPointStruct) {
			Port = ipEndPointStruct.port;
			if (AddressFamily.InterNetwork == socketAddress.Family) {
				for (int i = 0; i < IPv4Holder.Length; i++) {
					socketAddress[i + 4] = ipEndPointStruct.ipv4[i];
				}
			}
			else {
				for (int i = 0; i < IPv6Holder.Length; i++) {
					socketAddress[i + 8] = ipEndPointStruct.ipv6[i];
				}
			}
		}

		public override bool Equals(object obj) {
			MutableIPEndPoint iPEndPoint = obj as MutableIPEndPoint;
			return socketAddress.Equals(iPEndPoint.socketAddress);
		}

		public override int GetHashCode() {
			return socketAddress.GetHashCode();
		}

		public static implicit operator IPEndPoint(MutableIPEndPoint v) {
			return new IPEndPoint(v.Address, v.Port);
		}

		public static implicit operator IPEndPointStruct(MutableIPEndPoint v) {
			return new IPEndPointStruct(v);
		}
	}
}
