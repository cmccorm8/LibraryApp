using LibraryApp.Models;
using LibraryApp.Data;

namespace LibraryApp.Data
{
    public class MockBookRepo : IBookRepo
    {
        public Book CreateBook(Book book)
        {
            throw new NotImplementedException();
        }

        public Task<Book> CreateBookAsync(Book book)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Book>> GetAllBooksAsync()
        {
            var books =  new List<Book>
            {
                new Book{Id=0,Title="Cracking the Coding Interview", AuthorName="Gayle McDowell",IsbnNumber="9780984782857" },
                new Book{Id=1,Title="Dune",AuthorName="Frank Herbert",IsbnNumber="9780441172719"},
                new Book{Id=2,Title="Words of Radiance",AuthorName="Brandon Sanderson",IsbnNumber="9780765365286"}
            };
            
            return books;
        }

        /*public Book GetBookById(int id)
        {
            return new Book{Id=0,Title="Cracking the Coding Interview", AuthorName="Gayle McDowell",IsbnNumber="9780984782857" };

        }*/

        public Task<Book> GetBookByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(Book book)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Book> UpdateAsync(Book book)
        {
            throw new NotImplementedException();
        }

        public Task<Book> UpdateBookAsync(Book book)
        {
            throw new NotImplementedException();
        }
    }
}