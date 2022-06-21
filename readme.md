# Laraue.Crawling packages

The set of tools for fast writing crawlers on .NET.

### Build schema

```cs
var schema = new HtmlSchemaBuilder<OnePage>()
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
```

#### And use this schema for the passed html

```
var htmlParser = new HtmlParser();
var visitor = new AngleSharpParser(htmlParser);

var html = File.ReadAllText("test.html");
var model = visitor.VisitSchema(schema, html);

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
```