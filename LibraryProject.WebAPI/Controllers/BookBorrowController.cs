using LibraryProject.Application.Dtos;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Services;
using LibraryProject.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BookBorrowController : ControllerBase
    {
        private readonly IBookBorrowService _bookBorrowService;
        private readonly IBookBorrowRepository _bookBorrowRepository;

        public BookBorrowController(IBookBorrowService bookBorrowService, IBookBorrowRepository bookBorrowRepository)
        {
            _bookBorrowService = bookBorrowService;
            _bookBorrowRepository = bookBorrowRepository;
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
        public async Task<IActionResult> ReturnBook([FromBody]ReturnBookRequest request)
        {
            try
            {
                await _bookBorrowService.CheckInBook(request.UserId, request.BookId, request.TanggalKembali);
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
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue()
        {
            var result = await _bookBorrowRepository.GetOverdueBorrowsByUser();
            return Ok(result);
        }

        [HttpGet("most")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _bookBorrowRepository.GetAll();
            return Ok(result);
        }
    }
}
