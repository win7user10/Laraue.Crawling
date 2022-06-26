using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Laraue.Crawling.Abstractions;

public class Helper
{
    public static object GetInstanceOfType(Type type)
    {
        return type.GetConstructor(Type.EmptyTypes) != null 
            ? Activator.CreateInstance(type)!
            : RuntimeHelpers.GetUninitializedObject(type);
    }
    
    public static PropertyInfo GetParsingProperty<TModel, TValue>(Expression<Func<TModel, TValue>> schemaProperty)
    {
        if (schemaProperty.Body is not MemberExpression memberSelectorExpression)
        {
            throw new NotImplementedException();
        }
        
        var property = memberSelectorExpression.Member as PropertyInfo;
        return property ?? throw new NotImplementedException();
    }
}