using Microsoft.EntityFrameworkCore;
using WMS.Infrastructure.Data;

namespace WMS.Tests.Helpers;

public static class TestDbContextFactory
{
    public static WmsDbContext Create()
    {
        var options = new DbContextOptionsBuilder<WmsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WmsDbContext(options, null);
    }
}
