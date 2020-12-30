using System;
using System.Net;
using System.Net.Sockets;

namespace NoGcSockets {
	public static class SocketHandler {
		[ThreadStatic] private static MutableIPEndPoint temporalMutableEPv4;
		[ThreadStatic] private static MutableIPEndPoint temporalMutableEPv6;
		[ThreadStatic] private static EndPoint temporalMutableEPv4DownCasted;
		[ThreadStatic] private static EndPoint temporalMutableEPv6DownCasted;

		public static int SendTo(Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, ref IPEndPointStruct ipEndPointStruct) {
			if (AddressFamily.InterNetworkV6 == socket.AddressFamily) {
				if (null == temporalMutableEPv6) {
					temporalMutableEPv6 = new MutableIPEndPoint(socket.AddressFamily);
					temporalMutableEPv6DownCasted = temporalMutableEPv6;
				}
				temporalMutableEPv6.Set(ref ipEndPointStruct);
				return socket.SendTo(buffer, offset, size, socketFlags, temporalMutableEPv6);
			}
			else {
				if (null == temporalMutableEPv4) {
					temporalMutableEPv4 = new MutableIPEndPoint(socket.AddressFamily);
					temporalMutableEPv4DownCasted = temporalMutableEPv4;
				}
				temporalMutableEPv4.Set(ref ipEndPointStruct);
				return socket.SendTo(buffer, offset, size, socketFlags, temporalMutableEPv4);
			}
		}

		public static int ReceiveFrom(Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, ref IPEndPointStruct ipEndPointStruct) {
			if (AddressFamily.InterNetworkV6 == socket.AddressFamily) {
				if (null == temporalMutableEPv6) temporalMutableEPv6 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableEPv6.Set(ref ipEndPointStruct);
				temporalMutableEPv6DownCasted = temporalMutableEPv6;

				int receivedBytes = socket.ReceiveFrom(buffer, offset, size, socketFlags, ref temporalMutableEPv6DownCasted);
				ipEndPointStruct = new IPEndPointStruct(temporalMutableEPv6);
				return receivedBytes;
			}
			else {
				if (null == temporalMutableEPv4) temporalMutableEPv4 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableEPv4.Set(ref ipEndPointStruct);
				temporalMutableEPv4DownCasted = temporalMutableEPv4;

				int receivedBytes = socket.ReceiveFrom(buffer, offset, size, socketFlags, ref temporalMutableEPv4DownCasted);
				ipEndPointStruct = new IPEndPointStruct(temporalMutableEPv4);
				return receivedBytes;
			}
		}
	}
}
