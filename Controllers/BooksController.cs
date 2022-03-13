using System.Text.Json;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    // api/books
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly IBookRepo _repository;
        //private readonly MockBookRepo _repository = new Data.MockBookRepo();
        private readonly ILogger<BooksController> _logger;


        public BooksController(IBookRepo repository, ILogger<BooksController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        //GET api/books
        [HttpGet]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            try
            {
                var books = await _repository.GetAllBooksAsync();

                if(books != null && books.Any())
                {
                    _logger.LogTrace($"BooksController: GetAllBooksAsync succeeded.");
                    return Ok(books);
                }
                _logger.LogWarning($"BooksController: GetAllBooksAsync had no results.");
                return NoContent();

                
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, $"BooksController: GetAllBooksAsync failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //GET /api/books/{id}
        [HttpGet("{id}", Name="GetBookByIdAsync")]
        public async Task<ActionResult<Book>> GetBookByIdAsync(int id)
        {
            try
            {
                if(id==0)
                {
                    _logger.LogWarning($"BookController: GetBookByIdAsync had no id");
                    return BadRequest();
                }

                _logger.LogInformation($"BookController: GetBookByIdAsync, getting Book by Id {id}");
                var book = await _repository.GetBookByIdAsync(id);

                if(book != null)
                {
                    _logger.LogInformation($"BookController: GetBookByIdAsync for id {id} succeeded.");
                    return Ok(book);
                }
                _logger.LogWarning($"BookController: GetBookByIdAsync for id {id} not found.");
                return NotFound();
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, $"BookController: GetBookByIdAsync for id {id} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, e);

            }
        }


        //Utility function to get a book based on id
        public async Task <Book> GetBookById(int id)
        {
            var book = await _repository.GetBookByIdAsync(id);
            return book;
            
        }

        //POST /api/books
        [HttpPost]
        public async Task<IActionResult> CreateBookAsync([FromBody] Book book)
        {
            var jsonString = JsonSerializer.Serialize(book);
            try
            {
                if(book == null || book.Id != 0 || !ModelState.IsValid)
                {
                    _logger.LogWarning(
                        $"BookController: CreateBookAsync for {jsonString} is invalid."
                    );
                    return BadRequest();
                }

                _logger.LogInformation(
                    $"BookController: CreateBookAsync, creating {jsonString}."
                );
                //_repository.Add(book);
                //var created = await _repository.SaveChangesAsync();
                var created = await _repository.CreateBookAsync(book);

                _logger.LogInformation(
                    $"BookController: CreateBookAsync creating {jsonString} successful."
                );
                return StatusCode(StatusCodes.Status201Created, created);
            }
            catch(Exception e)
            {
                _logger.LogError(
                    $"BookController: CreateBookAsync failed."
                );
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //PUT /api/books
        [HttpPut]
        public async Task<IActionResult> UpdateBookAsync([FromBody] Book book)
        {
            var jsonString = JsonSerializer.Serialize(book);
            try
            {
                if(book == null || !ModelState.IsValid)
                {
                    _logger.LogWarning(
                        $"BookController: UpdateBookAsync for {jsonString} is invalid."
                    );
                    return BadRequest();
                }

                _logger.LogInformation(
                    $"BookController: UpdateBookAsync, updating {jsonString}."
                );
                //_repository.Add(book);
                //var created = await _repository.SaveChangesAsync();
                var result = await _repository.UpdateBookAsync(book);

                if(result != null)
                {
                    _logger.LogInformation(
                    $"BookController: UpdateBookAsync updating {jsonString} successful."
                    );
                    return Ok(result);
                }

                _logger.LogWarning(
                    $"BookController: UpdateBookAsync, updating {jsonString} not found."
                );
                return NotFound();
                
            }
            catch(Exception e)
            {
                _logger.LogError(
                    $"BookController: UpdateBookAsync failed."
                );
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //DELETE /api/books/{id}
        [HttpDelete("{id}")]
        public async Task <IActionResult> DeleteBookAsync(int id)
        {
            try
            {
                _logger.LogWarning($"BookController: DeleteBookAsync had no id.");
                if(id == 0)
                {
                    return BadRequest();
                }

                _logger.LogInformation($"BookController: DeleteBookAsync, deleting for id {id}.");
                var bookModel = await GetBookById(id);
                _repository.Remove(bookModel);
                await _repository.SaveChangesAsync();
                _logger.LogInformation($"BookController: DeleteBookAsync, deleting for id {id} succeeded.");
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"BookController: DeleteBookAsync for id {id} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }

        }

    }
}