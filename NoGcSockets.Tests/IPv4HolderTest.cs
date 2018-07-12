using NUnit.Framework;
using System.Net;
using System.Net.Sockets;

namespace NoGcSockets.Tests {
	[TestFixture]
	public class IPv4HolderTest {
		[Test]
		public void IPv4ByteAccessing() {
			IPHolder holder = new IPHolder(AddressFamily.InterNetwork);
			holder[0] = 192;
			holder[1] = 168;
			holder[2] = 1;
			holder[3] = 123;
			Assert.AreEqual(IPAddress.Parse("192.168.1.123"), holder.ToIPAddress());
			Assert.AreEqual(192, holder[0]);
			Assert.AreEqual(168, holder[1]);
			Assert.AreEqual(1, holder[2]);
			Assert.AreEqual(123, holder[3]);
		}

		[Test]
		public void IPv4SocketAddressInitializer() {
			var holder = new IPHolder(new IPEndPoint(IPAddress.Parse("192.168.1.123"), 12345).Serialize());
			Assert.AreEqual(IPAddress.Parse("192.168.1.123"), holder.ToIPAddress());
		}
	}
}
