using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Person.Api.Models
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions<PersonDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonType> PersonTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PersonType>().HasData(
                new PersonType { PersonTypeId = 1, Description = "Teacher" },
                new PersonType { PersonTypeId = 2, Description = "Student" }
            );
            modelBuilder.Entity<Person>().HasData(
                new Person { PersonId = 1, Name = "John Doe", Age = 30, PersonTypeId = 1 },
                new Person { PersonId = 2, Name = "Jane Smith", Age = 25, PersonTypeId = 2 },
                new Person { PersonId = 3, Name = "Alice Johnson", Age = 28, PersonTypeId = 1 },
                new Person { PersonId = 4, Name = "Bob Brown", Age = 22, PersonTypeId = 2 },
                new Person { PersonId = 5, Name = "Carol White", Age = 35, PersonTypeId = 1 },
                new Person { PersonId = 6, Name = "David Black", Age = 20, PersonTypeId = 2 },
                new Person { PersonId = 7, Name = "Eve Green", Age = 27, PersonTypeId = 1 },
                new Person { PersonId = 8, Name = "Frank Blue", Age = 23, PersonTypeId = 2 },
                new Person { PersonId = 9, Name = "Grace Red", Age = 32, PersonTypeId = 1 },
                new Person { PersonId = 10, Name = "Henry Yellow", Age = 21, PersonTypeId = 2 }
            );
        }
    }


}
