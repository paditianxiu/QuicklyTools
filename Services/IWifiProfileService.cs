using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QuicklyTools.Models;

namespace QuicklyTools.Services;

public interface IWifiProfileService
{
    Task<IReadOnlyList<WifiProfile>> GetProfilesAsync(CancellationToken cancellationToken = default);
}
