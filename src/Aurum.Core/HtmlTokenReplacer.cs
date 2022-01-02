using System.Text;
using HtmlAgilityPack;

namespace Aurum.Core;

public class HtmlTokenReplacer : ITokenReplacer
{
    public void ReplaceTokens(Stream input, Stream output, Dictionary<string, string> tokenValues)
    {
        if (!input.CanRead)
            throw new ArgumentException("Cannot read from input stream");

        if (!output.CanWrite || !output.CanSeek)
            throw new ArgumentException("Cannot write to output stream");
        
        var document = new HtmlDocument();
        document.Load(input);

        ValidateDocument(document);

        if (tokenValues.Any())
        {
            var descendantNodes = document.DocumentNode.SelectNodes(BuildXPathExpression(tokenValues.Keys.ToArray()));
            foreach (var node in descendantNodes.ToList())
            {
                var textNode = (HtmlTextNode)node;
                var text = System.Web.HttpUtility.HtmlDecode(textNode.Text);
                var (key, value) = tokenValues.First(pair => text.Contains(pair.Key));
                textNode.Text = text.Replace(key, value);
            }
        }

        output.Seek(0L, SeekOrigin.Begin);
        document.Save(output);
        output.Seek(0L, SeekOrigin.Begin);
    }

    private static string BuildXPathExpression(IReadOnlyList<string> tokens)
    {
        var sb = new StringBuilder();
        sb.Append("//text()[contains(.,");
        for (var i = 0; i < tokens.Count; i++)
        {
            sb.Append($"'{tokens[i]}'");
            sb.Append(')');

            if (i != tokens.Count - 1)
                sb.Append(" or contains(.,");
        }

        sb.Append(']');
        return sb.ToString();
    }

    private static void ValidateDocument(HtmlDocument document)
    {
        if(!document.ParseErrors.Any())
            return;

        var exceptions = document.ParseErrors.Select(error => new Exception(error.Reason)).ToList();
        if (exceptions.Count == 1)
            throw exceptions[0];

        throw new AggregateException("Multiple errors parsing document", exceptions);
    }
    
}