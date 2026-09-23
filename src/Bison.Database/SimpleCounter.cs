namespace Bison.Database;

public sealed class SimpleCounter
{
    readonly string _filePath;

    public SimpleCounter(string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            var parent = Path.GetDirectoryName(filePath);
            ArgumentException.ThrowIfNullOrEmpty(parent);
            Directory.CreateDirectory(parent);
            File.Create(_filePath).Close();
            // creates the file; it's empty so NextNumber's int.Parse
            // will throw -> gets caught and returns 0
        }
        else
        {
            var input = File.ReadLines(_filePath);
            try
            {
                int.Parse(input.First());
            }
            catch
            {
                File.Create(_filePath).Close();
                // overrides the file; it's empty so NextNumber's int.Parse
                // will throw -> gets caught and returns 0
            }
        }
    }

    public int NextNumber()
    {
        var input = File.ReadLines(_filePath);
        try
        {
            var nextId = int.Parse(input.First()) + 1;

            using var writer = File.CreateText(_filePath);
            writer.Write(nextId);

            return nextId;
        }
        catch
        {
            using var writer = File.CreateText(_filePath);
            writer.Write('0');

            return 0;
        }
    }
}
