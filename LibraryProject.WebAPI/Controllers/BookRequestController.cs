using LibraryProject.Application.Dtos;
using LibraryProject.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BookRequestController : ControllerBase
    {
        private readonly IBookRequestService _bookRequestService;

        public BookRequestController(IBookRequestService bookRequestService)
        {
            _bookRequestService = bookRequestService;
        }
        [Authorize(Roles = "Library User")]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitBookRequest(BookRequestDto bookRequestDto)
        {
            var res = await _bookRequestService.SubmitJobPostRequest(bookRequestDto);
            return Ok(res);
        }

        [Authorize(Roles = "Librarian,Library Manager")]
        [HttpPost("review")]
        public async Task<IActionResult> ReviewBookRequest(ReviewRequestDto reviewRequestDto)
        {
            var res = await _bookRequestService.ReviewJobPostRequest(reviewRequestDto);
            return Ok(res);
        }
        [Authorize(Roles = "Library User,Librarian,Library Manager")]
        [HttpGet("status")]
        public async Task<IActionResult> GetBookRequestStatus()
        {
            try
            {
                var statuses = await _bookRequestService.GetAllBookRequestStatuses();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("{processId}")]
        public async Task<IActionResult> GetProcess(int processId)
        {
            var res = await _bookRequestService.GetProcessAsync(processId);
            return Ok(res);
        }
    }
}
