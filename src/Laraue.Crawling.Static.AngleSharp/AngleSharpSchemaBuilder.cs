﻿using AngleSharp.Dom;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;

namespace Laraue.Crawling.Static.AngleSharp;

public class AngleSharpSchemaBuilder<TModel> : DocumentSchemaBuilder<IElement, HtmlSelector, TModel>
{
}