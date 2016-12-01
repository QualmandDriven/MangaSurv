using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MangaSurvWebApi.Migrations
{
    [DbContext(typeof(MangaSurvContext))]
    [Migration("20161128213710_ListMigration")]
    partial class ListMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Chapter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<float>("ChapterNo");

                    b.Property<DateTime>("EnterDate");

                    b.Property<int>("MangaId");

                    b.HasKey("Id");

                    b.HasIndex("MangaId");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<int>("ChapterId");

                    b.Property<int>("FileNo");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ChapterId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Manga", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("FileSystemName")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Mangas");
                });

            modelBuilder.Entity("Chapter", b =>
                {
                    b.HasOne("Manga")
                        .WithMany("Chapters")
                        .HasForeignKey("MangaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("File", b =>
                {
                    b.HasOne("Chapter")
                        .WithMany("Files")
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
