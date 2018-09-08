using System;
using System.Net;
using System.Net.Sockets;

namespace NoGcSockets {
	public static class SocketHandler {
		[ThreadStatic] private static MutableIPEndPoint temporalMutableEPv4;
		[ThreadStatic] private static MutableIPEndPoint temporalMutableEPv6;

		public static int SendTo(Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, ref IPEndPointStruct ipEndPointStruct) {
			EndPoint downCasted;

			if (AddressFamily.InterNetworkV6 == socket.AddressFamily) {
				if (null == temporalMutableEPv6) temporalMutableEPv6 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableEPv6.Set(ref ipEndPointStruct);
				downCasted = temporalMutableEPv6;
			}
			else {
				if (null == temporalMutableEPv4) temporalMutableEPv4 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableEPv4.Set(ref ipEndPointStruct);
				downCasted = temporalMutableEPv4;
			}
			return socket.SendTo(buffer, offset, size, socketFlags, downCasted);
		}

		public static int ReceiveFrom(Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, ref IPEndPointStruct ipEndPointStruct) {
			EndPoint downCasted;

			if (AddressFamily.InterNetworkV6 == socket.AddressFamily) {
				if (null == temporalMutableEPv6) temporalMutableEPv6 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableEPv6.Set(ref ipEndPointStruct);
				downCasted = temporalMutableEPv6;
			}
			else {
				if (null == temporalMutableEPv4) temporalMutableEPv4 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableEPv4.Set(ref ipEndPointStruct);
				downCasted = temporalMutableEPv4;
			}

			int receivedBytes = socket.ReceiveFrom(buffer, offset, size, socketFlags, ref downCasted);

			ipEndPointStruct = downCasted;

			return receivedBytes;
		}
	}
}
