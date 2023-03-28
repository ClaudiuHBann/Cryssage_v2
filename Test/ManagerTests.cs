namespace Test
{
class ManagerTests
{
    Queue<ITests> Tests = new();

    public ManagerTests()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Preparing Tests...");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void AddTest(ITests test)
    {
        Tests.Enqueue(test);
    }

    public bool Run()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Starting Tests...\n");

        while (Tests.Count > 0)
        {
            var test = Tests.Dequeue();
            if (!test.Test())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{test} FAILED...");
                Console.WriteLine("Tests FAILED...");
                Console.ForegroundColor = ConsoleColor.White;

                return false;
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("\nEnding Tests...");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Tests PASSED...");
        Console.ForegroundColor = ConsoleColor.White;

        return true;
    }
}
}
