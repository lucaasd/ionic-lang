namespace VM.AssemblyInfo.Validation.Tests;

public class DescriptorParserTest
{
    [Test]
    public void ParseTest()
    {
        var descriptor = "(II)B";
        var result = DescriptorParser.Parse(descriptor);
        string[] expectedArgumentTypes = ["I", "I"];
        Assert.That(result, Is.Not.Null);
        Assert.That(result?.ArgumentTypes, Is.EqualTo(expectedArgumentTypes));
        Assert.That(result?.returnType, Is.EqualTo("B"));
    }
}