namespace Laraue.Crawling.Abstractions.Schema.Binding;

public class BindObjectExpression<TElement, TSelector> : BindExpression<TElement, TSelector>
    where TSelector : Selector
{
    /// <summary>
    /// Array of expressions that used to bind 
    /// </summary>
    public SchemaExpression<TElement>[] ChildPropertiesBinders { get; }

    public BindObjectExpression(
        Type objectType,
        SetPropertyInfo? setPropertyInfo,
        TSelector? selector,
        SchemaExpression<TElement>[] childPropertiesBinders)
        : base(objectType, setPropertyInfo, selector)
    {
        ChildPropertiesBinders = childPropertiesBinders;
    }
}