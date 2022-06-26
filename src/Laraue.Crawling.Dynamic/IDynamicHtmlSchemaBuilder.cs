using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic;

public interface IDynamicHtmlSchemaBuilder<TModel, TPage, TElement>
{
    IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ExecuteAsync(
        Func<TPage, Task> function);
    
    IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ParseArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IDynamicHtmlSchemaBuilder<TValue, TPage, TElement>> childBuilder);
    
    IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ParseProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<TElement?, Task<TValue?>> mapFunction);
    
    IDynamicHtmlSchemaBuilder<TModel, TPage, TElement> ParseProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IDynamicHtmlSchemaBuilder<TModel, TPage, TElement>> childBuilder);

    ICompiledDynamicHtmlSchema<TModel, TPage, TElement> Build();
}