using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;

namespace NoGcSockets.Tests {
	[TestFixture]
	public class FullTest {
		[Test]
		public void Execute() {
			// Prepare
			Random r = new Random(0);
			byte[] toSend = new byte[128];
			byte[] receive = new byte[128];
			for (int i = 0; i < toSend.Length; i++) {
				toSend[i] = (byte) r.Next();
			}



			// Senders
			Socket sender4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			Socket sender6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			sender4.Bind(new IPEndPoint(IPAddress.Any, 0));
			sender6.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));

			// Receivers
			Socket receiver4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			Socket receiver6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			receiver4.Bind(new IPEndPoint(IPAddress.Any, 0));
			receiver6.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));


			// Send
			IPEndPointStruct sendTarget4 = new IPEndPointStruct(new IPHolder(IPAddress.Loopback), (ushort) ((IPEndPoint) receiver4.LocalEndPoint).Port);
			SocketHandler.SendTo(sender4, toSend, 0, toSend.Length, SocketFlags.None, ref sendTarget4);

			IPEndPointStruct sendTarget6 = new IPEndPointStruct(new IPHolder(IPAddress.IPv6Loopback), (ushort) ((IPEndPoint) receiver6.LocalEndPoint).Port);
			SocketHandler.SendTo(sender6, toSend, 0, toSend.Length, SocketFlags.None, ref sendTarget6);


			// Receive
			IPEndPointStruct from4 = new IPEndPointStruct(new IPHolder(AddressFamily.InterNetwork), 0);
			int recv4 = SocketHandler.ReceiveFrom(receiver4, receive, 0, receive.Length, SocketFlags.None, ref from4);

			Assert.AreEqual(toSend.Length, recv4);
			CollectionAssert.AreEqual(toSend, receive);


			IPEndPointStruct from6 = new IPEndPointStruct(new IPHolder(AddressFamily.InterNetworkV6), 0);
			int recv6 = SocketHandler.ReceiveFrom(receiver6, receive, 0, receive.Length, SocketFlags.None, ref from6);

			Assert.AreEqual(toSend.Length, recv6);
			CollectionAssert.AreEqual(toSend, receive);
		}
	}
}
