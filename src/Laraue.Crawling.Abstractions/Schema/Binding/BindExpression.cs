namespace Laraue.Crawling.Abstractions.Schema.Binding;

/// <summary>
/// Represents an expression for bind value to the property of the specified type.
/// </summary>
/// <typeparam name="TElement">The type of tree node.</typeparam>
/// <typeparam name="TSelector"></typeparam>
public abstract class BindExpression<TElement, TSelector> : SchemaExpression<TElement>
    where TSelector : Selector
{
    /// <summary>
    /// The type of object that this expression is bind.
    /// </summary>
    public Type ObjectType { get; }
    
    /// <summary>
    /// The delegate is used to bind properties of the object.
    /// </summary>
    public SetPropertyInfo? SetPropertyInfo { get; }
    
    /// <summary>
    /// If html selector is set, then it will be applied to the current element
    /// with html before execute binding.
    /// </summary>
    public TSelector? Selector { get; }
    
    protected BindExpression(Type objectType, SetPropertyInfo? setPropertyInfo, TSelector? selector)
    {
        ObjectType = objectType;
        SetPropertyInfo = setPropertyInfo;
        Selector = selector;
    }
}