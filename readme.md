# Laraue.Crawling packages

The set of tools for fast writing crawlers on .NET.


[![latest version](https://img.shields.io/nuget/v/Laraue.Crawling.Common)](https://www.nuget.org/packages/Laraue.Crawling.Common)
[![latest version](https://img.shields.io/nuget/dt/Laraue.Crawling.Common)](https://www.nuget.org/packages/Laraue.Crawling.Common)

### Static HTML crawling

Static means the crawling process is performing with the static html that not changes.
You can build a strongly typed schema with binding each element to related html block.
Then this schema can be parsed via AngleSharpParser class located in Laraue.Crawling.Static.AngleSharp library.

#### Build static HTML schema

```html
<div>
    <div class="title">Private info</div>
    <div class="user">
        <div class="name">Alex</div>
        <div class="age">10</div>
        <div class="dogs">
            <div class="dog">
                <div class="name">Jelly</div>
                <div class="age">5</div>
            </div>
            <div class="dog">
                <div class="name">Marly</div>
                <div class="age">7</div>
            </div>
        </div>
    </div>
    <div class="links">
        <a href="https://hey1.html"></a>
        <a href="https://hey2.html"></a>
    </div>
</div>
```

```csharp
public record OnePage(string Title, string[] ImageLinks, User User);
public record User(string Name, int Age, Dog[] Dogs);
public record Dog(string Name, int Age);


 var schema = new AngleSharpSchemaBuilder<OnePage>()
    .HasProperty(x => x.Title, ".title")
    .HasObjectProperty(x => x.User, ".user", userBuilder =>
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
```

#### Using of the static schema to parse the passed html

```csharp
var parser = new AngleSharpParser(new NullLoggerFactory());

var html = await File.ReadAllTextAsync("test.html");
var model = await parser.RunAsync(schema, html);

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

#### Element schema

Sometimes the full schema binding is not necessary (only one value is required). Then the element schema class can be used.

```csharp
var dogNamesSchema = new AngleSharpElementSchema<string[]>(builder => builder.UseSelector(".dog .name"));

var parser = new AngleSharpParser(new NullLoggerFactory());
var html = await File.ReadAllTextAsync("test.html");
var dogNames = await parser.RunAsync(schema, html);

Assert.Equal(2, dogNames.Length);
Assert.Equal("Jelly", dogNames[0]);
Assert.Equal("Marly", dogNames[1]);
```

### Dynamic HTML crawling

The package Laraue.Crawling.Dynamic.PuppeterSharp intended to parse schemas using PuppeterSharp library.
Let's rewrite static schema to the dynamic format:

```csharp
public record OnePage(string Title, string[] ImageLinks, User User);
public record User(string Name, int Age, Dog[] Dogs);
public record Dog(string Name, int Age);


var schema = new PuppeterSharpSchemaBuilder<OnePage>()
    .HasProperty(x => x.Title, ".title")
    .HasObjectProperty(x => x.User, ".user", userBuilder =>
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
        async handle => await handle.GetAttributeValueAsync("href"))
    .Build();
```

The main difference that all functions now interacts with ElementHandle class from PuppeterSharp library.
The crawling can be executed this way:

```csharp
await new BrowserFetcher().DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
var response = await page.GoToAsync(link);
var model = await _parser.RunAsync(schema, await page.QuerySelectorAsync("body"));
```

### Extended features

Sometimes binding of html element to property is not enough. For example - one string should
be divided into three elements.

```html
<p class="info">
    Bob Martin 37
</p>
```

```csharp
record User(string Name, string Surname, int Age);
var schema = new PuppeterSharpSchemaBuilder<User>()
    .BindManually(async (element, modelBinder) => {
        var element = await element.QuerySelectorAsync(".info");
        if (element is null) return;
        var elementText = await element.GetInnerTextAsync();
        var stringParts = elementText.Split(' ');
        if (stringParts.Length != 3) return;
        modelBinder.BindProperty(x => x.Name, stringParts[0]);
        modelBinder.BindProperty(x => x.Surname, stringParts[1]);
        modelBinder.BindProperty(x => x.Age, int.Parse(stringParts[2]));
    })
```

### XML static crawling

```xml
<all>
    <note>
        <to id="15">Tove</to>
        <body>Don't forget me this weekend!</body>
    </note>
    <note>
        <to id="16">Max</to>
        <body>Hi!</body>
    </note>
</all>
```

Use class XmlSchemaBuilder to build the schema. 

```csharp
var schema = new XmlSchemaBuilder<XmlContent>()
    .HasArrayProperty<Note>(x => x.Notes, "//note", builder =>
    {
        builder.HasProperty(y => y.Body, b => b.UseSelector("body"));
        builder.HasProperty(y => y.Id, b => b
            .UseSelector("to")
            .GetInnerTextFromAttribute("id"));
    })
    .Build();
```

Schema parsing
```csharp
var parser = new XmlParser(new NullLoggerFactory());
var xmlDocument = new XmlDocument();
xmlDocument.LoadXml(xml);
        
var result = await parser.RunAsync(schema, xmlDocument);

Assert.NotEmpty(result!.Notes);
var notes = result.Notes.ToArray();

Assert.Equal("Don't forget me this weekend!", notes[0].Body);
Assert.Equal(15, notes[0].Id);

Assert.Equal("Hi!", notes[1].Body);
Assert.Equal(16, notes[1].Id);
```