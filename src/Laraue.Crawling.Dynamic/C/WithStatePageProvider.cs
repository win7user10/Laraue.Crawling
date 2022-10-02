using System.Runtime.CompilerServices;

namespace Laraue.Crawling.Dynamic.C;

public abstract class WithStatePageProvider : IPagesProvider
{
    private ProviderState? _state;
    private readonly TimeSpan _waitForNextUrlsInStorageInterval = TimeSpan.FromSeconds(1);

    public async IAsyncEnumerable<Uri> GetPages([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Load state
        _state = await LoadStateAsync();

        if (_state == ProviderState.ProducingCompleted)
        {
            yield break;
        }

        await foreach (var pageUri in GetNotParsedPagesFromStorage(GetStorageEnumerator(), cancellationToken))
        {
            yield return pageUri;
        }
    }

    protected abstract IAsyncEnumerator<Uri?> GetStorageEnumerator();

    /// <summary>
    /// Returns stream from storage contains only uris to parse.
    /// </summary>
    /// <returns></returns>
    private async IAsyncEnumerable<Uri> GetNotParsedPagesFromStorage(
        IAsyncEnumerator<Uri?> storageEnumerator,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (_state != ProviderState.ProducingCompleted)
        {
            await storageEnumerator.MoveNextAsync();
            if (storageEnumerator.Current is not null)
            {
                yield return storageEnumerator.Current;
            }
            else
            {
                await Task.Delay(_waitForNextUrlsInStorageInterval, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Writes new found uri to the storage.
    /// </summary>
    /// <returns></returns>
    protected abstract Task WriteNewUriToStorageAsync(Uri uri);

    /// <summary>
    /// Parse links from the resource.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract IAsyncEnumerable<Uri> ParseUris(CancellationToken cancellationToken);

    /// <summary>
    /// Load state on the start.
    /// </summary>
    /// <returns></returns>
    protected abstract Task<ProviderState> LoadStateAsync();
}

public enum ProviderState
{
    Started,
    ParsingCompleted,
    ProducingCompleted,
}

// Menu links
// Save batch to file
// Yield return not returned

// How to restore state
// How to continue work
// Menu links
// 1. Read all links
// 2. Save to file
// 2.1 
// 3. If no links in file, run 1
// 4. If there are links in file, return Stream
// 5. Mark already parsed pages

// OnLoad - create read file stream if not exists
// Read stream, if state is not ready wait some time and continue read stream 