using System.Linq.Expressions;
using System.Reflection;
using Laraue.Crawling.Abstractions;

namespace Laraue.Crawling.Common;

public class ObjectBinder<T> : IObjectBinder<T>
{
    private readonly T _instance;

    public ObjectBinder(T instance)
    {
        _instance = instance;
    }

    public void BindProperty<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value)
    {
        var propertyInfo = Helper.GetParsingProperty(selector);

        propertyInfo.SetValue(_instance, value);
    }
}