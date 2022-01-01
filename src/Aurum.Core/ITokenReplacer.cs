namespace Aurum.Core;

public interface ITokenReplacer
{
    void ReplaceTokens(Stream input, Stream output, Dictionary<string, string> tokenValues);
}