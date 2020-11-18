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
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("InvataGermana.Model.Word", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Gen");

                    b.Property<string>("German");

                    b.Property<int>("LessonId");

                    b.Property<string>("Plural");

                    b.Property<int>("SpeechType");

                    b.Property<string>("Translation");

                    b.HasKey("ID");

                    b.HasIndex("LessonId");

                    b.ToTable("words");
                });

            modelBuilder.Entity("InvataGermana.Model.Word", b =>
                {
                    b.HasOne("InvataGermana.Model.Lesson", "Lesson")
                        .WithMany("Words")
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
