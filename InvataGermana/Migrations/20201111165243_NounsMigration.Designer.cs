using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using InvataGermana.Data;
using InvataGermana.Model;

namespace InvataGermana.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20201111165243_NounsMigration")]
    partial class NounsMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.3");

            modelBuilder.Entity("InvataGermana.Model.Lesson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("lessons");
                });

            modelBuilder.Entity("InvataGermana.Model.Noun", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Gen");

                    b.Property<int?>("ParentLessonId");

                    b.Property<string>("Plural");

                    b.Property<string>("Singular");

                    b.HasKey("Id");

                    b.HasIndex("ParentLessonId");

                    b.ToTable("nouns");
                });

            modelBuilder.Entity("InvataGermana.Model.Noun", b =>
                {
                    b.HasOne("InvataGermana.Model.Lesson", "ParentLesson")
                        .WithMany("Nouns")
                        .HasForeignKey("ParentLessonId");
                });
        }
    }
}
