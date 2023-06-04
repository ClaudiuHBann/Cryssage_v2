using Test.ParserTests;
using Test.NetworkingTests;

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
        MUST:
            - save and load context
            - notifications (not necessary)
            - run in background on close and add to tray icon (not necessary)
            - queue of messages for offline  (not necessary)
            - send bye when exiting app (not necessary)
            - cancel download or send (not necessary)
            - add the ability to block peers (not necessary)
            - split page in multiple views (not necessary)
            - add the ability to talk to yourself (not necessary)
            - loading sent received seen logic (not necessary)

        NICE TO HAVE:
            - add to the parser a bit in the metadata which says about the data if it is fragmented, if it is we have
              the implementation we got now else the packets of data will be just one and it will be just a stream of
              bytes (especially for the sending data protocol to make it faster)
            - the manager for file transfer should better handle and return errors not just a EOS for errors
            - redo the tests with a framework (not necessary)
            - switch to microsoft's json parser
*/
