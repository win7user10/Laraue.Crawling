namespace Laraue.Crawling.Abstractions.Schema.Delegates;

public delegate Task<TValue?> GetValueDelegate<in TElement, TValue>(TElement element);

public delegate Task<object?> GetObjectValueDelegate<in TElement>(TElement element);