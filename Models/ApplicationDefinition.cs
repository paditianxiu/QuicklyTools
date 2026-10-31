using System;

namespace QuicklyTools.Models;

/// <summary>Application card metadata, independent of UI controls and services.</summary>
public sealed record ApplicationDefinition(
    string Title,
    string Subtitle,
    string Icon,
    string Accent,
    Type TargetPage);
