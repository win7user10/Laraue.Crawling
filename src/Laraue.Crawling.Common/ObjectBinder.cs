using System.Collections.Concurrent;
using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Common;

public sealed class ObjectBinder<T> : ObjectBinder, IObjectBinder<T>
{
    private T TypedInstance => (T)Instance!;
    
    public ObjectBinder(T instance, ILoggerFactory loggerFactory, VisitorContext visitorContext)
        : base(instance, loggerFactory.CreateLogger<ObjectBinder<T>>(), visitorContext)
    {
    }

    /// <inheritdoc />
    public void BindProperty<TProperty>(Expression<Func<T, TProperty?>> selector, TProperty? value)
    {
        BindProperty((LambdaExpression)selector, value);
    }

    /// <inheritdoc />
    public async Task BindPropertyAsync<TProperty>(Expression<Func<T, TProperty?>> selector, Task<TProperty?> getValueTask)
    {
        try
        {
            var value = await getValueTask.ConfigureAwait(false);
            BindProperty((LambdaExpression)selector, value);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Bind {Property} failed", selector);
        }
    }

    /// <inheritdoc />
    public TProperty GetProperty<TProperty>(Func<T, TProperty> selector)
    {
        return selector(TypedInstance);
    }
}

public class ObjectBinder : IObjectBinder
{
    protected readonly object? Instance;
    protected readonly ILogger<ObjectBinder> Logger;
    private readonly VisitorContext _visitorContext;

    protected ObjectBinder(object? instance, ILogger<ObjectBinder> logger, VisitorContext visitorContext)
    {
        Instance = instance;
        Logger = logger;
        _visitorContext = visitorContext;
    }
    
    /// <inheritdoc />
    public void BindProperty(LambdaExpression selector, object? value)
    {
        var propertyInfo = Helper.GetParsingProperty(selector);

        if (Instance is null)
        {
            return;
        }

        var propertyContext = _visitorContext.Push(propertyInfo.Name);
        
        Logger.LogTrace("Bind Property: {Path} Value: {Value}", propertyContext, value);
        
        propertyInfo.SetValue(Instance, value);
    }
}

public sealed class ObjectBinderFactory
{
    private static readonly ConcurrentDictionary<Type, Type> BinderTypes = new ();

    public static IObjectBinder ForObject(object objectInstance, ILoggerFactory loggerFactory, VisitorContext context)
    {
        var objectType = objectInstance.GetType();
        
        if (!BinderTypes.TryGetValue(objectType, out var binderType))
        {
            var objectBinderType = typeof(ObjectBinder<>);
            var typeArgs = new []{ objectInstance.GetType() };
            binderType = objectBinderType.MakeGenericType(typeArgs);
            BinderTypes.TryAdd(objectType, binderType);
        }
        
        return (IObjectBinder)Activator.CreateInstance(binderType, objectInstance, loggerFactory, context)!;
    }
}