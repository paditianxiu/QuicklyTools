using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using QuicklyTools.Models;

namespace QuicklyTools.Services;

public sealed class JsonTreeService
{
    // Output is displayed as plain text, so Unicode characters can remain readable.
    private static readonly JsonSerializerOptions ReadableOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static readonly JsonSerializerOptions FormattingOptions = new(ReadableOptions)
    {
        WriteIndented = true,
    };

    public JsonTreeResult Parse(string json, CancellationToken cancellationToken = default)
    {
        using var document = JsonDocument.Parse(json);
        var count = 0;
        var root = BuildNode("$", document.RootElement, ref count, cancellationToken);
        return new(root, JsonSerializer.Serialize(document.RootElement, FormattingOptions), count);
    }

    private static JsonTreeNode BuildNode(string name, JsonElement element, ref int count, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (++count > 20000)
            throw new InvalidOperationException("JSON 节点超过 20,000 个，请缩小输入后重试。");

        var children = new List<JsonTreeNode>();
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                    children.Add(BuildNode(property.Name, property.Value, ref count, cancellationToken));
                break;
            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                    children.Add(BuildNode($"[{index++}]", item, ref count, cancellationToken));
                break;
        }

        var value = element.ValueKind switch
        {
            JsonValueKind.Object => $"{{ {children.Count} 个属性 }}",
            JsonValueKind.Array => $"[ {children.Count} 项 ]",
            JsonValueKind.String => JsonSerializer.Serialize(element.GetString(), ReadableOptions),
            _ => element.GetRawText(),
        };
        return new(name, value, element.ValueKind, children);
    }
}
