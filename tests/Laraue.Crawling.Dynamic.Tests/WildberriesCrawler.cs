using Laraue.Crawling.Dynamic.C;

namespace Laraue.Crawling.Dynamic.Tests;

public class WildberriesCrawler : BaseFileCrawler<WildberriesProductPage>
{
    public override string LinksToParsePath => "sections_lists.json";
    
    
}