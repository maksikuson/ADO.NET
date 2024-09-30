using GenFu.ValueGenerators.Music;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AdoNet_5
{
    public class MusicDbContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Country> Countrys { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Category> Categorys { get; set; }

        public MusicDbContext()
        {
            this.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-9U0F2B0\SQLEXPRESSS;
                                      Initial Catalog = MusicDB;
                                      Integrated Security=True;
                                      Connect Timeout=30;
                                      Encrypt=False;");
        }
        public static void Initialize(MusicDbContext context)
        {
            if (context.Artists.Any() && context.Albums.Any() && context.Tracks.Any() && context.Playlists.Any())
            {
                return;
            }
            context.Countrys.Add(new Country() { Name = "USA" });
            context.Countrys.Add(new Country() { Name = "Ukraine" });
            context.Categorys.Add(new Category() { Name = "Favorite" });
            context.Genres.Add(new Genre() { Name = "New Metal" });
            context.SaveChanges();

            // Додавання артистів
            var artist1 = new Artist { FirstName = "Slipknot", LastName = "Band", CountryId = 3 }; 
            var artist2 = new Artist { FirstName = "Korn", LastName = "Band", CountryId = 4 }; 
            context.Artists.AddRange(artist1, artist2);
            context.SaveChanges();

            // Додавання альбомів
            var album1 = new Album { Name = "Iowa", Year = 2001, GenreId = 6, ArtistId = artist1.Id };
            var album2 = new Album { Name = "Follow the Leader", Year = 1998, GenreId = 6, ArtistId = artist2.Id };
            context.Albums.AddRange(album1, album2);
            context.SaveChanges();

            // Додавання треків
            var track1 = new Track { Name = "Psychosocial", Duration = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(46)), AlbumId = album1.Id };
            var track2 = new Track { Name = "Left Behind", Duration = TimeSpan.FromMinutes(3.38), AlbumId = album1.Id };
            var track3 = new Track { Name = "Freak on a Leash", Duration = TimeSpan.FromMinutes(4.00), AlbumId = album2.Id };
            context.Tracks.AddRange(track1, track2, track3);
            context.SaveChanges();

            // Додавання плейлистів
            CreatePlaylist(context, "New Metal Hits", "New Metal", new List<int> { track1.Id, track2.Id });
            CreatePlaylist(context, "Classic New Metal", "New Metal", new List<int> { track3.Id });
        }

        public static void CreatePlaylist(MusicDbContext context, string name, string categoryName, List<int> trackIds)
        {
            var category = context.Categorys.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
                category = new Category { Name = categoryName };
                context.Categorys.Add(category);
                context.SaveChanges();
            }

            var tracks = context.Tracks.Where(t => trackIds.Contains(t.Id)).ToList();

            var playlist = new Playlist
            {
                Name = name,
                Category = category,
                Tracks = tracks
            };
            context.Playlists.Add(playlist);
            context.SaveChanges();
        }

    }
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Artist> Artists { get; set; }
    }
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
    public class Artist
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Country Country { get; set; }
        public int CountryId { get; set; }
        public ICollection<Album> Albums { get; set; }
    }

    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public Genre Genre { get; set; }
        public int GenreId { get; set; }
        public Artist Artist { get; set; }
        public int ArtistId { get; set; }
        public ICollection<Track> Tracks { get; set; }
    }

    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public Album Album { get; set; }
        public int AlbumId { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }

    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public ICollection<Track> Tracks { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new MusicDbContext())
            {

                MusicDbContext.Initialize(context);
            }
        }
    }
}
