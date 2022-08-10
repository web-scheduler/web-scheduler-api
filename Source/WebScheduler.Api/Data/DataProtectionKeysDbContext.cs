namespace WebScheduler.Api.Data;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// TODO.
/// </summary>
public class DataProtectionKeysDbContext : DbContext, IDataProtectionKeyContext
{
    /// <summary>
    ///     <para>
    ///         Initializes a new instance of the <see cref="DbContext" /> class using the specified options.
    ///         The <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)" /> method will still be called to allow further
    ///         configuration of the options.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see> and
    ///     <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see> for more information.
    /// </remarks>
    /// <param name="options">The options for this context.</param>
    public DataProtectionKeysDbContext(DbContextOptions<DataProtectionKeysDbContext> options)
    : base(options)
    {
    }

    /// <summary>
    /// A collection of <see cref="DataProtectionKey" />
    /// </summary>
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = default!;
}
