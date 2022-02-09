namespace IdentityServerHost.Data;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DataProtectionKeysDbContext : DbContext, IDataProtectionKeyContext
{
    public DataProtectionKeysDbContext(DbContextOptions<DataProtectionKeysDbContext> options)
    : base(options)
    {
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = default!;
}
