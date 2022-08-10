#nullable disable

namespace WebScheduler.Api.Data.Migrations.DataProtectionKeysDb;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

/// <summary>
/// TODO
/// </summary>
public partial class DataProtection : Migration
{
    /// <summary>
    ///     <para>
    ///         Builds the operations that will migrate the database 'up'.
    ///     </para>
    ///     <para>
    ///         That is, builds the operations that will take the database from the state left in by the
    ///         previous migration so that it is up-to-date with regard to this migration.
    ///     </para>
    ///     <para>
    ///         This method must be overridden in each class that inherits from <see cref="Migration" />.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see> for more information.
    /// </remarks>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder" /> that will build the operations.</param>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        _ = migrationBuilder.CreateTable(
            name: "DataProtectionKeys",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                FriendlyName = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Xml = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table => table.PrimaryKey("PK_DataProtectionKeys", x => x.Id))
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    /// <summary>
    ///     <para>
    ///         Builds the operations that will migrate the database 'down'.
    ///     </para>
    ///     <para>
    ///         That is, builds the operations that will take the database from the state left in by
    ///         this migration so that it returns to the state that it was in before this migration was applied.
    ///     </para>
    ///     <para>
    ///         This method must be overridden in each class that inherits from <see cref="Migration" /> if
    ///         both 'up' and 'down' migrations are to be supported. If it is not overridden, then calling it
    ///         will throw and it will not be possible to migrate in the 'down' direction.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see> for more information.
    /// </remarks>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder" /> that will build the operations.</param>
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "DataProtectionKeys");
}
