namespace Bison
{
    public sealed class UserInterface
    {
        private static UserInterface? instance = null;

        private UserInterface()
        {
        }

        public static UserInterface GetInstance()
        {
            instance ??= new();

            return instance;
        }

        public static void PrintObservations<T>(IEnumerable<T> observations)
        {
            foreach (var observation in observations)
            {
                Console.WriteLine(observation);
            }
        }
    }
}
