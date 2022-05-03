using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Acceptance.Tests;

public static class ILocatorExtension
{
    public static async Task<IReadOnlyList<ILocator>> GetAll(this ILocator loc)
    {
        var count = await loc.CountAsync();
        return Enumerable.Range(0, count)
                         .Select(loc.Nth)
                         .ToList();
    }
}