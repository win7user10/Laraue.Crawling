using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Laraue.Crawling.Static.AngleSharp;
using Laraue.Crawling.Static.AngleSharp.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Laraue.Crawling.Static.Tests;

public class AngleSharpParserTests
{
    [Fact]
    public async Task Scheme_ShouldBeParsedCorrectly()
    {
        var schema = new AngleSharpSchemaBuilder<OnePage>()
            .HasProperty(x => x.Title, ".title")
            .HasProperty(x => x.User, ".user", userBuilder =>
            {
                userBuilder.HasProperty(x => x.Name, ".name")
                    .HasProperty(x => x.Age, ".age")
                    .HasArrayProperty(x => x.Dogs, ".dog", dogsBuilder =>
                    {
                        dogsBuilder.HasProperty(x => x.Age, ".age")
                            .HasProperty(x => x.Name, ".name");
                    });
            })
            .HasArrayProperty(
                x => x.ImageLinks,
                ".links a",
                x => Task.FromResult(x.GetAttributeValue("href")))
            .Build();

        var visitor = new AngleSharpParser(new NullLoggerFactory());

        var html = await File.ReadAllTextAsync("test.html");
        var model = await visitor.RunAsync(schema, html);
        
        Assert.Equal("Private info", model.Title);
        Assert.Equal("Alex", model.User.Name);
        Assert.Equal(10, model.User.Age);

        var dogs = model.User.Dogs;
        Assert.Equal(2, dogs.Length);

        var dog1 = dogs[0];
        Assert.Equal(5, dog1.Age);
        Assert.Equal("Jelly", dog1.Name);
        
        var dog2 = dogs[1];
        Assert.Equal(7, dog2.Age);
        Assert.Equal("Marly", dog2.Name);

        var links = model.ImageLinks;
        Assert.Equal(2, links.Length);
        Assert.Equal("https://hey1.html", links[0]);
        Assert.Equal("https://hey2.html", links[1]);
    }
}

public record OnePage
{
    public string Title { get; init; }
    public string[] ImageLinks { get; init; }
    public User User { get; init; }
}

public record User(string Name, int Age, Dog[] Dogs, IDictionary<string, int> DogAge);
public record Dog(string Name, int Age);