using System.Collections.Generic;
using System.Text.Json;

namespace QuicklyTools.Models;

public sealed record JsonTreeNode(string Name, string Value, JsonValueKind Kind, IReadOnlyList<JsonTreeNode> Children);

public sealed record JsonTreeResult(JsonTreeNode Root, string FormattedJson, int NodeCount);
