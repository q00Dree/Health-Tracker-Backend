using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HealthTracker.DAL.Migrations
{
    public partial class Intiial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Controllers",
                columns: table => new
                {
                    DeviceId = table.Column<string>(nullable: false),
                    IsOnline = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controllers", x => x.DeviceId);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<string>(nullable: false),
                    SensorType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sensors_Controllers_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Controllers",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_DeviceId",
                table: "Sensors",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "Controllers");
        }
    }
}
