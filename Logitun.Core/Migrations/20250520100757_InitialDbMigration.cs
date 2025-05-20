using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitun.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auth_credentials",
                columns: table => new
                {
                    credential_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    activated = table.Column<bool>(type: "boolean", nullable: false),
                    lang_key = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    activation_key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    reset_key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    reset_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_credentials", x => x.credential_id);
                });

            migrationBuilder.CreateTable(
                name: "auth_information",
                columns: table => new
                {
                    information_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cin = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_information", x => x.information_id);
                });

            migrationBuilder.CreateTable(
                name: "auth_role",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_role", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.location_id);
                });

            migrationBuilder.CreateTable(
                name: "trucks",
                columns: table => new
                {
                    truck_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plate_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    manufacture_year = table.Column<int>(type: "integer", nullable: true),
                    capacity_kg = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    fuel_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    last_maintenance_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trucks", x => x.truck_id);
                });

            migrationBuilder.CreateTable(
                name: "auth_user",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    information_id = table.Column<int>(type: "integer", nullable: false),
                    credentials_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_user", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_auth_user_auth_credentials_credentials_id",
                        column: x => x.credentials_id,
                        principalTable: "auth_credentials",
                        principalColumn: "credential_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_auth_user_auth_information_information_id",
                        column: x => x.information_id,
                        principalTable: "auth_information",
                        principalColumn: "information_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auth_credentials_role",
                columns: table => new
                {
                    credential_id = table.Column<int>(type: "integer", nullable: false),
                    role_name = table.Column<string>(type: "character varying(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_credentials_role", x => new { x.credential_id, x.role_name });
                    table.ForeignKey(
                        name: "FK_auth_credentials_role_auth_credentials",
                        column: x => x.credential_id,
                        principalTable: "auth_credentials",
                        principalColumn: "credential_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_auth_credentials_role_auth_role",
                        column: x => x.role_name,
                        principalTable: "auth_role",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "missions",
                columns: table => new
                {
                    mission_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    truck_id = table.Column<int>(type: "integer", nullable: true),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    origin_location_id = table.Column<int>(type: "integer", nullable: true),
                    destination_location_id = table.Column<int>(type: "integer", nullable: true),
                    start_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    distance_km = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    payload_weight = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_missions", x => x.mission_id);
                    table.ForeignKey(
                        name: "FK_missions_auth_user_driver_id",
                        column: x => x.driver_id,
                        principalTable: "auth_user",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_missions_locations_destination_location_id",
                        column: x => x.destination_location_id,
                        principalTable: "locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_missions_locations_origin_location_id",
                        column: x => x.origin_location_id,
                        principalTable: "locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_missions_trucks_truck_id",
                        column: x => x.truck_id,
                        principalTable: "trucks",
                        principalColumn: "truck_id");
                });

            migrationBuilder.CreateTable(
                name: "time_off_requests",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_off_requests", x => x.request_id);
                    table.ForeignKey(
                        name: "FK_time_off_requests_auth_user_driver_id",
                        column: x => x.driver_id,
                        principalTable: "auth_user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_auth_credentials_login",
                table: "auth_credentials",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_credentials_role_role_name",
                table: "auth_credentials_role",
                column: "role_name");

            migrationBuilder.CreateIndex(
                name: "IX_auth_information_cin",
                table: "auth_information",
                column: "cin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_information_email",
                table: "auth_information",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_information_phone_number",
                table: "auth_information",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_user_credentials_id",
                table: "auth_user",
                column: "credentials_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_user_information_id",
                table: "auth_user",
                column: "information_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_missions_destination_location_id",
                table: "missions",
                column: "destination_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_missions_driver_id",
                table: "missions",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_missions_origin_location_id",
                table: "missions",
                column: "origin_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_missions_truck_id",
                table: "missions",
                column: "truck_id");

            migrationBuilder.CreateIndex(
                name: "IX_time_off_requests_driver_id",
                table: "time_off_requests",
                column: "driver_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth_credentials_role");

            migrationBuilder.DropTable(
                name: "missions");

            migrationBuilder.DropTable(
                name: "time_off_requests");

            migrationBuilder.DropTable(
                name: "auth_role");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "trucks");

            migrationBuilder.DropTable(
                name: "auth_user");

            migrationBuilder.DropTable(
                name: "auth_credentials");

            migrationBuilder.DropTable(
                name: "auth_information");
        }
    }
}
