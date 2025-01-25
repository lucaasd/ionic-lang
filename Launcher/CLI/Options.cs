using System.ComponentModel.DataAnnotations;
using CommandLine;

namespace Launcher.CLI;

public class Options
{
    [Value(0, Required = true)]
    public string Path { get; set; } = "";
}