// See https://aka.ms/new-console-template for more information

namespace Bison.CLI.Client
{
    public class Program
    {
        static int Main(string[] args)
        {
            var root = UserInterface.GetRootCommand();

            var result = root.Parse(args);
            return result.Invoke();
        }
    }
}
