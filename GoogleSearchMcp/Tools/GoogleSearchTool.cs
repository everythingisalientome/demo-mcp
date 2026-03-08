using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace GoogleSearchMcp.Tools
{
    [McpServerToolType]
    public class GoogleSearchTool
    {
        [McpServerTool, Description("Search Google for a given query and return results.")]
        public static string GoogleSearch(
            [Description("The search query string")] string query)
        {
            // Mock response — replace with real SerpAPI call later
            var mockResults = new[]
            {
            new { title = $"Result 1 for '{query}'", url = "https://example.com/1", snippet = "This is a mock search result snippet for result 1." },
            new { title = $"Result 2 for '{query}'", url = "https://example.com/2", snippet = "This is a mock search result snippet for result 2." },
            new { title = $"Result 3 for '{query}'", url = "https://example.com/3", snippet = "This is a mock search result snippet for result 3." },
        };

            return System.Text.Json.JsonSerializer.Serialize(mockResults);
        }
    }
}
