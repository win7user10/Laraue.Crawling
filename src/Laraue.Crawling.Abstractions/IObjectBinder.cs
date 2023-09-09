using System.Linq.Expressions;

namespace Laraue.Crawling.Abstractions;

/// <summary>
/// The class allows to bind some property of the object.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IObjectBinder<T>
{
    /// <summary>
    /// Sets the value to the specified property. Only <see cref="MemberExpression"/> is supported.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="value"></param>
    /// <typeparam name="TProperty"></typeparam>
    public void BindProperty<TProperty>(Expression<Func<T, TProperty?>> selector, TProperty? value);

    /// <summary>
    /// Await the task and sets the value to the specified property. Only <see cref="MemberExpression"/> is supported.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="getValueTask"></param>
    /// <typeparam name="TProperty"></typeparam>
    /// <returns></returns>
    public Task BindPropertyAsync<TProperty>(Expression<Func<T, TProperty?>> selector, Task<TProperty?> getValueTask);
    
    /// <summary>
    /// Gets the current value of the model using the specified selector.
    /// </summary>
    /// <param name="selector"></param>
    /// <typeparam name="TProperty"></typeparam>
    /// <returns></returns>
    public TProperty GetProperty<TProperty>(Func<T, TProperty> selector);
}

public interface IObjectBinder
{
    public void BindProperty(LambdaExpression selector, object? value);
}