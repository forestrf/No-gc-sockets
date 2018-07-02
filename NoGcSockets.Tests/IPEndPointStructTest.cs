using NUnit.Framework;
using System.Net;

namespace NoGcSockets.Tests {
	[TestFixture]
	public class IPEndPointStructTest {
		[Test]
		public void ToStringTest() {
			IPAddress ipv4 = IPAddress.Parse("127.0.0.1");
			IPAddress ipv6 = IPAddress.Parse("2001:4860:4801:32::37");
			Assert.AreEqual(new IPEndPoint(ipv4, 123).ToString(), new IPEndPointStruct(ipv4, 123).ToString());
			Assert.AreEqual(new IPEndPoint(ipv6, 123).ToString(), new IPEndPointStruct(ipv6, 123).ToString());
		}

		[Test]
		public void ImplicitCastTest() {
			var epToCast = new MutableIPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
			IPEndPointStruct casted1 = epToCast;
			IPEndPoint casted2 = epToCast;
			Assert.AreEqual(new IPv4Holder(epToCast.Address), casted1.ipv4);
			Assert.AreEqual(epToCast.Address, casted2.Address);
			Assert.AreEqual(epToCast.Port, casted1.port);
			Assert.AreEqual(epToCast.Port, casted2.Port);
		}
	}
}
