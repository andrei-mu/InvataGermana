using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvataGermana.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "nouns",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Gen = table.Column<int>(nullable: false),
                    LessonID = table.Column<int>(nullable: false),
                    Plural = table.Column<string>(nullable: true),
                    Singular = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nouns", x => x.ID);
                    table.ForeignKey(
                        name: "FK_nouns_lessons_LessonID",
                        column: x => x.LessonID,
                        principalTable: "lessons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nouns_LessonID",
                table: "nouns",
                column: "LessonID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nouns");

            migrationBuilder.DropTable(
                name: "lessons");
        }
    }
}
