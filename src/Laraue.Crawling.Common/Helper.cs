using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Laraue.Crawling.Common;

public class Helper
{
    /// <summary>
    /// Initializes an instance with the empty constructor or get the uninitialized object. 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object GetInstanceOfType(Type type)
    {
        return type.GetConstructor(Type.EmptyTypes) != null 
            ? Activator.CreateInstance(type)!
            : RuntimeHelpers.GetUninitializedObject(type);
    }
    
    /// <summary>
    /// Returns property name of the passed <see cref="MemberExpression"/>.
    /// </summary>
    /// <param name="schemaProperty"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static PropertyInfo GetParsingProperty(LambdaExpression schemaProperty)
    {
        if (schemaProperty.Body is not MemberExpression memberSelectorExpression)
        {
            throw new NotImplementedException();
        }
        
        var property = memberSelectorExpression.Member as PropertyInfo;
        return property ?? throw new NotImplementedException();
    }
}