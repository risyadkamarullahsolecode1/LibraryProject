using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Infrastructure.Data.Repository
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly LibraryProjectContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        public WorkflowRepository(LibraryProjectContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _emailService = emailService;
        }

        // Add workflow
        public async Task<Workflow> AddWorkflow(Workflow workflow)
        {
            await _context.Workflows.AddAsync(workflow);
            await _context.SaveChangesAsync();
            return workflow;
        }

        // add workflow sequence
        public async Task<WorkflowSequence> AddWorkflowSequence(WorkflowSequence workflowSequence)
        {
            await _context.WorkflowSequences.AddAsync(workflowSequence);
            await _context.SaveChangesAsync();
            return workflowSequence;
        }

        // add leave request
        public async Task<BookRequest> SubmitLeaveRequestAsync(BookRequest request)
        {
            await _context.BookRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<Process> AddProcessLeaveRequest(Process process)
        {
            await _context.Processs.AddAsync(process);
            await _context.SaveChangesAsync();
            return process;
        }
        public async Task<WorkflowAction> AddAction(WorkflowAction workflowAction)
        {
            await _context.WorkflowActions.AddAsync(workflowAction);
            await _context.SaveChangesAsync();
            return workflowAction;
        }
        public async Task SubmitBookRequestAsync(BookRequest requestDto, string userId)
        {
            // Initialize process
            var process = new Process
            {
                WorkflowId = 1, // ID for Book Request Workflow
                RequesterId = userId,
                RequestType = "Book Request",
                Status = "Pending Librarian Approval",
                CurrentStepId = 2, // Step ID for Librarian Approval
                RequestDate = DateTime.UtcNow
            };

            _context.Processs.Add(process);
            await _context.SaveChangesAsync();

            // Add book request
            var bookRequest = new BookRequest
            {
                BookTitle = requestDto.BookTitle,
                RequestName = requestDto.RequestName,
                Description = requestDto.Description,
                Author = requestDto.Author,
                Publisher = requestDto.Publisher,
                ProcessId = process.ProcessId,
                AppUserId = userId,
            };

            _context.BookRequests.Add(bookRequest);
            await _context.SaveChangesAsync();

            var action = new WorkflowAction
            {
                ProcessId = process.ProcessId,
                StepId = 1,
                ActorId = userId,
                Action = process.Status,
                ActionDate = DateTime.UtcNow,
                Comment = "Submit a book request"
            };

            _context.WorkflowActions.Add(action);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var emailSubject = "Book Request Submitted";
                var emailBody = $"Dear {user.UserName},<br>Your book request for {bookRequest.BookTitle} by {bookRequest.Author} has been submitted and is awaiting approval.";

                // Sending email using the email from AspNetUsers
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
        }

        public async Task<bool> ApproveBookRequestAsync(int workflowActionId, int processId, string actorId, string role, bool isApproved, string comment)
        {
            var userRoles = _httpContextAccessor.HttpContext.User.Claims
                  .Where(c => c.Type == ClaimTypes.Role)
                  .Select(c => c.Value)
            .ToList();

            var bookRequest = await _context.BookRequests.FirstOrDefaultAsync(br => br.ProcessId == processId);
            if (bookRequest == null)
            {
                throw new Exception("Book request not found.");
            }

            var currentAction = await _context.WorkflowActions
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ProcessId == processId && a.ActorId == actorId && a.StepId == 1);
            if (currentAction == null)
            {
                throw new Exception("Current action not found.");
            }
            // Initialize variables for StepId and Status
            int nextStepId = 0;
            string nextProcessStatus = string.Empty;
            string emailSubject = string.Empty;
            string emailBody = string.Empty;
            string employeeEmail = await GetUserEmailById(bookRequest.AppUserId);


            // Supervisor approval flow
            if (userRoles.Contains("Librarian"))
            {
                if (isApproved)
                {
                    // Move to HR approval if Supervisor approves
                    nextStepId = 3; // StepId for HR Manager approval
                    nextProcessStatus = "Pending Library Manager Approval";
                    bookRequest.Description = "Pending Library Manager Approval";
                    // Email notification to HR Manager
                    emailSubject = "Leave Request Pending Library Manager Approval";
                    emailBody = $"The book request for {bookRequest.RequestName} is pending Library Manager approval.";
                }
                else
                {
                    // If Supervisor rejects, end the process
                    nextStepId = 5; // Rejected
                    nextProcessStatus = "Rejected";
                    bookRequest.Description = "Rejected by Librarian";
                    // Email notification to Employee
                    emailSubject = "Book Request Rejected";
                    emailBody = $"Your book request '{bookRequest.RequestName}' has been rejected by the librarian.";
                }

                // Create new action for HR or rejection
                var newAction = new WorkflowAction
                {
                    ProcessId = processId,
                    StepId = isApproved ? nextStepId : 4, // Next step for HR if approved
                    ActorId = isApproved ? "dba97099-e473-4a05-ba33-ab666a5d5b24" : actorId,
                    Action = isApproved ? "Pending Library Manager Approval" : "Rejected by Librarian",
                    ActionDate = DateTime.UtcNow,
                    Comment = comment
                };

                _context.WorkflowActions.Update(newAction);
            }
            // HR Manager approval flow
            else if (userRoles.Contains("Library Manager"))
            {
                if (isApproved)
                {
                    // Approve the leave request
                    nextStepId = 4; // StepId for final approval
                    nextProcessStatus = "Approved";
                    bookRequest.Description = "Approved by Library Manager";
                    // Email notification to Employee
                    emailSubject = "Book Request Approved";
                    emailBody = $"Your book request '{bookRequest.RequestName}' has been approved by Library Manager.";
                }
                else
                {
                    // Reject the leave request
                    nextStepId = 5; // Rejected
                    nextProcessStatus = "Rejected";
                    bookRequest.Description = "Rejected by Library Manager";
                    // Email notification to Employee
                    emailSubject = "Book Request Rejected";
                    emailBody = $"Your book request '{bookRequest.RequestName}' has been rejected by Library Manager.";
                }

                // Update the HR action in WorkflowActions
                var mgrAction = new WorkflowAction
                {
                    ProcessId = processId,
                    StepId = nextStepId,
                    ActorId = actorId,
                    Action = isApproved ? "Approved by Library Manager" : "Rejected by Library Manager",
                    ActionDate = DateTime.UtcNow,
                    Comment = comment
                };

                _context.WorkflowActions.Update(mgrAction);
            }
            else
            {
                throw new UnauthorizedAccessException("You are not authorized to approve this request.");
            }

            // Update the Process
            var process = await _context.Processs.FindAsync(processId);
            if (process != null)
            {
                process.Status = nextProcessStatus;
                process.CurrentStepId = nextStepId;
            }

            await _context.SaveChangesAsync();
            // Send email notification
            await _emailService.SendEmailAsync(employeeEmail, emailSubject, emailBody);
            return true;
        }

        private async Task<string> GetUserEmailById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user?.Email;
        }

        public async Task<IEnumerable<Process>> GetProcess()
        {
           var processes = await _context.Processs
                .ToListAsync();

            return processes;
        }
    }
}
