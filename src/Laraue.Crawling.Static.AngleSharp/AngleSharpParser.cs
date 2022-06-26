using System.Runtime.CompilerServices;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Static.Abstractions;

namespace Laraue.Crawling.Static.AngleSharp;

public class AngleSharpParser : IStaticHtmlSchemaParser
{
    private readonly IHtmlParser _htmlParser;

    public AngleSharpParser(IHtmlParser htmlParser)
    {
        _htmlParser = htmlParser;
    }
    
    public TModel? Parse<TModel>(ICompiledStaticHtmlSchema<TModel> schema, string html)
    {
        var instance = Parse(schema.BindingExpression, _htmlParser.ParseDocument(html).Body);

        return (TModel?) instance;
    }

    private object? Parse(BindingExpression bindingExpression, IElement? document)
    {
        return bindingExpression switch
        {
            ComplexTypeBindingExpression complexType => Parse(complexType, document),
            ArrayBindingExpression arrayType => Parse(arrayType, document),
            SimpleTypeBindingExpression simpleType => Parse(simpleType, document),
            _ => throw new NotImplementedException()
        };
    }

    private object? Parse(ComplexTypeBindingExpression complexType, IElement? document)
    {
        var intermediateNode = complexType.HtmlSelector is not null
            ? document?.QuerySelector(complexType.HtmlSelector.Selector)
            : document;

        if (intermediateNode is null)
        {
            return null;
        }

        var objectInstance = Helper.GetInstanceOfType(complexType.ObjectType);

        foreach (var element in complexType.Elements)
        {
            var value = Parse(element, intermediateNode);
            element.PropertySetter(objectInstance, value);
        }

        return objectInstance;
    }
    
    private static object? Parse(SimpleTypeBindingExpression simpleType, IElement? document)
    {
        if (simpleType.HtmlSelector is not null)
        {
            document = document?.QuerySelector(simpleType.HtmlSelector.Selector);
        }
                    
        if (document?.InnerHtml is not null)
        {
            return simpleType.PropertyGetter(new AngleSharpHtmlElement(document));
        }

        return null;
    }
    
    private object? Parse(ArrayBindingExpression arrayType, IElement? document)
    {
        IHtmlCollection<IElement>? children = null;
        if (arrayType.HtmlSelector is not null)
        {
            children = document?.QuerySelectorAll(arrayType.HtmlSelector.Selector);
        }
        
        if (children is null)
        {
            return null;
        }

        var result = (object?[])Array.CreateInstance(arrayType.ObjectType, children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var value = Parse(arrayType.Element, child);
            result[i] = value;
        }
        
        return result;
    }
}