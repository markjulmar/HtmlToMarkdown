using Julmar.DocsToMarkdown;

//var html = @"https://learn.microsoft.com/en-us/java/openjdk/overview";
//var html = @"https://learn.microsoft.com/en-us/azure/app-service/overview-security";
//const string html = @"https://learn.microsoft.com/en-us/azure/app-service/monitor-app-service";
//const string html = @"https://learn.microsoft.com/en-us/azure/app-service/configure-linux-open-ssh-session?pivots=container-linux";
//const string html = @"https://learn.microsoft.com/en-us/azure/app-service/monitor-instances-health-check?tabs=dotnet";

//const string html = @"https://learn.microsoft.com/en-us/training/modules/secure-ai-services/2-authentication";const string html = @"https://learn.microsoft.com/en-us/training/modules/secure-ai-services/2-authentication";
const string html = @"https://learn.microsoft.com/en-us/training/modules/secure-ai-services/5-knowledge-check";

var converter = await HtmlConverterFactory.Create(new Uri(html, UriKind.Absolute));
converter.UseDocfxExtensions = true;
converter.UnhandledTagCallback = tag 
    => Console.Error.WriteLine($"Unhandled tag: {tag}");

var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.md");
await using var outputFile = File.CreateText(filename);

// Write the YAML header
var md = converter.Metadata;
var header = md.YamlHeader;
if (!string.IsNullOrEmpty(header))
{
    await outputFile.WriteLineAsync(header);
}

foreach (var markdownText in converter.Process())
{
    await outputFile.WriteLineAsync(markdownText);
}
