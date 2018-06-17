using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NoGcSockets {
	[StructLayout(LayoutKind.Explicit)]
	public struct IPEndPointStruct : IEquatable<IPEndPointStruct>, IEquatable<IPEndPoint> {
		[FieldOffset(0)] public readonly ushort port;
		[FieldOffset(2)] public readonly AddressFamily family;
		[FieldOffset(6)] public readonly IPv4Holder ipv4;
		[FieldOffset(6)] public readonly IPv6Holder ipv6;

		public IPEndPointStruct(IPv4Holder ipv4, ushort port) : this() {
			family = AddressFamily.InterNetwork;
			this.ipv4 = ipv4;
			this.port = port;
		}

		public IPEndPointStruct(IPv6Holder ipv6, ushort port) : this() {
			family = AddressFamily.InterNetwork;
			this.ipv6 = ipv6;
			this.port = port;
		}

		internal IPEndPointStruct(MutableIPEndPoint ep) : this() {
			port = ep.Port;
			family = ep.AddressFamily;
			if (AddressFamily.InterNetwork == family) {
				ipv4 = new IPv4Holder(ep.socketAddress);
			}
			else {
				ipv6 = new IPv6Holder(ep.socketAddress);
			}
		}

		public IPAddress GetAddress() {
			if (AddressFamily.InterNetwork == family) {
				return ipv4.ToIPAddress();
			}
			else {
				return ipv6.ToIPAddress();
			}
		}

		public bool Equals(IPEndPointStruct other) {
			return port == other.port && family == other.family && (
				AddressFamily.InterNetwork == family ?
				ipv4.Equals(other.ipv4) :
				ipv6.Equals(other.ipv6));
		}

		internal bool Equals(MutableIPEndPoint other) {
			return port == other.Port && family == other.AddressFamily && (
				AddressFamily.InterNetwork == family ?
				ipv4.Equals(other.socketAddress) :
				ipv6.Equals(other.socketAddress));
		}

		public bool Equals(IPEndPoint other) {
			return port == other.Port && family == other.AddressFamily && (
				AddressFamily.InterNetwork == family ?
				ipv4.Equals(other.Address) :
				ipv6.Equals(other.Address));
		}
	}
}
