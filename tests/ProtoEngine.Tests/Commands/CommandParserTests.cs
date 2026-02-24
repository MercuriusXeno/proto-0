using ProtoEngine.Commands;

namespace ProtoEngine.Tests.Commands;

public class CommandParserTests
{
    private readonly CommandParser _parser = new();

    [Fact]
    public void Parse_SimpleVerb_ReturnsVerbAndNoArgs()
    {
        var (verb, args) = _parser.Parse("look");

        Assert.Equal("look", verb);
        Assert.Empty(args);
    }

    [Fact]
    public void Parse_VerbWithOneArg_SplitsCorrectly()
    {
        var (verb, args) = _parser.Parse("take sword");

        Assert.Equal("take", verb);
        Assert.Single(args);
        Assert.Equal("sword", args[0]);
    }

    [Fact]
    public void Parse_VerbWithMultipleArgs_SplitsAll()
    {
        var (verb, args) = _parser.Parse("give sword merchant");

        Assert.Equal("give", verb);
        Assert.Equal(2, args.Length);
        Assert.Equal("sword", args[0]);
        Assert.Equal("merchant", args[1]);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsEmptyVerbAndNoArgs()
    {
        var (verb, args) = _parser.Parse("");

        Assert.Equal(string.Empty, verb);
        Assert.Empty(args);
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsEmptyVerbAndNoArgs()
    {
        var (verb, args) = _parser.Parse("   ");

        Assert.Equal(string.Empty, verb);
        Assert.Empty(args);
    }

    [Fact]
    public void Parse_LeadingWhitespace_TrimsCorrectly()
    {
        var (verb, args) = _parser.Parse("  look  ");

        Assert.Equal("look", verb);
        Assert.Empty(args);
    }

    [Fact]
    public void Parse_MultipleSpacesBetweenWords_HandlesCorrectly()
    {
        var (verb, args) = _parser.Parse("take   sword");

        Assert.Equal("take", verb);
        Assert.Single(args);
        Assert.Equal("sword", args[0]);
    }

    [Fact]
    public void Parse_MixedCase_NormalizesToLowercase()
    {
        var (verb, args) = _parser.Parse("LOOK");

        Assert.Equal("look", verb);
    }

    [Fact]
    public void Parse_VerbCaseNormalized_ArgsPreserveCase()
    {
        var (verb, args) = _parser.Parse("Take Sword");

        Assert.Equal("take", verb);
        Assert.Equal("Sword", args[0]);
    }
}
