using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Win32.SafeHandles;
using QuicklyTools.Models;

namespace QuicklyTools.Services;

/// <summary>Reads local saved WiFi profiles through the Windows WLAN API.</summary>
public sealed class WindowsWifiProfileService : IWifiProfileService
{
    public Task<IReadOnlyList<WifiProfile>> GetProfilesAsync(CancellationToken cancellationToken = default)
        => Task.Run(() => ReadProfiles(cancellationToken), cancellationToken);

    private static IReadOnlyList<WifiProfile> ReadProfiles(CancellationToken cancellationToken)
    {
        ThrowIfError(Native.WlanOpenHandle(2, IntPtr.Zero, out _, out var handle));
        using (handle)
        {
            ThrowIfError(Native.WlanEnumInterfaces(handle, IntPtr.Zero, out var interfaces));
            try
            {
                var result = new List<WifiProfile>();
                foreach (var info in ReadList<InterfaceInfo>(interfaces))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var interfaceId = info.Id;
                    ThrowIfError(Native.WlanGetProfileList(handle, ref interfaceId, IntPtr.Zero, out var profiles));
                    try
                    {
                        foreach (var profile in ReadList<ProfileInfo>(profiles))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            result.Add(ReadProfile(handle, interfaceId, profile.Name));
                        }
                    }
                    finally { Native.WlanFreeMemory(profiles); }
                }

                return result.OrderBy(item => item.ProfileName, StringComparer.CurrentCultureIgnoreCase).ToArray();
            }
            finally { Native.WlanFreeMemory(interfaces); }
        }
    }

    private static WifiProfile ReadProfile(WlanHandle handle, Guid interfaceId, string name)
    {
        uint flags = 4; // WLAN_PROFILE_GET_PLAINTEXT_KEY; Windows enforces access permissions.
        var error = Native.WlanGetProfile(handle, ref interfaceId, name, IntPtr.Zero, out var xml, ref flags, out _);
        var passwordDenied = error == 5;
        if (passwordDenied)
        {
            flags = 0;
            error = Native.WlanGetProfile(handle, ref interfaceId, name, IntPtr.Zero, out xml, ref flags, out _);
        }

        if (error != 0)
            return new(name, name, string.Empty, null, new Win32Exception((int)error).Message);

        try
        {
            var document = XDocument.Parse(Marshal.PtrToStringUni(xml)!);
            var root = document.Root!;
            var ns = root.Name.Namespace;
            var security = root.Element(ns + "MSM")?.Element(ns + "security");
            var sharedKey = security?.Element(ns + "sharedKey");
            var authentication = security?.Element(ns + "authEncryption")?.Element(ns + "authentication")?.Value ?? "未知";
            var ssid = root.Element(ns + "SSIDConfig")?.Element(ns + "SSID")?.Element(ns + "name")?.Value ?? name;
            var encrypted = !string.Equals(sharedKey?.Element(ns + "protected")?.Value, "false", StringComparison.OrdinalIgnoreCase);
            var password = !passwordDenied && !encrypted ? sharedKey?.Element(ns + "keyMaterial")?.Value : null;
            var status = password is not null ? "已保存密码"
                : sharedKey is not null ? "系统未授予读取明文密码的权限"
                : authentication == "open" ? "开放网络，无需密码" : "此网络没有可显示的共享密码";
            return new(name, ssid, authentication, password, status);
        }
        finally { Native.WlanFreeMemory(xml); }
    }

    private static IEnumerable<T> ReadList<T>(IntPtr list) where T : struct
    {
        var count = Marshal.ReadInt32(list);
        var size = Marshal.SizeOf<T>();
        for (var i = 0; i < count; i++)
            yield return Marshal.PtrToStructure<T>(IntPtr.Add(list, 8 + i * size));
    }

    private static void ThrowIfError(uint error)
    {
        if (error != 0)
            throw new Win32Exception((int)error);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct InterfaceInfo
    {
        public Guid Id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Description;
        public int State;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct ProfileInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Name;
        public uint Flags;
    }

    private sealed class WlanHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public WlanHandle() : base(true) { }
        protected override bool ReleaseHandle() => Native.WlanCloseHandle(handle, IntPtr.Zero) == 0;
    }

    private static class Native
    {
        [DllImport("wlanapi.dll")]
        internal static extern uint WlanOpenHandle(uint version, IntPtr reserved, out uint negotiatedVersion, out WlanHandle handle);
        [DllImport("wlanapi.dll")]
        internal static extern uint WlanCloseHandle(IntPtr handle, IntPtr reserved);
        [DllImport("wlanapi.dll")]
        internal static extern uint WlanEnumInterfaces(WlanHandle handle, IntPtr reserved, out IntPtr interfaces);
        [DllImport("wlanapi.dll")]
        internal static extern uint WlanGetProfileList(WlanHandle handle, ref Guid interfaceId, IntPtr reserved, out IntPtr profiles);
        [DllImport("wlanapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern uint WlanGetProfile(WlanHandle handle, ref Guid interfaceId, string name, IntPtr reserved,
            out IntPtr xml, ref uint flags, out uint grantedAccess);
        [DllImport("wlanapi.dll")]
        internal static extern void WlanFreeMemory(IntPtr memory);
    }
}
