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
	}
}
