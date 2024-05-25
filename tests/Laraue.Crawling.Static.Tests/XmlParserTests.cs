using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Laraue.Crawling.Abstractions;
using Laraue.Crawling.Static.Xml;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Laraue.Crawling.Static.Tests;

public class XmlParserTests
{
    [Fact]
    public async Task Scheme_ShouldBeParsedCorrectlyAsync()
    {
        var xml = @"
<all>
    <note>
        <to id=""15"">Tove</to>
        <body>Don't forget me this weekend!</body>
    </note>
    <note>
        <to id=""16"">Max</to>
        <body>Hi!</body>
    </note>
</all>
";

        var schema = new XmlSchemaBuilder<XmlContent>()
            .HasArrayProperty<Note>(x => x.Notes, "//note", builder =>
            {
                builder.HasProperty(y => y.Body, b => b
                    .UseSelector("body"));
                builder.HasProperty(y => y.Id, b => b
                    .UseSelector("to")
                    .GetInnerTextFromAttribute("id"));
            })
            .Build();

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
    }

    private sealed record XmlContent : ICrawlingModel
    {
        public IEnumerable<Note> Notes { get; init; }
    }
    
    private sealed record Note : ICrawlingModel
    {
        public int Id { get; init; }
        public string Body { get; init; }
    }
}