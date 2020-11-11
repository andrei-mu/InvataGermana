using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvataGermana.Migrations
{
    public partial class NounsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nouns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Gen = table.Column<int>(nullable: false),
                    ParentLessonId = table.Column<int>(nullable: true),
                    Plural = table.Column<string>(nullable: true),
                    Singular = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nouns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nouns_lessons_ParentLessonId",
                        column: x => x.ParentLessonId,
                        principalTable: "lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nouns_ParentLessonId",
                table: "nouns",
                column: "ParentLessonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nouns");
        }
    }
}
