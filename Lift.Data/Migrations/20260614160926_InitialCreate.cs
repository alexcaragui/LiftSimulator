using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lift.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiftEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessInterruptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoppedByUserId = table.Column<int>(type: "int", nullable: false),
                    StoppedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RestartedByUserId = table.Column<int>(type: "int", nullable: true),
                    RestartedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessInterruptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessInterruptions_Users_RestartedByUserId",
                        column: x => x.RestartedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessInterruptions_Users_StoppedByUserId",
                        column: x => x.StoppedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$11$3P57MKhJUmv8Q0z2PVOoy.TlUfHogbzRrZyYutsQjzWeyATE498By", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInterruptions_RestartedByUserId",
                table: "ProcessInterruptions",
                column: "RestartedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInterruptions_StoppedByUserId",
                table: "ProcessInterruptions",
                column: "StoppedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiftEvents");

            migrationBuilder.DropTable(
                name: "ProcessInterruptions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
