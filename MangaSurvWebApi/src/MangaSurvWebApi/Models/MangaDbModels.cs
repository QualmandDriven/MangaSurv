using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;
using MangaSurvWebApi.Service;

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

        public static void AddManga(Manga manga)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                List<Chapter> lChapters = manga.Chapters;
                manga.Chapters = new List<Chapter>();

                context.Mangas.Add(manga);
                context.SaveChanges();

                lChapters.ForEach(chapter =>
                {
                    chapter.MangaId = manga.Id;
                    Chapter.AddChapter(chapter);
                });

                context.SaveChanges();
            }
        }
    }

    public class Chapter : IComparable
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

        public static void AddChapter(Chapter chapter)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                List<File> lFiles = chapter.Files;
                chapter.Files = new List<File>();

                context.Chapters.Add(chapter);
                context.SaveChanges();

                // Add new chapters to following users
                UserNewChapters.AddChapterToUsers(chapter);

                lFiles.ForEach(file =>
                {
                    file.ChapterId = chapter.Id;
                    File.AddFile(file);
                });

                context.SaveChanges();
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Chapter))
                return -1;

            //return (obj as Chapter).ChapterNo.CompareTo(this.ChapterNo);
            return this.ChapterNo.CompareTo((obj as Chapter).ChapterNo);
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

        public string Address { get; set; }

        public static void AddFile(File file)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                context.Files.Add(file);
                context.SaveChanges();
            }
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

        public static async void AddChapterToUsers(Chapter chapter)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {

                // Add chapter to following users
                var users = from user in context.UserFollowMangas
                            where user.MangaId == chapter.MangaId
                            select user.UserId;

                foreach (int userId in users)
                {
                    await AddChapterToUser(chapter, userId);
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task<UserNewChapters> AddChapterToUser(Chapter chapter, long userId)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {

                UserNewChapters unc = new UserNewChapters();
                unc.ChapterId = chapter.Id;
                unc.UserId = userId;
                context.UserNewChapters.Add(unc);
                
                await context.SaveChangesAsync();

                return unc;
            }
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
        public async static Task<UserFollowMangas> AddMangaToUser(Manga manga, User user)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
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
    }

    public class UserNewEpisodes
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public long EpisodeId { get; set; }
        public Episode Episode { get; set; }

        public static async void AddEpisodeToUsers(Episode episode)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                // Add chapter to following users
                var users = from user in context.UserFollowAnimes
                            where user.AnimeId == episode.AnimeId
                            select user.UserId;

                foreach (int userId in users)
                {
                    await AddEpisodeToUser(episode, userId);
                }
                
                await context.SaveChangesAsync();
            }
        }

        public static async Task<UserNewEpisodes> AddEpisodeToUser(Episode episode, long userId)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                UserNewEpisodes une = new UserNewEpisodes();
                une.EpisodeId = episode.Id;
                une.UserId = userId;
                context.UserNewEpisodes.Add(une);

                await context.UserNewEpisodes.AddAsync(une);
                await context.SaveChangesAsync();

                return une;
            }
        }
    }

    public class UserFollowAnimes
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public long AnimeId { get; set; }
        public Anime Anime { get; set; }

        public async static Task<UserFollowAnimes> AddAnimeToUser(Anime anime, User user)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
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
    }

    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<UserFollowMangas> FollowedMangas { get; set; } = new List<UserFollowMangas>();
        public List<UserNewChapters> NewChapters { get; set; } = new List<UserNewChapters>();

        public List<UserFollowAnimes> FollowedAnimes { get; set; } = new List<UserFollowAnimes>();
        public List<UserNewEpisodes> NewEpisodes { get; set; } = new List<UserNewEpisodes>();

        public static User GetUser(long userid, UserTokenDetails userDetails)
        {
            if (userDetails.IsVerified == false)
            {
                return null;
            }

            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                User user = null;
                if (userid > 0 && userDetails.IsAdmin)
                {
                    // Wenn mit Admin-Account auf einen anderen Benutzer zugegriffen werden soll,
                    // dann geben wir den User gleich zurück - auch wenn er nicht existiert
                    // Kein Admin legt einen Benutzer für jemand anderen an
                    // -> Wir haben dann auch keine Auth0-ID die wir setzen können
                    return context.Users.FirstOrDefault(u => u.Id == userid);
                }
                else if (userid == 0)
                {
                    user = context.Users.FirstOrDefault(u => u.Name == userDetails.id);
                }

                // Wenn Nutzer nicht existiert, dann legen wir ihn an
                if(user == null)
                {
                    user = AddUser(userDetails);
                }

                return user;
            }
        }

        public static User AddUser(UserTokenDetails userDetails)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                User user = new User();
                user.Name = userDetails.id;
                context.Users.Add(user);
                context.SaveChanges();

                return user;
            }
        }
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

        public static void AddAnime(Anime anime)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                List<Episode> lEpisodes = anime.Episodes;
                anime.Episodes = new List<Episode>();

                context.Animes.Add(anime);
                context.SaveChanges();

                lEpisodes.ForEach(episode =>
                {
                    episode.AnimeId = anime.Id;
                    Episode.AddEpisode(episode);
                });
                
                context.SaveChanges();
            }
        }
    }

    public class Episode : IComparable
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

        public static void AddEpisode(Episode episode)
        {
            using (MangaSurvContext context = ApplicationDependencies.GetMangaSurvContext())
            {
                episode.DoDefaultDate();
                context.Episodes.Add(episode);
                context.SaveChanges();

                UserNewEpisodes.AddEpisodeToUsers(episode);
                
                context.SaveChanges();
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Episode))
                return -1;

            //return (obj as Episode).CompareTo(this.EpisodeNo);
            return this.EpisodeNo.CompareTo((obj as Episode).EpisodeNo);
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