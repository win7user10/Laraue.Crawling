using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common.Impl;

public abstract class ElementSchema<TElement, TSelector, TModel> : ICompiledElementSchema<TElement, TSelector, TModel>
    where TSelector : Selector
{
    public ICompiledDocumentSchema<TElement, TSelector, GenericResponse<TModel>> ObjectSchema { get; }

    protected ElementSchema(
        DocumentSchemaBuilder<TElement, TSelector, GenericResponse<TModel>> schemaBuilder,
        Action<PropertyBuilder<TElement, TSelector, GenericResponse<TModel>, TModel>> propertyBuilder)
    {
        schemaBuilder.HasProperty(x => x.Value, propertyBuilder);

        ObjectSchema = schemaBuilder.Build();
    }
}