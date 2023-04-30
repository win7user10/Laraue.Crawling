using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp.Utils;

/// <inheritdoc />
public sealed class BrowserFactory : IBrowserFactory
{
    private readonly SemaphoreSlim _semaphore = new (1);
    private bool _isInitialized;
    private IBrowser? _browserInstance;
    private readonly LaunchOptions _launchOptions;

    /// <summary>
    /// Initializes a new instance of <see cref="BrowserFactory"/>.
    /// </summary>
    /// <param name="launchOptions"></param>
    public BrowserFactory(LaunchOptions launchOptions)
    {
        _launchOptions = launchOptions;
    }
    
    /// <inheritdoc />
    public async ValueTask<IBrowser> GetInstanceAsync()
    {
        try
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            if (!_isInitialized)
            {
                await new BrowserFetcher().DownloadAsync().ConfigureAwait(false);

                _isInitialized = true;
            }

            if (_browserInstance is not null)
            {
                return _browserInstance;
            }
            
            _browserInstance = await Puppeteer.LaunchAsync(_launchOptions).ConfigureAwait(false);

            return _browserInstance;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}