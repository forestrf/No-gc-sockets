using System;
using System.Net;
using System.Net.Sockets;

namespace NoGcSockets {
	public struct IPEndPointStruct : IEquatable<IPEndPointStruct>, IEquatable<IPEndPoint> {
		public readonly ushort port;
		public readonly IPHolder ip;

		public AddressFamily AddressFamily {
			get {
				return ip.isIPv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6;
			}
		}

		public IPEndPointStruct(IPHolder ip, ushort port) : this() {
			this.ip = ip;
			this.port = port;
		}

		public IPEndPointStruct(IPAddress address, ushort port) : this() {
			this.port = port;
			ip = new IPHolder(address);
		}

		internal IPEndPointStruct(MutableIPEndPoint ep) : this() {
			port = ep.Port;
			ip = new IPHolder(ep.socketAddress);
		}

		public IPEndPointStruct(IPEndPoint ipEndPoint) : this(ipEndPoint.Address, (ushort) ipEndPoint.Port) {
		}

		public IPAddress GetAddress() {
			return ip.ToIPAddress();
		}

		public bool Equals(IPEndPointStruct other) {
			return port == other.port && ip.Equals(other.ip);
		}

		internal bool Equals(MutableIPEndPoint other) {
			return port == other.Port && ip.Equals(other.socketAddress);
		}

		public bool Equals(IPEndPoint other) {
			return port == other.Port && ip.Equals(other.Address);
		}

		public override int GetHashCode() {
			return port << 16 ^ ip.GetHashCode();
		}

		public override string ToString() {
			return ip.ToString() + ":" + port;
		}

		public static implicit operator IPEndPointStruct(EndPoint v) {
			if (v is IPEndPoint) return new IPEndPointStruct(((IPEndPoint) v).Address, (ushort) ((IPEndPoint) v).Port);
			if (v is MutableIPEndPoint) return (MutableIPEndPoint) v;
			throw new Exception("Cast from " + v.GetType() + " to IPEndPointStruct not allowed");
		}
	}
}
