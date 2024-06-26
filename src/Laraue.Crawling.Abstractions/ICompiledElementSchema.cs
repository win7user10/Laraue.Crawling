﻿namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Schema ready for the crawling.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TSelector"></typeparam>
/// <typeparam name="TElement"></typeparam>
public interface ICompiledElementSchema<TElement, TSelector, TModel>
    where TSelector : Selector
{
    /// <summary>
    /// Schema of the parsing for the current object.
    /// </summary>
    public ICompiledDocumentSchema<TElement, TSelector, GenericCrawlingModel<TModel>> ObjectSchema { get; }
}