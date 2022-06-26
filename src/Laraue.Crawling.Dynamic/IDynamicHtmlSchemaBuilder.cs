using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Dynamic;

public interface IDynamicHtmlSchemaBuilder<TModel, TPage>
{
    IDynamicHtmlSchemaBuilder<TModel, TPage> ExecuteAsync(
        Func<TPage, Task> function);
    
    IDynamicHtmlSchemaBuilder<TModel, TPage> ParseArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IDynamicHtmlSchemaBuilder<TValue, TPage>> childBuilder);
    
    IDynamicHtmlSchemaBuilder<TModel, TPage> ParseProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IHtmlElement?, TValue?> mapFunction);
    
    IDynamicHtmlSchemaBuilder<TModel, TPage> ParseProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IDynamicHtmlSchemaBuilder<TModel, TPage>> childBuilder);

    ICompiledDynamicHtmlSchema<TModel, TPage> Build();
}