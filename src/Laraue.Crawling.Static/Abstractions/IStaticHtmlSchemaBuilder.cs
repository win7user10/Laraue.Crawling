using System.Linq.Expressions;

namespace Laraue.Crawling.Static.Abstractions;

/// <summary>
/// Builder for the <see cref="ICompiledStaticHtmlSchema{TModel}"/>.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IStaticHtmlSchemaBuilder<TModel>
{
    IStaticHtmlSchemaBuilder<TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IHtmlElement?, TValue?> mapFunction);
    
    IStaticHtmlSchemaBuilder<TModel> HasProperty<TValue>(
        Expression<Func<TModel, TValue>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IStaticHtmlSchemaBuilder<TValue>> childBuilder);
    
    IStaticHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        Func<IHtmlElement?, TValue?> mapFunction);
    
    IStaticHtmlSchemaBuilder<TModel> HasArrayProperty<TValue>(
        Expression<Func<TModel, TValue[]>> schemaProperty,
        HtmlSelector htmlSelector,
        Action<IStaticHtmlSchemaBuilder<TValue>> childBuilder);

    ICompiledStaticHtmlSchema<TModel> Build();
}