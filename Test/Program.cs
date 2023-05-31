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
    *ONLY THE CLIENT SENDS DATA
    *ONLY THE SERVER RECEIVES DATA

    TODO:
         - make every context implements it's way of ToStream and FromStream
         - call the progress event only for GUI messages
         - add the progress event of a file in the manager of files
         - cannot send the same file twice
         - if we have the same file already we download it and append to the existing file
*/
