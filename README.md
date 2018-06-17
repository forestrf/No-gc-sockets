# No Garbage Sockets
Small library to send and receive using sockets without allocating garbage

It is accomplished by wrapping the logic of receiving and sending using a socket with methods that use the class `MutableIPEndPoint`, a custom implementation of `IPEndPoint` that does not generate garbage but in exchange does not work as expected when used outside simply sending and receiving. To avoid using this problematic class, a struct that holds both ipv4 and ipv6 is presented `IPEndPointStruct`. Because it is a struct it is highly recommended to pass it using `ref` in methods to avoid uneeded copies of it and a slowdown.

This functions do not use try catch, it is your job to continue using them as before.

You will need to start using IPEndPointStruct instead of IPEndPoint.

## How to use

There is a working version made in this [unit test](https://github.com/forestrf/No-gc-sockets/blob/master/NoGcSockets.Tests/FullTest.cs).

```csharp
// Setup of sockets. Done as always
Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
sender.Bind(new IPEndPoint(IPAddress.Any, 0));
Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
receiver.Bind(new IPEndPoint(IPAddress.Any, 0));

// Send
IPEndPointStruct sendTarget = new IPEndPointStruct(new IPv4Holder(IPAddress.Loopback), 0); // Listen locally
SocketHandler.SendTo(sender, bufferSend, 0, bufferSend.Length, SocketFlags.None, ref sendTarget);

// Receive
IPEndPointStruct from = new IPEndPointStruct(new IPv4Holder(), 0);
int recvBytes = SocketHandler.ReceiveFrom(receiver, bufferRecv, 0, bufferRecv.Length, SocketFlags.None, ref from);
// from contains the sender address, recvBytes the number of received bytes
```

This code is licensed under the permissive MIT X11 license. For the full text
see `LICENSE.txt`.
