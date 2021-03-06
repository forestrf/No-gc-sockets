using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NoGcSockets.Tests {
	[TestFixture]
	public class IPv6HolderTest {
		[Test]
		public void IPv6AndReflection() {
			Assert.AreEqual(IPAddress.IPv6Any, new IPHolder(IPAddress.IPv6Any, false).ToIPAddress());
			Assert.AreEqual(IPAddress.IPv6Any, new IPHolder(IPAddress.IPv6Any, true).ToIPAddress());
			Assert.AreEqual(IPAddress.IPv6Loopback, new IPHolder(IPAddress.IPv6Loopback, false).ToIPAddress());
			Assert.AreEqual(IPAddress.IPv6Loopback, new IPHolder(IPAddress.IPv6Loopback, true).ToIPAddress());
			Assert.AreEqual(IPAddress.Parse("2001:4860:4801:32::37"), new IPHolder(IPAddress.Parse("2001:4860:4801:32::37"), false).ToIPAddress());
			Assert.AreEqual(IPAddress.Parse("2001:4860:4801:32::37"), new IPHolder(IPAddress.Parse("2001:4860:4801:32::37"), true).ToIPAddress());
		}

		[Test]
		public void IPv6ByteAccessing() {
			IPHolder holder = new IPHolder(AddressFamily.InterNetworkV6);
			holder[0] = 0x20;
			holder[1] = 0x01;
			holder[2] = 0x48;
			holder[3] = 0x60;
			holder[4] = 0x48;
			holder[5] = 0x01;
			holder[6] = 0x00;
			holder[7] = 0x32;
			holder[8] = 0x00;
			holder[9] = 0x00;
			holder[10] = 0x00;
			holder[11] = 0x00;
			holder[12] = 0x00;
			holder[13] = 0x00;
			holder[14] = 0x00;
			holder[15] = 0x37;
			Assert.AreEqual(IPAddress.Parse("2001:4860:4801:32::37"), holder.ToIPAddress());
			Assert.AreEqual(0x20, holder[0]);
			Assert.AreEqual(0x01, holder[1]);
			Assert.AreEqual(0x48, holder[2]);
			Assert.AreEqual(0x60, holder[3]);
			Assert.AreEqual(0x48, holder[4]);
			Assert.AreEqual(0x01, holder[5]);
			Assert.AreEqual(0x00, holder[6]);
			Assert.AreEqual(0x32, holder[7]);
			Assert.AreEqual(0x00, holder[8]);
			Assert.AreEqual(0x00, holder[9]);
			Assert.AreEqual(0x00, holder[10]);
			Assert.AreEqual(0x00, holder[11]);
			Assert.AreEqual(0x00, holder[12]);
			Assert.AreEqual(0x00, holder[13]);
			Assert.AreEqual(0x00, holder[14]);
			Assert.AreEqual(0x37, holder[15]);
		}

		[Test]
		public void IPv6SocketAddressInitializer() {
			var holder = new IPHolder(new IPEndPoint(IPAddress.Parse("2001:4860:4801:32::37"), 12345).Serialize());
			Assert.AreEqual(IPAddress.Parse("2001:4860:4801:32::37"), holder.ToIPAddress());
		}

		[Test]
		public void IPv6RandomAddresses() {
			Random r = new Random(0);
			for (int i = 0; i < 10000; i++) {
				StringBuilder addr = new StringBuilder();
				for (int j = 0; j < 8; j++) {
					addr.Append(r.Next(0, ushort.MaxValue).ToString("X2"));
					if (j < 7) addr.Append(":");
				}
				string address = addr.ToString();
				
				var ip = IPAddress.Parse(address);
				var holder = new IPHolder(new IPEndPoint(ip, r.Next(0, ushort.MaxValue)).Serialize());
				Assert.AreEqual(ip, holder.ToIPAddress());
			}
		}
	}
}
