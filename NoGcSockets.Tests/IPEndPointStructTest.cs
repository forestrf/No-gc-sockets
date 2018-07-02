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
		public void ImplicitCastTest1() {
			MutableIPEndPoint epToCast = new MutableIPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
			IPEndPointStruct casted1 = epToCast;
			IPEndPoint casted2 = epToCast;
			Assert.AreEqual(new IPv4Holder(epToCast.Address), casted1.ipv4);
			Assert.AreEqual(epToCast.Address, casted2.Address);
			Assert.AreEqual(epToCast.Port, casted1.port);
			Assert.AreEqual(epToCast.Port, casted2.Port);
		}

		[Test]
		public void ImplicitCastTest2() {
			EndPoint epToCast1 = new MutableIPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
			IPEndPointStruct casted1 = epToCast1;
			EndPoint epToCast2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
			IPEndPointStruct casted2 = epToCast2;
			Assert.AreEqual(casted1, casted2);
			Assert.AreEqual(casted1.ipv4, new IPv4Holder(((MutableIPEndPoint) epToCast1).Address));
			Assert.AreEqual(casted2.ipv4, new IPv4Holder(((IPEndPoint) epToCast2).Address));
			Assert.AreEqual(casted1.port, ((MutableIPEndPoint) epToCast1).Port);
			Assert.AreEqual(casted2.port, ((IPEndPoint) epToCast2).Port);
		}
	}
}
