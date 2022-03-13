using LibraryApp.Models;

namespace LibraryApp.Data
{
    public interface IBookRepo
    {
        Task<int> SaveChangesAsync();
        Task<ICollection<Book>> GetAllBooksAsync();
        //Book GetBookById(int id);
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task<Book> UpdateAsync(Book book);
        void Remove(Book book);
        
    }
}