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
}