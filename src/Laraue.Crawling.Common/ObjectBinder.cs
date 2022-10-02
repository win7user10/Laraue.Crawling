using System.Collections.Concurrent;
using System.Linq.Expressions;
using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common;

public class ObjectBinder<T> : ObjectBinder, IObjectBinder<T>
{
    public ObjectBinder(T instance) : base(instance)
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
    
    public ObjectBinder(object? instance)
    {
        _instance = instance;
    }
    
    public void BindProperty(LambdaExpression selector, object? value)
    {
        var propertyInfo = Helper.GetParsingProperty(selector);

        if (_instance is null)
        {
            return;
        }
        
        propertyInfo.SetValue(_instance, value);
    }
}

public sealed class ObjectBinderFactory
{
    private static readonly ConcurrentDictionary<Type, Type> BinderTypes = new ();

    public static IObjectBinder ForObject(object objectInstance)
    {
        var objectType = objectInstance.GetType();
        
        if (!BinderTypes.TryGetValue(objectType, out var binderType))
        {
            var objectBinderType = typeof(ObjectBinder<>);
            var typeArgs = new []{ objectInstance.GetType() };
            binderType = objectBinderType.MakeGenericType(typeArgs);
            BinderTypes.TryAdd(objectType, binderType);
        }
        
        return (IObjectBinder)Activator.CreateInstance(binderType, objectInstance)!;
    }
}