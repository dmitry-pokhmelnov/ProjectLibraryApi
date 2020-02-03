using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectsLibrary.Models
{
    public class LibraryContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Geometry> Geometries { get; set; }
        public DbSet<Attribute> Attributes { get; set; }

        public LibraryContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=library;Username=postgres;Password=1234");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Project>()
            .HasMany(c => c.Geometries)
            .WithOne(e => e.Project)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
            .HasMany(c => c.Attributes)
            .WithOne(e => e.Project)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Geometry>()
            .HasMany(c => c.Attributes)
            .WithOne(e => e.Geometry);
        }
    }

    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; }

        public List<Geometry> Geometries { get; set; }
        public List<Attribute> Attributes { get; set; }
    }

    public class Geometry
    {
        [Key]
        public int Id { get; set; }

        public string GeometryName { get; set; }

        [ForeignKey("ProjectFK")]
        public string ProjectFK {get;set;}
        public Project Project { get; set; }
        public List<Attribute> Attributes { get; }
    }

    public class Attribute
    {
        [Key]
        public int Id { get; set; }
        public string AttributeName { get; set; }

        [ForeignKey("ProjectFK")]
        public string ProjectFK { get; set; }
        public Project Project { get; set; }
        public Geometry Geometry { get; set; }

    }
}
