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
    [Migration("20201116181021_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.3");

            modelBuilder.Entity("InvataGermana.Model.Lesson", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title");

                    b.HasKey("ID");

                    b.ToTable("lessons");
                });

            modelBuilder.Entity("InvataGermana.Model.Noun", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Gen");

                    b.Property<int>("LessonId");

                    b.Property<string>("Plural");

                    b.Property<string>("Singular");

                    b.Property<string>("Translation");

                    b.HasKey("ID");

                    b.HasIndex("LessonId");

                    b.ToTable("nouns");
                });

            modelBuilder.Entity("InvataGermana.Model.Noun", b =>
                {
                    b.HasOne("InvataGermana.Model.Lesson", "Lesson")
                        .WithMany("Nouns")
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
