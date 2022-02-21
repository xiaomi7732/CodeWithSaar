namespace LearnDisposable;

class MyTextWriterV2 : MyTextWriter
{
    public MyTextWriterV2(string fileName) : base(Path.ChangeExtension(fileName, ".v2.txt"))
    {
    }

    protected override void Disposing()
    {
        base.Disposing();
        // Dispose my own unmanaged resources.
    }
}