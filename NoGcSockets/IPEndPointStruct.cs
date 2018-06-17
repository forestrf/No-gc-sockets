using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NoGcSockets {
	[StructLayout(LayoutKind.Explicit)]
	public struct IPEndPointStruct : IEquatable<IPEndPointStruct>, IEquatable<IPEndPoint> {
		[FieldOffset(0)] public readonly ushort port;
		[FieldOffset(2)] public readonly AddressFamily addressFamily;
		[FieldOffset(6)] public readonly IPv4Holder ipv4;
		[FieldOffset(6)] public readonly IPv6Holder ipv6;

		public IPEndPointStruct(IPv4Holder ipv4, ushort port) : this() {
			addressFamily = AddressFamily.InterNetwork;
			this.ipv4 = ipv4;
			this.port = port;
		}

		public IPEndPointStruct(IPv6Holder ipv6, ushort port) : this() {
			addressFamily = AddressFamily.InterNetwork;
			this.ipv6 = ipv6;
			this.port = port;
		}

		public IPEndPointStruct(IPAddress address, ushort port) : this() {
			this.port = port;
			addressFamily = address.AddressFamily;
			if (AddressFamily.InterNetwork == addressFamily) {
				ipv4 = new IPv4Holder(address);
			}
			else {
				ipv6 = new IPv6Holder(address);
			}
		}

		internal IPEndPointStruct(MutableIPEndPoint ep) : this() {
			port = ep.Port;
			addressFamily = ep.AddressFamily;
			if (AddressFamily.InterNetwork == addressFamily) {
				ipv4 = new IPv4Holder(ep.socketAddress);
			}
			else {
				ipv6 = new IPv6Holder(ep.socketAddress);
			}
		}

		public IPAddress GetAddress() {
			if (AddressFamily.InterNetwork == addressFamily) {
				return ipv4.ToIPAddress();
			}
			else {
				return ipv6.ToIPAddress();
			}
		}

		public bool Equals(IPEndPointStruct other) {
			return port == other.port && addressFamily == other.addressFamily && (
				AddressFamily.InterNetwork == addressFamily ?
				ipv4.Equals(other.ipv4) :
				ipv6.Equals(other.ipv6));
		}

		internal bool Equals(MutableIPEndPoint other) {
			return port == other.Port && addressFamily == other.AddressFamily && (
				AddressFamily.InterNetwork == addressFamily ?
				ipv4.Equals(other.socketAddress) :
				ipv6.Equals(other.socketAddress));
		}

		public bool Equals(IPEndPoint other) {
			return port == other.Port && addressFamily == other.AddressFamily && (
				AddressFamily.InterNetwork == addressFamily ?
				ipv4.Equals(other.Address) :
				ipv6.Equals(other.Address));
		}

		public override string ToString() {
			if (AddressFamily.InterNetwork == addressFamily) {
				return ipv4.ToString() + ":" + port;
			}
			else {
				return ipv6.ToString() + ":" + port;
			}
		}
	}
}
