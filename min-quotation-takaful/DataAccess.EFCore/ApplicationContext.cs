using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.EFCore //Infrastructure.Data
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //set default schema
            builder.HasDefaultSchema("dbo");

            //set primary key
            /*builder.Entity<MstUser>().HasKey(e => e.UserId);
            builder.Entity<UserRole>().HasKey(e => e.Id);
            builder.Entity<Roles>().HasKey(e => e.RoleId);
            builder.Entity<RefreshToken>().HasKey(e => e.RefreshTokenId);*/

            //set table schema
            /*builder.Entity<MstUser>().ToTable("MstUser", "dbo");
            builder.Entity<Roles>().ToTable("Roles", "dbo");
            builder.Entity<UserRole>().ToTable("UserRole", "dbo");
            builder.Entity<RefreshToken>().ToTable("RefreshToken", "dbo");*/
        }
        /*public virtual DbSet<MstUser> MstUsers { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }*/
    }
}
