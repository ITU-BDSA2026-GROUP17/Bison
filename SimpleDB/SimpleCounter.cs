namespace Bison.SimpleDB;

public sealed class SimpleCounter
{
    readonly string _filePath;

    public SimpleCounter(string filePath)
    {
        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            using var writer = File.CreateText(_filePath);
            writer.Write('-');
            writer.Write('1');
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
                using var writer = File.CreateText(_filePath);
                writer.Write('-');
                writer.Write('1');
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
