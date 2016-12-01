using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Manga
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string FileSystemName { get; set; }

    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
}

public class Chapter
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int MangaId { get; set; }
    //public long PageId{ get; set; }
    [Required]
    public float ChapterNo { get; set; }
    //public int StateId{ get; set; }
    [Required]
    public string Address { get; set; }
    public DateTime EnterDate { get; set; }

    public List<File> Files { get; set; } = new List<File>();
}

public class File
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ChapterId { get; set; }
    [Required]
    public int FileNo { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
}

public class MangaSurvContext : DbContext
{
    public MangaSurvContext(DbContextOptions<MangaSurvContext> options) : base(options)
    {
        
    }

    public DbSet<Manga> Mangas { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<File> Files { get; set; }
}