using System.Linq.Expressions;

namespace Laraue.Crawling.Static.Abstractions;

public interface IHtmlSchemaBuilder<TModel>
{
    IHtmlSchemaBuilder<TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IHtmlElement?, TValue?> mapFunction);
    
    IHtmlSchemaBuilder<TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IHtmlSchemaBuilder<TValue>> childBuilder);
    
    IHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IHtmlElement?, TValue?> mapFunction);
    
    IHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IHtmlSchemaBuilder<TValue>> childBuilder);

    ICompiledHtmlSchema<TModel> Build();
}