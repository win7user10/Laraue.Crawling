﻿using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Static.AngleSharp;

/// <inheritdoc />
public class AngleSharpParser : BaseDocumentSchemaParser<IElement, HtmlSelector>
{
    /// <inheritdoc />
    public AngleSharpParser(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
    
    /// <inheritdoc />
    protected override Task<IElement?> GetElementAsync(IElement currentElement, HtmlSelector htmlSelector)
    {
        return Task.FromResult(currentElement?.QuerySelector(htmlSelector.Value));
    }

    /// <inheritdoc />
    protected override Task<IElement[]?> GetElementsAsync(IElement currentElement, HtmlSelector htmlSelector)
    {
        var result = currentElement?.QuerySelectorAll(htmlSelector.Value);

        return Task.FromResult(result?.ToArray());
    }
}