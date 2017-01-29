using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Model
{
    public class Manga
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FileSystemName { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime EnterDate { get; set; }

        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        public List<UserFollowMangas> FollowingUsers { get; set; } = new List<UserFollowMangas>();

        public async static void AddManga(MangaSurvContext context, Manga manga, bool bSaveChanges)
        {
            List<Chapter> lChapters = manga.Chapters;
            manga.Chapters = new List<Chapter>();

            await context.Mangas.AddAsync(manga);
            await context.SaveChangesAsync();

            lChapters.ForEach(chapter =>
            {
                chapter.MangaId = manga.Id;
                Chapter.AddChapter(context, chapter, bSaveChanges);
            });

            if (bSaveChanges)
                await context.SaveChangesAsync();
        }
    }

    public class Chapter
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long MangaId { get; set; }
        public long PageId { get; set; }
        [Required]
        public float ChapterNo { get; set; }
        public int StateId { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime EnterDate { get; set; }

        public List<File> Files { get; set; } = new List<File>();
        public List<UserNewChapters> NewChaptersForUsers { get; set; } = new List<UserNewChapters>();

        public async static void AddChapter(MangaSurvContext context, Chapter chapter, bool bSaveChanges)
        {
            List<File> lFiles = chapter.Files;
            chapter.Files = new List<File>();

            await context.Chapters.AddAsync(chapter);
            await context.SaveChangesAsync();
            
            // Add new chapters to following users
            UserNewChapters.AddChapterToUsers(context, chapter, true);

            lFiles.ForEach(file =>
            {
                file.ChapterId = chapter.Id;
                File.AddFile(context, file, false);
            });

            if (bSaveChanges)
                await context.SaveChangesAsync();
        }
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

        public async static void AddFile(MangaSurvContext context, File file, bool bSaveChanges)
        {
            await context.Files.AddAsync(file);

            if (bSaveChanges)
                await context.SaveChangesAsync();
        }
    }

    public class Page
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContentType { get; set; }
    }

    public class UserNewChapters
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public long ChapterId { get; set; }
        public Chapter Chapter { get; set; }

        public static async void AddChapterToUsers(MangaSurvContext context, Chapter chapter, bool bSaveChanges)
        {
            // Add chapter to following users
            var users = from user in context.UserFollowMangas
                        where user.MangaId == chapter.MangaId
                        select user;

            await users.ForEachAsync(user =>
            {
                UserNewChapters unc = new UserNewChapters();
                unc.ChapterId = chapter.Id;
                unc.UserId = user.UserId;
                context.UserNewChapters.Add(unc);
            });

            if (bSaveChanges)
                await context.SaveChangesAsync();
        }
    }

    public class UserFollowMangas
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public long MangaId { get; set; }
        public Manga Manga { get; set; }

        /// <summary>
        /// Adds manga to specific user.
        /// Adds manga to user 1 in general (admin user).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="manga"></param>
        public async static Task<UserFollowMangas> AddMangaToUser(MangaSurvContext context, Manga manga, User user)
        {
            UserFollowMangas ufm = new UserFollowMangas();
            ufm.Manga = manga;
            ufm.MangaId = manga.Id;
            ufm.User = user;
            ufm.UserId = user.Id;

            await context.UserFollowMangas.AddAsync(ufm);
            await context.SaveChangesAsync();

            return ufm;
        }
    }

    public class UserNewEpisodes
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public long EpisodeId { get; set; }
        public Episode Episode { get; set; }

        public static async void AddEpisodeToUsers(MangaSurvContext context, Episode episode, bool bSaveChanges)
        {
            // Add chapter to following users
            var users = from user in context.UserFollowAnimes
                        where user.AnimeId == episode.AnimeId
                        select user;

            await users.ForEachAsync(user =>
            {
                UserNewEpisodes une = new UserNewEpisodes();
                une.EpisodeId = episode.Id;
                une.UserId = user.UserId;
                context.UserNewEpisodes.Add(une);
            });

            if (bSaveChanges)
                await context.SaveChangesAsync();
        }
    }

    public class UserFollowAnimes
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public long AnimeId { get; set; }
        public Anime Anime { get; set; }

        public async static Task<UserFollowAnimes> AddAnimeToUser(MangaSurvContext context, Anime anime, User user)
        {
            UserFollowAnimes ufa = new UserFollowAnimes();
            ufa.Anime = anime;
            ufa.AnimeId = anime.Id;
            ufa.User = user;
            ufa.UserId = user.Id;

            await context.UserFollowAnimes.AddAsync(ufa);
            await context.SaveChangesAsync();

            return ufa;
        }
    }

    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<UserFollowMangas> FollowedMangas { get; set; } = new List<UserFollowMangas>();
        public List<UserNewChapters> NewChapters { get; set; } = new List<UserNewChapters>();

        public List<UserFollowAnimes> FollowedAnimes { get; set; } = new List<UserFollowAnimes>();
        public List<UserNewEpisodes> NewEpisodes { get; set; } = new List<UserNewEpisodes>();
    }

    public class Anime
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FileSystemName { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime EnterDate { get; set; }

        public List<Episode> Episodes { get; set; } = new List<Episode>();
        public List<UserFollowAnimes> FollowingUsers { get; set; } = new List<UserFollowAnimes>();

        public async static void AddAnime(MangaSurvContext context, Anime anime, bool bSaveChanges)
        {
            List<Episode> lEpisodes = anime.Episodes;
            anime.Episodes = new List<Episode>();

            await context.Animes.AddAsync(anime);
            await context.SaveChangesAsync();

            lEpisodes.ForEach(episode =>
            {
                episode.AnimeId = anime.Id;
                Episode.AddEpisode(context, episode, false);
            });

            if (bSaveChanges)
                await context.SaveChangesAsync();
        }
    }

    public class Episode
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long AnimeId { get; set; }
        [Required]
        public float EpisodeNo { get; set; }
        public int StateId { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime EnterDate { get; set; }
        public List<UserNewEpisodes> NewEpisodesForUsers { get; set; } = new List<UserNewEpisodes>();

        public void DoDefaultDate()
        {
            if (this.EnterDate == null || this.EnterDate == DateTime.MinValue)
                this.EnterDate = DateTime.Now;
        }

        public async static void AddEpisode(MangaSurvContext context, Episode episode, bool bSaveChanges)
        {
            episode.DoDefaultDate();
            await context.Episodes.AddAsync(episode);
            await context.SaveChangesAsync();

            UserNewEpisodes.AddEpisodeToUsers(context, episode, bSaveChanges);

            if (bSaveChanges)
                await context.SaveChangesAsync();
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


            modelBuilder.Entity<UserNewEpisodes>()
                .HasOne(uc => uc.User)
                .WithMany(uc => uc.NewEpisodes)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserNewEpisodes>()
                .HasOne(uc => uc.Episode)
                .WithMany(uc => uc.NewEpisodesForUsers)
                .HasForeignKey(uc => uc.EpisodeId);

            modelBuilder.Entity<UserFollowAnimes>()
                .HasOne(uc => uc.User)
                .WithMany(uc => uc.FollowedAnimes)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserFollowAnimes>()
                .HasOne(uc => uc.Anime)
                .WithMany(uc => uc.FollowingUsers)
                .HasForeignKey(uc => uc.AnimeId);
        }

        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFollowMangas> UserFollowMangas { get; set; }
        public DbSet<UserNewChapters> UserNewChapters { get; set; }

        public DbSet<Anime> Animes { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<UserFollowAnimes> UserFollowAnimes { get; set; }
        public DbSet<UserNewEpisodes> UserNewEpisodes { get; set; }
    }
}