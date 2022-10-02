using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Abstractions.Schema.Binding;

/// <summary>
/// Represents an expression for bind value to the property of the specified type.
/// </summary>
/// <typeparam name="TElement">The type of html provider.</typeparam>
public abstract class BindExpression<TElement> : SchemaExpression<TElement>
{
    /// <summary>
    /// The type of object that this expression is bind.
    /// </summary>
    public Type ObjectType { get; }
    
    /// <summary>
    /// The delegate is used to bind properties of the object.
    /// </summary>
    public SetPropertyDelegate? PropertySetter { get; }
    
    /// <summary>
    /// If html selector is set, then it will be applied to the current element
    /// with html before execute binding.
    /// </summary>
    public HtmlSelector? HtmlSelector { get; }
    
    protected BindExpression(Type objectType, SetPropertyDelegate? propertySetter, HtmlSelector? htmlSelector)
    {
        ObjectType = objectType;
        PropertySetter = propertySetter;
        HtmlSelector = htmlSelector;
    }
}