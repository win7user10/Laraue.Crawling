using System.Text.Json;

namespace Laraue.Crawling.Dynamic.C;

public class FileStatePageProvider : WithStatePageProvider
{
    private readonly string _stateFilePath;
    private readonly string _urlsFilePath;

    public FileStatePageProvider(string stateFilePath, string urlsFilePath)
    {
        _stateFilePath = stateFilePath;
        _urlsFilePath = urlsFilePath;

        File.Create(urlsFilePath);
    }
    
    protected override IAsyncEnumerator<Uri?> GetStorageEnumerator()
    {
        var fileStream = File.OpenRead(_urlsFilePath);

        return new FileEnumerator(fileStream);
    }

    protected override async Task WriteNewUriToStorageAsync(Uri uri)
    {
        await using var stream = File.OpenWrite(_urlsFilePath);
        var streamWriter = new StreamWriter(stream);

        await streamWriter.WriteLineAsync(uri.ToString());
    }

    // TODO - this method should contain base logic to change state of the crawling.
    protected override IAsyncEnumerable<Uri> ParseUris(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override async Task<ProviderState> LoadStateAsync()
    {
        if (!File.Exists(_stateFilePath))
        {
            return ProviderState.Started;
        }

        var fileStream = File.OpenRead(_stateFilePath);
        return await JsonSerializer.DeserializeAsync<ProviderState>(fileStream);
    }

    private class FileEnumerator : IAsyncEnumerator<Uri?>
    {
        private readonly FileStream _fileStream;
        private readonly StreamReader _streamReader;
        
        public FileEnumerator(FileStream fileStream)
        {
            _fileStream = fileStream;
            _streamReader = new StreamReader(fileStream);
        }
        
        public ValueTask DisposeAsync()
        {
            return _fileStream.DisposeAsync();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_streamReader.EndOfStream)
            {
                return false;
            }

            var nextLine = await _streamReader.ReadLineAsync();
            if (nextLine is null)
            {
                return false;
            }
            
            Current = new Uri(nextLine);
            return true;
        }

        public Uri? Current { get; private set; }
    }
}