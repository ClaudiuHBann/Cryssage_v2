using Test.NetworkingTests;
using Test.ParserTests;

namespace Test
{
class Program
{
    static void Main()
    {
        ManagerTests managerTests = new();

        managerTests.AddTest(new MessageTests());
        managerTests.AddTest(new TCPTests());

        managerTests.Run();
    }
}
}

/*
    TODO:
            - add to the parser a bit in the metadata which says about the data if it is fragmented, if it is we have
              the implementation we got now else the packets of data will be just one and it will be just a stream of
              bytes (especially for the sending data protocol to make it faster)
            - add progress in the GUI
            - add the ability to block peers
            - the manager for file transfer should better handle and return errors not just a EOS for errors
            - add the ability to talk to yourself
            - use the current time in the GUI for messages
            - use the name of the files in the messages in GUI
            - add a menu bar for quick actions (broadcast, clear database, set default download location)
*/
