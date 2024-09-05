using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flight.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    route_id = table.Column<int>(type: "INTEGER", nullable: false),
                    origin_city_id = table.Column<int>(type: "INTEGER", nullable: false),
                    destination_city_id = table.Column<int>(type: "INTEGER", nullable: false),
                    departure_date = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routes", x => x.route_id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    agency_id = table.Column<byte>(type: "INTEGER", nullable: false),
                    origin_city_id = table.Column<int>(type: "INTEGER", nullable: false),
                    destination_city_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => new { x.agency_id, x.origin_city_id, x.destination_city_id });
                });

            migrationBuilder.CreateTable(
                name: "flights",
                columns: table => new
                {
                    flight_id = table.Column<int>(type: "INTEGER", nullable: false),
                    route_id = table.Column<int>(type: "INTEGER", nullable: false),
                    airline_id = table.Column<int>(type: "INTEGER", nullable: false),
                    departure_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    arrival_time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flights", x => x.flight_id);
                    table.ForeignKey(
                        name: "FK_flights_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "route_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_flights_route_id_departure_time",
                table: "flights",
                columns: new[] { "route_id", "departure_time" });

            migrationBuilder.CreateIndex(
                name: "IX_routes_origin_city_id_destination_city_id",
                table: "routes",
                columns: new[] { "origin_city_id", "destination_city_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "flights");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "routes");
        }
    }
}
