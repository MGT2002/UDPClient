using MyExtensions;
using System.Net;
using System.Net.Sockets;
using System.Text;

const int Timeout = 3000;
const int MaxTries = 5;
const int DefaultEchoPort = 7;

if (args.Length < 2 || args.Length > 3)
    throw new ArgumentException("Parameters: <Server>, <Word>, [<Port>]");

string server = args[0];
int servPort = args.Length == 3 ? int.Parse(args[2]) : DefaultEchoPort;
byte[] sendPacket = Encoding.ASCII.GetBytes(args[1]);
byte[] rcvPacket = new byte[sendPacket.Length];

using Socket sock = new(AddressFamily.InterNetwork, SocketType.Dgram,
    ProtocolType.Udp);
sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout,
    Timeout);
EndPoint remoteEndPoint = new IPEndPoint(Dns.GetHostEntry(server, AddressFamily.InterNetwork)
    .AddressList[0], servPort);

int tries = 0;
bool responseReceived = false;

do
{
    sock.SendTo(sendPacket, remoteEndPoint);
    Console.WriteLine("Sent {0} bytes to the server!", sendPacket.Length);

    try
    {
        sock.ReceiveFrom(rcvPacket, ref remoteEndPoint);
        responseReceived = true;

    }
    catch (SocketException se)
    {
        if (se.ErrorCode == 10060)
            $"Timed out, {MaxTries - tries - 1} more tries".Log();
        else
            se.ErrorCode.Log(se.Message);
    }
    catch (Exception e)
    {
        e.Message.Log();
    }
    finally
    {
        tries++;
    }
} while (!responseReceived && tries < MaxTries);

if (responseReceived)
    $"Received {rcvPacket.Length} bytes from {remoteEndPoint}:"
    .Log(Encoding.ASCII.GetString(rcvPacket));
else
    "No response - - giving up.".Log();