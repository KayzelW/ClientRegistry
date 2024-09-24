using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientRegistry.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AddDate = table.Column<DateTime>(type: "timestamp(6) without time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Inn);
                });

            migrationBuilder.CreateTable(
                name: "Founders",
                columns: table => new
                {
                    Inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddDate = table.Column<DateTime>(type: "timestamp(6) without time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Founders", x => x.Inn);
                });

            migrationBuilder.CreateTable(
                name: "ClientFounder",
                columns: table => new
                {
                    ClientsInn = table.Column<string>(type: "character varying(12)", nullable: false),
                    FoundersInn = table.Column<string>(type: "character varying(12)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientFounder", x => new { x.ClientsInn, x.FoundersInn });
                    table.ForeignKey(
                        name: "FK_ClientFounder_Clients_ClientsInn",
                        column: x => x.ClientsInn,
                        principalTable: "Clients",
                        principalColumn: "Inn",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientFounder_Founders_FoundersInn",
                        column: x => x.FoundersInn,
                        principalTable: "Founders",
                        principalColumn: "Inn",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientFounder_FoundersInn",
                table: "ClientFounder",
                column: "FoundersInn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientFounder");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Founders");
        }
    }
}
