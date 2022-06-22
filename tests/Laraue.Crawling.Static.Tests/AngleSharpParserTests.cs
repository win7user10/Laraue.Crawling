using System.Collections.Generic;
using System.IO;
using AngleSharp.Html.Parser;
using Laraue.Crawling.Static.AngleSharp;
using Laraue.Crawling.Static.Impl;
using Xunit;

namespace Laraue.Crawling.Static.Tests;

public class AngleSharpParserTests
{
    [Fact]
    public void Scheme_ShouldBeParsedCorrectly()
    {
        var schema = new StaticHtmlSchemaBuilder<OnePage>()
            .HasProperty(x => x.Title, ".title")
            .HasProperty(x => x.User, ".user", userBuilder =>
            {
                userBuilder.HasProperty(x => x.Name, ".name")
                    .HasProperty(x => x.Age, ".age", x => int.Parse(x.GetInnerHtml()))
                    .HasArrayProperty(x => x.Dogs, ".dog", dogsBuilder =>
                    {
                        dogsBuilder.HasProperty(x => x.Age, ".age", x => int.Parse(x.GetInnerHtml()))
                            .HasProperty(x => x.Name, ".name");
                    });
            })
            .HasArrayProperty(x => x.ImageLinks, ".links a", x => x.GetAttribute("href"))
            .Build();

        var htmlParser = new HtmlParser();
        var visitor = new AngleSharpParser(htmlParser);

        var html = File.ReadAllText("test.html");
        var model = visitor.Parse(schema, html);
        
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