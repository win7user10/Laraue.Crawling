# Laraue.Crawling packages

The set of tools for fast writing crawlers on .NET.

### Static crawling

Static means the crawling process is performing in the html which is not changes.
You can build a strongly typed schema where for each property defines it mapping to
html by using html selectors. Then this schema can be parsed via IStaticHtmlSchemaParser class.
Use default implementation AngleSharpHtmlParser to retrieve all data.

#### Build static schema

```cs
record User(string Name, int Age, Dog[] Dogs);
record Dog(string Name, int Age);

var schema = new HtmlSchemaBuilder<User>()
    .HasProperty(x => x.Name, ".name")
    .HasProperty(x => x.Age, ".age", ageElement => int.Parse(ageElement.GetInnerHtml()))
    .HasArrayProperty(x => x.Dogs, ".dog", dogsBuilder =>
    {
        dogsBuilder.HasProperty(x => x.Age, ".age", x => int.Parse(x.GetInnerHtml()))
            .HasProperty(x => x.Name, ".name");
    });
    .Build();
```

#### Using of the static schema for parsing of the passed html

```cs
var htmlParser = new HtmlParser();
var visitor = new AngleSharpParser(htmlParser);

var html = File.ReadAllText("test.html");
var model = visitor.Parse(schema, html);

Assert.Equal("Alex", model.Name);
Assert.Equal(10, model.Age);

var dogs = model.User.Dogs;
Assert.Equal(2, dogs.Length);

var dog1 = dogs[0];
Assert.Equal(5, dog1.Age);
Assert.Equal("Jelly", dog1.Name);

var dog2 = dogs[1];
Assert.Equal(7, dog2.Age);
Assert.Equal("Marly", dog2.Name);
```

### Dynamic crawling

This section is not ready