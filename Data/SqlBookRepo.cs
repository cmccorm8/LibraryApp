using LibraryApp.Models;
using LibraryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data
{
    public class SqlBookRepo : IBookRepo
    {
        private readonly BookContext _context;

        public SqlBookRepo(BookContext context)
        {
            _context = context;
        }
        /*
            Creates a new book entry and adds to the database via _context
        */
        public async Task<Book> CreateBookAsync(Book book)
        {
            await _context.AddAsync(book);
            await SaveChangesAsync();
            return book;
        }

        /*
            Updates a book entry with new information
        */
        public async Task<Book> UpdateBookAsync(Book book)
        {
            await UpdateAsync(book);
            await SaveChangesAsync();
            return book;
        }

        /*
            Gets all of the book entries from the database via _context and put the entries into a list to be displayed
        */
        public async Task<ICollection<Book>> GetAllBooksAsync()
        {
            var books = await _context.Books.ToListAsync();
            return books;
        }

        //Non-Async version to get a single book by its id
        /*public Book GetBookById(int id)
        {
            return _context.Books.FirstOrDefault(book => book.Id == id);
            
        }*/

        /*
            Gets a single book entry based on id via _context
        */
        public async Task<Book> GetBookByIdAsync(int id)
        {
            Book? book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

            /*if(book != null)
            {
                _context.Entry(book).State = EntityState.Detached;
            }*/
            return book;
            
        }

        /*
            Utility function that saves any changes made to the database via _context
        */
        public async Task<int> SaveChangesAsync()
        {
            var dbRecord = await _context.SaveChangesAsync();
            return dbRecord;
        }

        /*
            Utility function that updates a database entry via _context
        */
        public async Task<Book> UpdateAsync(Book book)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(existing => existing.Id == book.Id);
            if(existingBook != null)
            {
                _context.Entry(existingBook).CurrentValues.SetValues(book);
                return book;
            }
            return null;
        }

        /*
            Utility function that removes an entry from the database via _context
        */
        public void Remove(Book book)
        {
            _context.Remove(book);
        }
    }
}