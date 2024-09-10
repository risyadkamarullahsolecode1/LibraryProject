using LibraryProject.Application.Dtos.Account;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowController(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        [Authorize(Roles = "Library User, Librarian, Library Manager")]
        [HttpPost]
        public async Task<ActionResult> SubmitBookRequestAsync(BookRequest requestDto, string userId)
        {
            await _workflowRepository.SubmitBookRequestAsync(requestDto, userId);
            return Ok("Book Request succecfully submitted");
        }

        [Authorize(Roles = "Librarian, Library Manager")]
        [HttpPut]
        public async Task<IActionResult> ApproveBookRequestAsync([FromBody] ApprovalBookRequest request)
        {
            try
            {
                var result = await _workflowRepository.ApproveBookRequestAsync(
                    request.WorkflowActionId,
                    request.ProcessId,
                    request.ActorId,
                    request.Role,
                    request.IsApproved,
                    request.Comment
                    );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
