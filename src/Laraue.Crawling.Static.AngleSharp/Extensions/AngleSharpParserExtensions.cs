using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Static.AngleSharp.Extensions;

public static class AngleSharpParserExtensions
{
    private static readonly IHtmlParser HtmlParser = new HtmlParser();
    
    public static Task<TModel?> RunAsync<TModel>(
        this AngleSharpParser parser,
        ICompiledHtmlSchema<IElement, TModel> schema,
        string html)
    {
        var rootElement = HtmlParser.ParseDocument(html).Body;

        return parser.RunAsync(schema, rootElement);
    }
}