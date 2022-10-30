namespace Laraue.Crawling.Abstractions.Schema.Delegates;

/// <summary>
/// Strongly-typed expression to return <see cref="TValue"/>
/// based on the passed <see cref="TElement"/>.
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TValue"></typeparam>
public delegate Task<TValue?> GetValueDelegate<in TElement, TValue>(TElement element);

/// <summary>
/// Expression to return object value
/// based on the passed <see cref="TElement"/>.
/// </summary>
/// <typeparam name="TElement"></typeparam>
public delegate Task<object?> GetObjectValueDelegate<in TElement>(TElement element);