using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp.Extensions;

public static class AngleSharpParserExtensions
{
    private static readonly IHtmlParser HtmlParser = new HtmlParser();
    
    public static Task<TModel?> RunAsync<TModel>(
        this BaseDocumentSchemaParser<IElement, HtmlSelector> parser,
        ICompiledDocumentSchema<IElement, HtmlSelector, TModel> schema,
        string html)
    {
        var rootElement = HtmlParser.ParseDocument(html).Body;

        return parser.RunAsync(schema, rootElement);
    }
    
    public static Task<TModel?> RunAsync<TModel>(
        this BaseDocumentSchemaParser<IElement, HtmlSelector> parser,
        ICompiledElementSchema<IElement, HtmlSelector, TModel> schema,
        string html)
    {
        var rootElement = HtmlParser.ParseDocument(html).Body;

        return parser.RunAsync(schema, rootElement);
    }
}