using LibraryProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BookBorrowController : ControllerBase
    {
        private readonly IBookBorrowService _bookBorrowService;

        public BookBorrowController(IBookBorrowService bookBorrowService)
        {
            _bookBorrowService = bookBorrowService;
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook([FromQuery] string userId, [FromQuery] int bookId)
        {
            try
            {
                await _bookBorrowService.CheckOutBook(userId, bookId);
                return Ok("Book borrowed successfully.");
            }
            catch (InvalidOperationException ex)
            {
                // Handle case where the user has reached the borrowing limit
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                return StatusCode(500, "An error occurred while borrowing the book.");
            }
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromQuery] string userId, [FromQuery] int bookId)
        {
            try
            {
                await _bookBorrowService.CheckInBook(userId, bookId);
                return Ok("Book returned successfully.");
            }
            catch (InvalidOperationException ex)
            {
                // Handle case where the user has reached the borrowing limit
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                return StatusCode(500, "An error occurred while borrowing the book.");
            }
        }
    }
}
