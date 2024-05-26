namespace Laraue.Crawling.Abstractions;

/// <summary>
/// Adapter that describe how to get some objects from the Node.
/// </summary>
/// <typeparam name="TNode"></typeparam>
public interface ICrawlingAdapter<in TNode>
{
    /// <summary>
    /// Get <see cref="TDestination"/> object from the string.
    /// </summary>
    /// <param name="element"></param>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public TDestination? MapValue<TDestination>(string? element);
    
    /// <summary>
    /// Get value of the passed element mapped to the passed type.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="destinationType"></param>
    /// <returns></returns>
    public Task<object?> GetValueAsync(TNode? element, Type destinationType);
    
    /// <summary>
    /// Returns Node inner text.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public Task<string?> GetInnerTextAsync(TNode? element);
    
    /// <summary>
    /// Returns node attribute.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public Task<string?> GetAttributeTextAsync(TNode? element, string attributeName);
}