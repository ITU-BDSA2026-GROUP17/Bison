// See https://aka.ms/new-console-template for more information

namespace Bison.CLI.Client
{
    public class Program
    {
        static readonly UserInterface UserInterface = new(
            "data/bison_observation_db.csv",
            "data/bison_comment_db.csv",
            "data/observation_id.txt"
        );

        static int Main(string[] args)
        {
            var root = UserInterface.GetRootCommand();

            var result = root.Parse(args);
            return result.Invoke();
        }
    }
}
