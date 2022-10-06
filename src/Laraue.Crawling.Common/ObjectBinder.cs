using System.Collections.Concurrent;
using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Common.Impl;
using Microsoft.Extensions.Logging;

namespace Laraue.Crawling.Common;

public class ObjectBinder<T> : ObjectBinder, IObjectBinder<T>
{
    public ObjectBinder(T instance, ILoggerFactory loggerFactory, VisitorContext visitorContext)
        : base(instance, loggerFactory.CreateLogger<ObjectBinder<T>>(), visitorContext)
    {
    }

    public void BindProperty<TProperty>(Expression<Func<T, TProperty?>> selector, TProperty? value)
    {
        BindProperty((LambdaExpression)selector, value);
    }
}

public class ObjectBinder : IObjectBinder
{
    private readonly object? _instance;
    private readonly ILogger<ObjectBinder> _logger;
    private readonly VisitorContext _visitorContext;

    public ObjectBinder(object? instance, ILogger<ObjectBinder> logger, VisitorContext visitorContext)
    {
        _instance = instance;
        _logger = logger;
        _visitorContext = visitorContext;
    }
    
    public void BindProperty(LambdaExpression selector, object? value)
    {
        var propertyInfo = Helper.GetParsingProperty(selector);

        if (_instance is null)
        {
            return;
        }

        var propertyContext = _visitorContext.Push(propertyInfo.Name);
        
        _logger.LogTrace("Bind Property: {Path} Value: {Value}", propertyContext, value);
        
        propertyInfo.SetValue(_instance, value);
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