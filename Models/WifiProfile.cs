namespace QuicklyTools.Models;

public sealed record WifiProfile(string ProfileName, string Ssid, string Authentication, string? Password, string Status);
