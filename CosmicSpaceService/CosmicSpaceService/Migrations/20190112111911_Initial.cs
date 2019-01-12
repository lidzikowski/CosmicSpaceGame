using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CosmicSpaceService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ammunitions",
                columns: table => new
                {
                    AmmunitionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    MultiplierPilot = table.Column<double>(nullable: false, defaultValueSql: "((1))"),
                    MultiplierEnemy = table.Column<double>(nullable: false, defaultValueSql: "((1))"),
                    ScrapPrice = table.Column<double>(nullable: false),
                    MetalPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ammunitions", x => x.AmmunitionId);
                });

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    MapId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    IsPvp = table.Column<bool>(nullable: false, defaultValueSql: "((0))"),
                    RequiredLevel = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.MapId);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    RewardId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Experience = table.Column<long>(nullable: false),
                    Metal = table.Column<double>(nullable: false),
                    Scrap = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.RewardId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(maxLength: 128, nullable: false),
                    Password = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: false),
                    EmailNewsletter = table.Column<bool>(nullable: false),
                    AcceptRules = table.Column<bool>(nullable: false),
                    Ban = table.Column<bool>(nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Enemies",
                columns: table => new
                {
                    EnemyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    BasicHitpoints = table.Column<long>(nullable: false),
                    BasicShields = table.Column<long>(nullable: false),
                    BasicSpeed = table.Column<int>(nullable: false),
                    BasicDamage = table.Column<long>(nullable: false),
                    BasicShotDistance = table.Column<int>(nullable: false),
                    IsAggressive = table.Column<bool>(nullable: false),
                    RewardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enemies", x => x.EnemyId);
                    table.ForeignKey(
                        name: "FK_Enemies_Rewards_RewardId",
                        column: x => x.RewardId,
                        principalTable: "Rewards",
                        principalColumn: "RewardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    ShipId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    RequiredLevel = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    ScrapPrice = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    MetalPrice = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    Lasers = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    Generators = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    Extras = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    BasicSpeed = table.Column<int>(nullable: false, defaultValueSql: "((50))"),
                    BasicCargo = table.Column<int>(nullable: false, defaultValueSql: "((100))"),
                    BasicHitpoints = table.Column<long>(nullable: false, defaultValueSql: "((1000))"),
                    RewardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.ShipId);
                    table.ForeignKey(
                        name: "FK_Ships_Rewards_RewardId",
                        column: x => x.RewardId,
                        principalTable: "Rewards",
                        principalColumn: "RewardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnemiesMaps",
                columns: table => new
                {
                    MapId = table.Column<int>(nullable: false),
                    EnemyId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnemiesMaps", x => new { x.MapId, x.EnemyId });
                    table.ForeignKey(
                        name: "FK_EnemiesMaps_Enemies_EnemyId",
                        column: x => x.EnemyId,
                        principalTable: "Enemies",
                        principalColumn: "EnemyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnemiesMaps_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    Nickname = table.Column<string>(maxLength: 30, nullable: false),
                    PositionX = table.Column<float>(nullable: false, defaultValueSql: "((100))"),
                    PositionY = table.Column<float>(nullable: false, defaultValueSql: "((-100))"),
                    Experience = table.Column<long>(nullable: false, defaultValueSql: "((1000))"),
                    Level = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    Scrap = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    Metal = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    Hitpoints = table.Column<long>(nullable: false, defaultValueSql: "((1000))"),
                    Shields = table.Column<long>(nullable: false, defaultValueSql: "((0))"),
                    IsDead = table.Column<bool>(nullable: false, defaultValueSql: "((0))"),
                    KillBy = table.Column<string>(maxLength: 30, nullable: true),
                    MapId = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    ShipId = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Pilots_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pilots_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pilots_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AmmunitionsPilots",
                columns: table => new
                {
                    AmmunitionId = table.Column<int>(nullable: false),
                    PilotId = table.Column<long>(nullable: false),
                    Count = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmmunitionsPilots", x => new { x.AmmunitionId, x.PilotId });
                    table.ForeignKey(
                        name: "FK_AmmunitionsPilots_Ammunitions_AmmunitionId",
                        column: x => x.AmmunitionId,
                        principalTable: "Ammunitions",
                        principalColumn: "AmmunitionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AmmunitionsPilots_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UC_Ammunition",
                table: "Ammunitions",
                columns: new[] { "AmmunitionId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AmmunitionsPilots_AmmunitionId",
                table: "AmmunitionsPilots",
                column: "AmmunitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AmmunitionsPilots_PilotId",
                table: "AmmunitionsPilots",
                column: "PilotId");

            migrationBuilder.CreateIndex(
                name: "IX_Enemies_RewardId",
                table: "Enemies",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_EnemiesMaps_EnemyId",
                table: "EnemiesMaps",
                column: "EnemyId");

            migrationBuilder.CreateIndex(
                name: "UC_Map",
                table: "Maps",
                columns: new[] { "MapId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pilots_MapId",
                table: "Pilots",
                column: "MapId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UC_Pilot",
                table: "Pilots",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pilots_ShipId",
                table: "Pilots",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UC_Reward",
                table: "Rewards",
                column: "RewardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ships_RewardId",
                table: "Ships",
                column: "RewardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UC_Ship",
                table: "Ships",
                columns: new[] { "ShipId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UC_User",
                table: "Users",
                columns: new[] { "UserId", "Username", "Password", "Email" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmmunitionsPilots");

            migrationBuilder.DropTable(
                name: "EnemiesMaps");

            migrationBuilder.DropTable(
                name: "Ammunitions");

            migrationBuilder.DropTable(
                name: "Pilots");

            migrationBuilder.DropTable(
                name: "Enemies");

            migrationBuilder.DropTable(
                name: "Maps");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Rewards");
        }
    }
}
