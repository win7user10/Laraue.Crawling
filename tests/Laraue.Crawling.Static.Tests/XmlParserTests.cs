using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Laraue.Crawling.Static.Xml;
using Laraue.Crawling.Static.Xml.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Laraue.Crawling.Static.Tests;

public class XmlParserTests
{
    [Fact]
    public async Task Scheme_ShouldBeParsedCorrectlyAsync()
    {
        var xml = @"
<note>
    <to id=""15"">Tove</to>
    <from>Jani</from>
    <heading>Reminder</heading>
    <body>Don't forget me this weekend!</body>
</note>
";

        var schema = new XmlSchemaBuilder<XmlContent>()
            .HasArrayProperty<Note>(x => x.Notes, "//note", builder =>
            {
                builder.HasProperty(y => y.Body, xPathSelector: "//body");
                builder.HasProperty(y => y.Id, xPathSelector: "//to", attributeName: "id");
            })
            .Build();

        var parser = new XmlParser(new NullLoggerFactory());
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        
        var result = await parser.RunAsync(schema, xmlDocument);
        var note = Assert.Single(result!.Notes);
        
        Assert.Equal("Don't forget me this weekend!", note.Body);
        Assert.Equal(15, note.Id);
    }

    private sealed record XmlContent
    {
        public IEnumerable<Note> Notes { get; init; }
    }
    
    private sealed record Note
    {
        public int Id { get; init; }
        public string Body { get; init; }
    }
}