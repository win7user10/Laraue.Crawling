using System.Reflection;
using Laraue.Crawling.Abstractions.Schema.Delegates;

namespace Laraue.Crawling.Abstractions.Schema.Binding;

public record SetPropertyInfo(SetPropertyDelegate SetPropertyDelegate, PropertyInfo PropertyInfo)
{
}