using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common.Impl;

public abstract class ElementSchema<TElement, TSelector, TModel> : ICompiledElementSchema<TElement, TSelector, TModel>
    where TSelector : Selector
{
    public ICompiledDocumentSchema<TElement, TSelector, GenericCrawlingModel<TModel>> ObjectSchema { get; }

    protected ElementSchema(
        DocumentSchemaBuilder<TElement, TSelector, GenericCrawlingModel<TModel>> schemaBuilder,
        Action<PropertyBuilder<TElement, TSelector, GenericCrawlingModel<TModel>, TModel>> propertyBuilder)
    {
        schemaBuilder.HasProperty(x => x.Value, propertyBuilder);

        ObjectSchema = schemaBuilder.Build();
    }
}