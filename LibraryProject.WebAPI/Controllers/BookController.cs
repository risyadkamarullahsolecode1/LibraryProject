using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Mappers;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Helpers;
using LibraryProject.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        public BookController(IBookRepository bookRepository, IBookService bookService)
        {
            _bookRepository = bookRepository;
            _bookService = bookService;
        }

        [Authorize(Roles = "Library User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookRepository.GetAllBooks();
            var bookDto = books.Select(b => b.ToBookDto()).ToList();
            return Ok(bookDto);
        }
        [Authorize(Roles = "Library User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            var bookDto = book.ToBookDto();
            return Ok(bookDto);
        }
        [Authorize(Roles = "Librarian, Library Manager")]
        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            var createdBook = await _bookRepository.AddBook(book);
            return Ok(createdBook);
        }
        [Authorize(Roles = "Librarian, Library Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id) return BadRequest();

            var createdBook = await _bookRepository.UpdateBook(book);
            var bookDto = createdBook.ToBookDto();
            return Ok(bookDto);
        }
        [Authorize(Roles = "Librarian, Library Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookRepository.DeleteBook(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok("Buku telah dihapus");
        }
        [Authorize(Roles = "Library User")]
        [HttpGet("search-book")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBookAsync([FromQuery] QueryObject query, [FromQuery] Pagination pagination)
        {
            var querybook = await _bookRepository.SearchBookAsync(query, pagination);
            return Ok(querybook);
        }
        [Authorize(Roles = "Library User")]
        [HttpGet("search-book-language")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBookLanguage([FromQuery] string language)
        {
            var booklanguage = await _bookService.SearchBookLanguage(language);
            var booklanguageDto = booklanguage.Select(x => x.ToBookDto()).ToList();
            return Ok(booklanguageDto);
        }
        [HttpPut("delete-stamp/{id}")]
        public async Task<ActionResult> DeleteStampBook(int id, string deleteStatus)
        {
            await _bookService.DeleteStampBook(id, deleteStatus);
            return Ok(new { id, deleteStatus });
        }
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalBooks()
        {
            var result = await _bookService.GetTotalBook();
            return Ok("Total book in the library : " + result);
        }

        [HttpGet("report")]
        public async Task<IActionResult> Report()
        {
            var Filename = "BookReport.pdf";
            var file = await _bookService.generatereportpdf();
            return File(file, "application/pdf", Filename);
        }
    }
}
