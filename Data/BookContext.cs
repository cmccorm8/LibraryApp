using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> opt) : base(opt)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            CreateAndSeedBookExamples(builder);
        }

        public DbSet<Book> Books { get; set; }

        //Creates and adds seed data to the database
        private void CreateAndSeedBookExamples(ModelBuilder builder)
        {
            builder
                .Entity<Book>(entity =>
                {
                    entity.HasKey(k => k.Id);
                    entity.ToTable("Books");
                    entity.HasData(
                        new Book
                        {
                            Id=1,
                            Title="Cracking the Coding Interview", 
                            AuthorName="Gayle McDowell",
                            IsbnNumber="9780984782857" 
                        },
                        new Book
                        {
                            Id=2,
                            Title="Dune",
                            AuthorName="Frank Herbert",
                            IsbnNumber="9780441172719"
                            },
                        new Book
                        {
                            Id=3,
                            Title="Words of Radiance",
                            AuthorName="Brandon Sanderson",
                            IsbnNumber="9780765365286"
                        }
                    );  
                });
        }
    }
}