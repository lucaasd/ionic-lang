using System.Text.RegularExpressions;

namespace VM.AssemblyInfo.Validation;

public static class DescriptorParser
{
    public static Descriptor? Parse(string descriptorString)
    {
        var descriptorRegex = @"[a-zA-Z_]([a-z A-Z 0-9 _]+)*";
        string[] argumentTypes = [];
        var returnType = "";
        var matches = Regex.Matches(descriptorString, descriptorRegex);
        if (matches.Count != 2)
        {
            return null;
        }
        argumentTypes = matches[0].Value.ToCharArray().Select(c => c.ToString()).ToArray();
        returnType = matches[1].Value;
        var descriptor = new Descriptor([.. argumentTypes], returnType);
        return descriptor;
    }
}