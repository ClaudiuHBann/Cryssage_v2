using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Test
{
abstract partial class ITests
{
    static uint Count = 0;

    protected Queue<Func<bool>> Modules = new();

    protected virtual bool TestModules()
    {
        var thisName = ToString();
        if (thisName != null)
        {
            thisName = thisName.Replace("Test.", "");
        }
        else
        {
            thisName = "?";
        }

        if (Count++ > 0)
        {
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"\tPreparing {thisName} Test Modules...");
        Console.WriteLine($"\tStarting {thisName} Test Modules...");

        while (Modules.Count > 0)
        {
            var test = Modules.Dequeue();
            if (!test())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\t{test.Method.Name.Replace("Test.", "")} Test Module FAILED...");
                Console.ForegroundColor = ConsoleColor.White;

                return false;
            }
        }

        Console.WriteLine($"\tEnding {thisName} Test Modules...");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\t{thisName} Test Modules PASSED...");
        Console.ForegroundColor = ConsoleColor.White;

        return true;
    }

    [GeneratedRegex("\\t|\\n|\\r")]
    private static partial Regex RegexArgumentExpressionWithoutNewLines();

    [GeneratedRegex(@"\s+")]
    private static partial Regex RegexArgumentExpressionOneContinuouslySpace();

    protected static bool PrintModuleTest(bool expression,
                                          [CallerArgumentExpression("expression")] string? argumentExpression = null,
                                          [CallerFilePath] string? filePath = null,
                                          [CallerLineNumber] int lineNumber = 0)
    {
        if (!expression)
        {
            string argumentExpressionNew = "argumentExpression";
            if (argumentExpression != null)
            {
                argumentExpressionNew = RegexArgumentExpressionWithoutNewLines().Replace(argumentExpression, "");
                argumentExpressionNew =
                    RegexArgumentExpressionOneContinuouslySpace().Replace(argumentExpressionNew, " ");
            }

            /*Console.WriteLine($"\t\t{filePath}:{lineNumber}\t {argumentExpressionNew} -> {expression}");*/
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\t\t{argumentExpressionNew} failed...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        return !expression;
    }

    public abstract bool Test();
}
}
