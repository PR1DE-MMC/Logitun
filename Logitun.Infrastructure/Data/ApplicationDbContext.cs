using Logitun.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Logitun.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Credentials> AuthCredentials { get; set; }
        public DbSet<Role> AuthRoles { get; set; }
        public DbSet<PersonalInformation> AuthInformation { get; set; }
        public DbSet<User> AuthUsers { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<TimeOffRequest> TimeOffRequests { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Mission> Missions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Constraints
            modelBuilder.Entity<Credentials>()
                .HasIndex(c => c.Login)
                .IsUnique();

            modelBuilder.Entity<PersonalInformation>()
                .HasIndex(i => i.Email)
                .IsUnique();
            modelBuilder.Entity<PersonalInformation>()
                .HasIndex(i => i.Cin)
                .IsUnique();
            modelBuilder.Entity<PersonalInformation>()
                .HasIndex(i => i.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.InformationId)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.CredentialsId)
                .IsUnique();

            // Many-to-Many Relationship between User and Role
            modelBuilder.Entity<Credentials>()
            .HasMany(ac => ac.Roles)
            .WithMany(ar => ar.Credentials)
            .UsingEntity<Dictionary<string, object>>(
                "auth_user_role",
                j => j
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("role_name")
                    .HasConstraintName("FK_auth_credentials_role_auth_role"),
                j => j
                    .HasOne<Credentials>()
                    .WithMany()
                    .HasForeignKey("credential_id")
                    .HasConstraintName("FK_auth_credentials_role_auth_credentials"),
                j =>
                {
                    j.HasKey("credential_id", "role_name");
                    j.ToTable("auth_credentials_role");
                });


            // One-to-One AuthUser → AuthCredentials, AuthInformation
            modelBuilder.Entity<User>()
                .HasOne(u => u.Credentials)
                .WithOne(c => c.User)
                .HasForeignKey<User>(u => u.CredentialsId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Information)
                .WithOne(i => i.User)
                .HasForeignKey<User>(u => u.InformationId);

            // TimeOffRequest → AuthUser
            modelBuilder.Entity<TimeOffRequest>()
                .HasOne(t => t.Driver)
                .WithMany(u => u.TimeOffRequests)
                .HasForeignKey(t => t.DriverId);

            // Mission Relationships
            modelBuilder.Entity<Mission>()
                .HasOne(m => m.Truck)
                .WithMany(t => t.Missions)
                .HasForeignKey(m => m.TruckId);

            modelBuilder.Entity<Mission>()
                .HasOne(m => m.Driver)
                .WithMany(u => u.Missions)
                .HasForeignKey(m => m.DriverId);

            modelBuilder.Entity<Mission>()
                .HasOne(m => m.OriginLocation)
                .WithMany(l => l.OriginMissions)
                .HasForeignKey(m => m.OriginLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mission>()
                .HasOne(m => m.DestinationLocation)
                .WithMany(l => l.DestinationMissions)
                .HasForeignKey(m => m.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}