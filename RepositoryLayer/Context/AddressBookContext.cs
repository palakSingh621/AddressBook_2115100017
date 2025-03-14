using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Context
{
    public class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options) { }

        public DbSet<Entity.AddressBookEntity> AddressBookContacts { get; set; }
        public DbSet<Entity.UserEntity> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity.UserEntity>().ToTable("Users"); 
        }
    }
}
