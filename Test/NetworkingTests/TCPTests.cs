using Parser.Message;

using System.Net.Sockets;

using Networking.TCP.Client;
using Networking.TCP.Server;

namespace Test.NetworkingTests
{
class TCPTests : ITests
{
    const string SERVER_IP = "127.0.0.1";
    const ushort SERVER_PORT = 6969;

    const uint TEST_DATA_BYTES_SIZE = 1_000_000;
    const Message.Type TEST_DATA_TYPE = Message.Type.PING;

    bool SucceededServer = false;
    bool SucceededClient = false;
    int TestsTestedCount = 0;

    readonly TCPServerRaw Server = new(SERVER_PORT);
    readonly TCPClient Client = new();

    readonly byte[] TEST_DATA_BYTES = new byte[TEST_DATA_BYTES_SIZE];

    public override bool Test()
    {
        TestModulesStart();

        TestServer();
        TestClient();

        while (TestsTestedCount < 2)
        {
            Thread.Sleep(1);
        }

        if (SucceededServer && SucceededClient)
        {
            TestModulesEnd();
        }

        return SucceededServer && SucceededClient;
    }

    public TCPTests()
    {
        for (int i = 0; i < TEST_DATA_BYTES_SIZE; i++)
        {
            TEST_DATA_BYTES[i] = (byte)i;
        }
    }

    void TestServer()
    {
        Server.Start((error, client) =>
                     {
                         if (error != SocketError.Success)
                         {
                             Fail("TestServer");
                             return;
                         }

                         client.Receive((error, messageDisassembled) =>
                                        {
                                            if (messageDisassembled != null && messageDisassembled.Stream != null &&
                                                messageDisassembled.Stream.SequenceEqual(TEST_DATA_BYTES) &&
                                                messageDisassembled.Type == TEST_DATA_TYPE)
                                            {
                                                client.Send(messageDisassembled.Stream, messageDisassembled.Type,
                                                            (error, bytesTransferred) =>
                                                            {
                                                                if (error != SocketError.Success)
                                                                {
                                                                    Fail("TestServer");
                                                                    return;
                                                                }

                                                                SucceededServer = true;
                                                                PrintModuleTest(true, "TestServer");

                                                                Interlocked.Increment(ref TestsTestedCount);
                                                            });
                                            }
                                            else
                                            {
                                                Fail("TestServer");
                                            }
                                        });
                     });
    }

    void TestClient()
    {
        Client.Connect(SERVER_IP, SERVER_PORT,
                       (_, connected) =>
                       {
                           if (!connected)
                           {
                               Fail("TestClient");
                               return;
                           }

                           Client.Send(TEST_DATA_BYTES, TEST_DATA_TYPE,
                                       (error, _) =>
                                       {
                                           if (error != SocketError.Success)
                                           {
                                               Fail("TestClient");
                                               return;
                                           }

                                           Client.Receive(
                                               (_, messageDisassembled) =>
                                               {
                                                   if (messageDisassembled != null &&
                                                       messageDisassembled.Stream != null &&
                                                       messageDisassembled.Stream.SequenceEqual(TEST_DATA_BYTES) &&
                                                       messageDisassembled.Type == TEST_DATA_TYPE)
                                                   {
                                                       SucceededClient = true;
                                                       PrintModuleTest(true, "TestClient");

                                                       Interlocked.Increment(ref TestsTestedCount);
                                                   }
                                                   else
                                                   {
                                                       Fail("TestClient");
                                                   }
                                               });
                                       });
                       });
    }

    void Fail(string methodName)
    {
        Client.Stop();
        Server.Stop();

        SucceededServer = false;
        SucceededClient = false;

        Interlocked.Exchange(ref TestsTestedCount, 2);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\t{methodName} Test Module FAILED...");
        Console.ForegroundColor = ConsoleColor.White;
    }
}
}
