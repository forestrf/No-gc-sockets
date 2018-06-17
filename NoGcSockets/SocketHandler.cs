using System;
using System.Net;
using System.Net.Sockets;

namespace NoGcSockets {
    public static class SocketHandler {
		[ThreadStatic] private static MutableIPEndPoint temporalMutableIPEndPointv4;
		[ThreadStatic] private static MutableIPEndPoint temporalMutableIPEndPointv6;
		
		public static int SendTo(Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, ref IPEndPointStruct ipEndPointStruct) {
			EndPoint downCasted;

			if (AddressFamily.InterNetworkV6 == socket.AddressFamily) {
				if (null == temporalMutableIPEndPointv6)
					temporalMutableIPEndPointv6 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableIPEndPointv6.Set(ipEndPointStruct);
				downCasted = temporalMutableIPEndPointv6;
				return socket.SendTo(buffer, offset, size, socketFlags, downCasted);
			}
			else {
				if (null == temporalMutableIPEndPointv4)
					temporalMutableIPEndPointv4 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableIPEndPointv4.Set(ipEndPointStruct);
				downCasted = temporalMutableIPEndPointv4;
				return socket.SendTo(buffer, offset, size, socketFlags, downCasted);
			}
		}

		public static int ReceiveFrom(Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, ref IPEndPointStruct ipEndPointStruct) {
			EndPoint downCasted;

			if (AddressFamily.InterNetwork == socket.AddressFamily) {
				if (null == temporalMutableIPEndPointv4)
					temporalMutableIPEndPointv4 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableIPEndPointv4.Set(ipEndPointStruct);
				downCasted = temporalMutableIPEndPointv4;
			}
			else {
				if (null == temporalMutableIPEndPointv6)
					temporalMutableIPEndPointv6 = new MutableIPEndPoint(socket.AddressFamily);
				temporalMutableIPEndPointv6.Set(ipEndPointStruct);
				downCasted = temporalMutableIPEndPointv6;
			}
			
			int receivedBytes = socket.ReceiveFrom(buffer, offset, size, socketFlags, ref downCasted);

			ipEndPointStruct = new IPEndPointStruct((MutableIPEndPoint) downCasted);

			return receivedBytes;
		}
    }
}
