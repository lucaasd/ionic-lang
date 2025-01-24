using CommandLine;
using Launcher.CLI;
using VM.Code;
using VM.Configuration;
using VM.Instructions;
using VM.VirtualMachine;

namespace Launcher;

public class LauncherMain
{
    public static void Main(string[] args)
    {
        var parser = new Parser();
        var options = parser.ParseArguments<Options>(args).Value;
        var lexer = new Lexer(File.ReadAllText(options.Path) + "\0");
        lexer.Tokenize();
        Console.WriteLine(string.Join<Token>('\n', lexer.GetTokens()));
        var converter = new Converter(lexer.GetTokens());
        converter.Convert();
        var builder = VMBuilder.CreateBuilder()
            .ConfigureStdinStream(Console.OpenStandardInput())
            .ConfigureStdoutStream(Console.OpenStandardOutput())
            .ConfigureStderrStream(Console.OpenStandardError());

        var vm = IonicVM.CreateVM(builder);
        vm.LoadCode([.. converter.ByteCode]);
        vm.Run();
    }
}