﻿using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace Laraue.Crawling.Dynamic.PuppeterSharp.Utils;

/// <inheritdoc />
public sealed class BrowserFactory : IBrowserFactory
{
    private readonly SemaphoreSlim _semaphore = new (1);
    private bool _isInitialized;
    private IBrowser? _browserInstance;
    private readonly LaunchOptions _launchOptions;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of <see cref="BrowserFactory"/>.
    /// </summary>
    /// <param name="launchOptions"></param>
    /// <param name="loggerFactory"></param>
    public BrowserFactory(LaunchOptions launchOptions, ILoggerFactory loggerFactory)
    {
        _launchOptions = launchOptions;
        _loggerFactory = loggerFactory;
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
                if (!_browserInstance.IsClosed)
                {
                    return _browserInstance;
                }
                
                await _browserInstance.DisposeAsync().ConfigureAwait(false);
                _browserInstance = await GetBrowserInstanceAsync().ConfigureAwait(false);

                return _browserInstance;
            }
            
            _browserInstance = await GetBrowserInstanceAsync().ConfigureAwait(false);

            return _browserInstance;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private Task<IBrowser> GetBrowserInstanceAsync()
    {
        return Puppeteer.LaunchAsync(_launchOptions, _loggerFactory);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _semaphore.Dispose();
        _browserInstance?.Dispose();
    }
}