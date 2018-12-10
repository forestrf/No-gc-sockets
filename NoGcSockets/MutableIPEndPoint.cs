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
				int socketAddressOffset = AddressFamily.InterNetwork == socketAddress.Family ? 4 : 8;
				byte[] bytes = AddressFamily.InterNetwork == socketAddress.Family ? new byte[4] : new byte[16];
				for (int i = 0; i < bytes.Length; i++) {
					bytes[i] = socketAddress[socketAddressOffset + i];
				}
				return new IPAddress(bytes);
			}
			set {
				int socketAddressOffset = AddressFamily.InterNetwork == socketAddress.Family ? 4 : 8;
				var ip = new IPHolder(value);
				for (int i = 0; i < ip.Length; i++) {
					socketAddress[socketAddressOffset + i] = ip[i];
				}
			}
		}

		internal void Set(ref IPEndPointStruct ipEndPointStruct) {
			Port = ipEndPointStruct.port;
			int socketAddressOffset = AddressFamily.InterNetwork == socketAddress.Family ? 4 : 8;
			for (int i = 0; i < ipEndPointStruct.ip.Length; i++) {
				socketAddress[socketAddressOffset + i] = ipEndPointStruct.ip[i];
			}
		}

		public override bool Equals(object obj) {
			MutableIPEndPoint iPEndPoint = obj as MutableIPEndPoint;
			return null != iPEndPoint && socketAddress.Equals(iPEndPoint.socketAddress);
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
