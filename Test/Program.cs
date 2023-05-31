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
         - make every context implements it's way of ToStream and FromStream
         - call the progress event only for GUI messages
         - add the progress event of a file in the manager of files
*/
