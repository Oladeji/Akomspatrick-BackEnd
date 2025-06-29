using Microsoft.EntityFrameworkCore;
using Person.Domain.Entities;
namespace Person.Infrastructure
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions<PersonDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person.Domain.Entities.Person> Persons { get; set; }
        public DbSet<PersonType> PersonTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }


}
