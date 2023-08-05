using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QL_HS.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Username = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    Password = table.Column<string>(type: "NVARCHAR(200)", nullable: true),
                    Role = table.Column<string>(type: "NVARCHAR(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guardians",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR(200)", nullable: true),
                    Phone = table.Column<string>(type: "NVARCHAR(20)", nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR(300)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    AccountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guardians_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR(300)", nullable: true),
                    Dob = table.Column<DateTime>(nullable: false),
                    Class = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    Grade = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    Bio = table.Column<string>(type: "NVARCHAR(1000)", nullable: true),
                    GuardianId = table.Column<int>(nullable: false),
                    GuardianEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Guardians_GuardianEntityId",
                        column: x => x.GuardianEntityId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pickups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    StudentId = table.Column<int>(nullable: false),
                    GuardianId = table.Column<int>(nullable: false),
                    GuardianEntityId = table.Column<int>(nullable: true),
                    StudentEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pickups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pickups_Guardians_GuardianEntityId",
                        column: x => x.GuardianEntityId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pickups_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pickups_Students_StudentEntityId",
                        column: x => x.StudentEntityId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pickups_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_AccountId",
                table: "Guardians",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_GuardianEntityId",
                table: "Pickups",
                column: "GuardianEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_GuardianId",
                table: "Pickups",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_StudentEntityId",
                table: "Pickups",
                column: "StudentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_StudentId_GuardianId",
                table: "Pickups",
                columns: new[] { "StudentId", "GuardianId" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianEntityId",
                table: "Students",
                column: "GuardianEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianId",
                table: "Students",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Name_Class",
                table: "Students",
                columns: new[] { "Name", "Class" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pickups");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
