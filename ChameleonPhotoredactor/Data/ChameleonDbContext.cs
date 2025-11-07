using ChameleonPhotoredactor.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChameleonPhotoredactor.Data
{
    public class ChameleonDbContext : DbContext
    {
        public ChameleonDbContext(DbContextOptions<ChameleonDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageEdit> ImageEdits { get; set; }
        public DbSet<UserStats> UserStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserStats) 
                //^^ A User havr one UserStats
                .WithOne(s => s.User)       
                //^^ A UserStats is related to one User
                .HasForeignKey<UserStats>(s => s.UserId); 
                //^^ The FK is UserStats.UserId

            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Images)     
                //^^ A User have many Images
                .WithOne(i => i.User)       
                //^^ An Image is related to one User
                .HasForeignKey(i => i.UserId); 
                //^^ The FK is Image.UserId

            
            modelBuilder.Entity<Image>()
                .HasMany(i => i.Edits)      
                //^^ An Image have many Edits
                .WithOne(e => e.Image)      
                //^^ An Edit is related to one Image
                .HasForeignKey(e => e.ImageId); 
                //^^ The FK is ImageEdit.ImageId
        }
    }
}
