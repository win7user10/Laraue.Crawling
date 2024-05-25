using System.Diagnostics.CodeAnalysis;
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
        var propertyExpression = schemaProperty.Body as MemberExpression;
        if (propertyExpression is null)
        {
            if (schemaProperty.Body is UnaryExpression unaryExpression)
            {
                propertyExpression = unaryExpression.Operand as MemberExpression;
            }
        }

        if (propertyExpression is null)
        {
            throw new InvalidOperationException($"Expression {schemaProperty} is not supported.");
        }

        var property = propertyExpression.Member as PropertyInfo;
        return property ?? throw new NotImplementedException();
    }

    public static bool TryGetArrayDefinition(Type type, [NotNullWhen(true)] out Type? arrayType)
    {
        if (type == typeof(string))
        {
            arrayType = null;
            return false;
        }
        
        if (type is { IsInterface: true, IsGenericType: true }
            && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            arrayType = type.GetGenericArguments()[0];
            return true;
        }
        
        foreach(var iType in type.GetInterfaces())
        {
            if (!iType.IsGenericType || iType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
            {
                continue;
            }
            
            arrayType = iType.GetGenericArguments()[0];
            return true;
        }

        arrayType = null;
        return false;
    }
}