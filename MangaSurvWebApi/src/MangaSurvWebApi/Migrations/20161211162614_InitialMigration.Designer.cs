using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MangaSurvWebApi.Model;

namespace MangaSurvWebApi.Migrations
{
    [DbContext(typeof(MangaSurvContext))]
    [Migration("20161211162614_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Chapter", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<float>("ChapterNo");

                    b.Property<DateTime>("EnterDate");

                    b.Property<long>("MangaId");

                    b.HasKey("Id");

                    b.HasIndex("MangaId");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("File", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<long>("ChapterId");

                    b.Property<int>("FileNo");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ChapterId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Manga", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("FileSystemName")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Mangas");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserFollowMangas", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("MangaId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("MangaId");

                    b.HasIndex("UserId");

                    b.ToTable("UserFollowMangas");
                });

            modelBuilder.Entity("UserNewChapters", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ChapterId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId");

                    b.HasIndex("UserId");

                    b.ToTable("UserNewChapters");
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

            modelBuilder.Entity("UserFollowMangas", b =>
                {
                    b.HasOne("Manga", "Manga")
                        .WithMany("FollowingUsers")
                        .HasForeignKey("MangaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("User", "User")
                        .WithMany("FollowedMangas")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("UserNewChapters", b =>
                {
                    b.HasOne("Chapter", "Chapter")
                        .WithMany("NewChaptersForUsers")
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("User", "User")
                        .WithMany("NewChapters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
