namespace VM.Configuration;

public class VMBuilder
{
    public static VMBuilder CreateBuilder()
    {
        return new VMBuilder();
    }

    private Stream? stdoutStream;
    private Stream? stderrStream;
    private Stream? stdinStream;

    public Stream? StdoutStream => stdoutStream;
    public Stream? StderrStream => stderrStream;
    public Stream? StdinStream => stdinStream;

    public VMBuilder ConfigureStdoutStream(Stream stream)
    {
        stdoutStream = stream;
        return this;
    }

    public VMBuilder ConfigureStderrStream(Stream stream)
    {
        stderrStream = stream;
        return this;
    }

    public VMBuilder ConfigureStdinStream(Stream stream)
    {
        stdinStream = stream;
        return this;
    }
}