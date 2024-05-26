using System.IO;
using System.Threading.Tasks;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Static.AngleSharp;
using Laraue.Crawling.Static.AngleSharp.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Laraue.Crawling.Static.Tests;

public class AngleSharpParserTests
{
    [Fact]
    public async Task Schema_ShouldBeParsedCorrectlyAsync()
    {
        var schema = new AngleSharpSchemaBuilder<OnePage>()
            .HasProperty(x => x.Title, ".title")
            .HasObjectProperty(x => x.User, ".user", userBuilder =>
            {
                userBuilder.HasProperty(x => x.Name, ".name")
                    .HasProperty(x => x.Age, ".age")
                    .HasArrayProperty(x => x.Dogs, ".dog", dogsBuilder =>
                    {
                        dogsBuilder.HasProperty(x => x.Age, ".age")
                            .HasProperty(x => x.Name, ".name")
                            .BindManually((element, binder) =>
                            {
                                var age = binder.GetProperty(x => x.Age);
                                var name = binder.GetProperty(x => x.Name);
                                
                                binder.BindProperty(x => x.Identifier, $"{name}_{age}");
                                
                                return Task.CompletedTask;
                            });
                    });
            })
            .HasArrayProperty(
                x => x.ImageLinks,
                ".links a",
                x => Task.FromResult(x.GetAttributeValue("href")))
            .Build();

        var visitor = new AngleSharpParser(new NullLoggerFactory());

        var html = await File.ReadAllTextAsync("test.html");
        var model = await visitor.RunAsync(schema, html)!;
        
        Assert.Equal("Private info", model!.Title);
        Assert.Equal("Alex", model.User.Name);
        Assert.Equal(10, model.User.Age);

        var dogs = model.User.Dogs;
        Assert.Equal(2, dogs.Length);

        var dog1 = dogs[0];
        Assert.Equal(5, dog1.Age);
        Assert.Equal("Jelly", dog1.Name);
        Assert.Equal("Jelly_5", dog1.Identifier);
        
        var dog2 = dogs[1];
        Assert.Equal(7, dog2.Age);
        Assert.Equal("Marly", dog2.Name);
        Assert.Equal("Marly_7", dog2.Identifier);

        var links = model.ImageLinks;
        Assert.Equal(2, links.Length);
        Assert.Equal("https://hey1.html", links[0]);
        Assert.Equal("https://hey2.html", links[1]);
    }
    
    [Fact]
    public async Task ElementSchema_ShouldBeParsedCorrectly_WhenBindManuallyAsync()
    {
        var textContentSchema = new AngleSharpElementSchema<string>(builder => builder
            .GetValueFromElement(element => element?.QuerySelector(".title")!.InnerHtml));
        
        var value = await TestElementSchemaAsync(textContentSchema);
        
        Assert.Equal("Private info", value);
    }
    
    [Fact]
    public async Task ElementSchema_ShouldBeParsedCorrectly_WithDefaultGetterAsync()
    {
        var textContentSchema = new AngleSharpElementSchema<string>(builder => builder.UseSelector(".title"));
        
        var value = await TestElementSchemaAsync(textContentSchema);
        
        Assert.Equal("Private info", value);
    }
    
    [Fact]
    public async Task ElementSchema_ShouldBeParsedCorrectly_WithDefaultGetterAndNotStringTypeAsync()
    {
        var textContentSchema = new AngleSharpElementSchema<int>(builder => builder.UseSelector(".user .age"));
        
        var value = await TestElementSchemaAsync(textContentSchema);
        
        Assert.Equal(10, value);
    }
    
    [Fact]
    public async Task ElementSchema_ShouldBeParsedCorrectly_ForSimpleArrayTypeAsync()
    {
        var textContentSchema = new AngleSharpElementSchema<int[]>(builder => builder.UseSelector(".dog .age"));
        
        var values = await TestElementSchemaAsync(textContentSchema);
        
        Assert.Equal(2, values.Length);
        Assert.Equal(5, values[0]);
        Assert.Equal(7, values[1]);
    }

    private async Task<TElementSchema> TestElementSchemaAsync<TElementSchema>(AngleSharpElementSchema<TElementSchema> schema)
    {
        var parser = new AngleSharpParser(new NullLoggerFactory());

        var html = await File.ReadAllTextAsync("test.html");
        var value = await parser.RunAsync(schema, html)!;

        return value!;
    }
}

public record OnePage : ICrawlingModel
{
    public required string Title { get; init; }
    public required string[] ImageLinks { get; init; }
    public required User User { get; init; }
}

public record User(string Name, int Age, Dog[] Dogs) : ICrawlingModel;
public record Dog(string Name, int Age, string Identifier) : ICrawlingModel;