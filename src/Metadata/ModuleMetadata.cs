using System.Text;
using HtmlAgilityPack;
using MSLearnCatalogAPI;

namespace Julmar.DocsToMarkdown.Metadata;

public sealed class ModuleMetadata : BaseMetadata
{
    private Module? _catalogMetadata;
    public override string? PageKind => "module";

    private ModuleMetadata(HtmlDocument document) : base(document)
    {
    }

    public static async Task<IDocsMetadata> Create(HtmlDocument document)
    {
        var metadata = new ModuleMetadata(document);
        await metadata.FillInFromLearnApi();
        return metadata;
    }

    private async Task FillInFromLearnApi()
    {
        var id = LoadedValues.GetValueOrDefault("uid");
        if (string.IsNullOrWhiteSpace(id))
            return;
        
        var modules = await CatalogApi.GetModulesAsync(filter: new CatalogFilter()
        {
            Uids = [id]
        });
        
        if (modules.Count == 1)
            _catalogMetadata = modules[0];
    }

    private static readonly string[] MetadataNames =
    [
        "title",
        "description",
        "keywords",
        "ms.topic",
        "ms.date",
        "ms.custom",
        "author",
        "ms.author",
        "manager",
        "ms.service",
        "ms.devlang",
        "zone_pivot_groups",
    ];
    
    public override string YamlHeader
    {
        get
        {
            var sb = new StringBuilder("### YamlMime:Module" + Environment.NewLine);
            var id = LoadedValues.GetValueOrDefault("uid");
            if (!string.IsNullOrWhiteSpace(id))
                sb.AppendLine($"uid: {id}");

            sb.AppendLine("metadata:");
            foreach (var key in MetadataNames)
            {
                if (LoadedValues.TryGetValue(key, out var value)
                    && !string.IsNullOrWhiteSpace(value))
                    sb.AppendLine($"  {key}: {EscapeYaml(value)}");
            }

            var title = LoadedValues.GetValueOrDefault("title");
            if (!string.IsNullOrWhiteSpace(title))
                sb.AppendLine($"title: {EscapeYaml(title)}");
            
            if (_catalogMetadata != null)
            {
                const string prefix = @"https://learn.microsoft.com/en-us/";
                sb.AppendLine($"summary: {EscapeYaml(_catalogMetadata.Summary)}")
                  .AppendLine($"iconUrl: /{_catalogMetadata.IconUrl[prefix.Length..]}");
                if (_catalogMetadata.Levels.Count > 0)
                {
                    sb.AppendLine("levels:");
                    foreach (var level in _catalogMetadata.Levels)
                        sb.AppendLine($"- {level}");
                }

                if (_catalogMetadata.Roles.Count > 0)
                {
                    sb.AppendLine("roles:");
                    foreach (var role in _catalogMetadata.Roles)
                        sb.AppendLine($"- {role}");
                }
                
                if (_catalogMetadata.Products.Count > 0)
                {
                    sb.AppendLine("products:");
                    foreach (var product in _catalogMetadata.Products)
                        sb.AppendLine($"- {product}");
                }

                if (_catalogMetadata.Subjects.Count > 0)
                {
                    sb.AppendLine("subjects:");
                    foreach (var subject in _catalogMetadata.Subjects)
                        sb.AppendLine($"- {subject}");
                }

                if (_catalogMetadata.Units.Count > 0)
                {
                    sb.AppendLine("units:");
                    foreach (var unit in _catalogMetadata.Units)
                        sb.AppendLine($"- {unit}");
                }

                if (!string.IsNullOrWhiteSpace(id))
                    sb.AppendLine($"badge:").AppendLine($"  uid: {id}.badge");
            }

            return sb.ToString();
        }
    }
}