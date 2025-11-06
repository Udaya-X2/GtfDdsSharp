namespace GtfDdsSharp.Tests;

internal class TempFile : IDisposable
{
    private readonly string _name = Path.GetRandomFileName();

    public static implicit operator string(TempFile tempFile) => tempFile._name;

    public void Dispose()
    {
        try
        {
            File.Delete(_name);
        }
        catch
        {
        }
    }

    public override string ToString() => _name;
}
