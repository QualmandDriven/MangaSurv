using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Manga
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string FileSystemName { get; set; }

    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    public List<UserFollowMangas> FollowingUsers { get; set; } = new List<UserFollowMangas>();
}

public class Chapter
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long MangaId { get; set; }
    //public long PageId{ get; set; }
    [Required]
    public float ChapterNo { get; set; }
    //public int StateId{ get; set; }
    [Required]
    public string Address { get; set; }
    public DateTime EnterDate { get; set; }

    public List<File> Files { get; set; } = new List<File>();
    public List<UserNewChapters> NewChaptersForUsers { get; set; } = new List<UserNewChapters>();
}

public class File
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long ChapterId { get; set; }
    [Required]
    public int FileNo { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
}

public class UserNewChapters
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; }

    public long ChapterId { get; set; }
    public Chapter Chapter { get; set; }
}

public class UserFollowMangas
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; }

    public long MangaId { get; set; }
    public Manga Manga { get; set; }
}

public class User
{
    public long Id { get; set; }
    public string Name { get; set; }
    
    public List<UserFollowMangas> FollowedMangas { get; set; } = new List<UserFollowMangas>();
    public List<UserNewChapters> NewChapters { get; set; } = new List<UserNewChapters>();
}

public class Anime
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string FileSystemName { get; set; }

    public List<Episode> Episodes { get; set; } = new List<Episode>();
}

public class Episode
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long AnimeId { get; set; }
    [Required]
    public float EpisodeNo { get; set; }
    //public int StateId{ get; set; }
    [Required]
    public string Address { get; set; }
    public DateTime EnterDate { get; set; }

    public void DoDefaultDate()
    {
        if (this.EnterDate == null || this.EnterDate == DateTime.MinValue)
            this.EnterDate = DateTime.Now;
    }
}

public class MangaSurvContext : DbContext
{
    public MangaSurvContext(DbContextOptions<MangaSurvContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserNewChapters>()
            .HasOne(uc => uc.User)
            .WithMany(uc => uc.NewChapters)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserNewChapters>()
            .HasOne(uc => uc.Chapter)
            .WithMany(uc => uc.NewChaptersForUsers)
            .HasForeignKey(uc => uc.ChapterId);

        modelBuilder.Entity<UserFollowMangas>()
            .HasOne(uc => uc.User)
            .WithMany(uc => uc.FollowedMangas)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserFollowMangas>()
            .HasOne(uc => uc.Manga)
            .WithMany(uc => uc.FollowingUsers)
            .HasForeignKey(uc => uc.MangaId);
    }

    public DbSet<Manga> Mangas { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserFollowMangas> UserFollowMangas { get; set; }
    public DbSet<UserNewChapters> UserNewChapters { get; set; }

    public DbSet<Anime> Animes { get; set; }
    public DbSet<Episode> Episodes { get; set; }
}