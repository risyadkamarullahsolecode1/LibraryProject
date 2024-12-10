using LibraryProject.Application.Dtos;
using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Services
{
    public class BookRequestService : IBookRequestService
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBookRequestRepository _bookRequestRepository;
        private readonly INextStepRuleRepository _nextStepRuleRepository;
        private readonly IWorkflowSequenceRepository _workflowSequenceRepository;
        private readonly IWorkflowActionRepository _workflowActionRepository;
        private readonly IProcessRepository _processRepository;

        public BookRequestService(IWorkflowRepository workflowRepository, IHttpContextAccessor httpContextAccessor, IEmailService emailService, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IBookRequestRepository bookRequestRepository, INextStepRuleRepository nextStepRuleRepository, IWorkflowSequenceRepository workflowSequenceRepository, IWorkflowActionRepository workflowActionRepository, IProcessRepository processRepository)
        {
            _workflowRepository = workflowRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _bookRequestRepository = bookRequestRepository;
            _nextStepRuleRepository = nextStepRuleRepository;
            _workflowSequenceRepository = workflowSequenceRepository;
            _workflowActionRepository = workflowActionRepository;
            _processRepository = processRepository;
        }

        public async Task<BaseResponseDto> SubmitJobPostRequest(BookRequestDto request)
        {
            // Get the current logged-in user
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

            // Get the user information from UserManager
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new BaseResponseDto
                {
                    Status = "Error",
                    Message = "User not found!"
                };
            }

            // Get the workflow for requests
            var workflow = await _workflowRepository.GetFirstOrDefaultAsync(w => w.WorkflowName == "Book Request");
            if (workflow == null)
            {
                return new BaseResponseDto
                {
                    Status = "Error",
                    Message = "Workflow for request not found!"
                };
            }

            // Get the current step and next step in the workflow
            var currentStepId = await _workflowSequenceRepository.GetFirstOrDefaultAsync(wfs => wfs.WorkflowId == workflow.WorkflowId);
            if (currentStepId == null)
            {
                return new BaseResponseDto
                {
                    Status = "Error",
                    Message = "Current workflow step not found!"
                };
            }

            var nextStepId = await _nextStepRuleRepository.GetFirstOrDefaultAsync(n => n.CurrentStepId == currentStepId.StepId);
            if (nextStepId == null)
            {
                return new BaseResponseDto
                {
                    Status = "Error",
                    Message = "Next step in the workflow not found!"
                };
            }

            // Create a new process for the job post request
            var newProcess = new Process
            {
                RequesterId = user.Id,
                WorkflowId = workflow.WorkflowId,
                RequestType = "Job Post",
                Status = "Pending Approval",
                RequestDate = DateTime.UtcNow,
                CurrentStepId = nextStepId.NextStepId,
            };

            await _processRepository.CreateAsync(newProcess);

            // Create a new job post request
            var newBookRequest = new BookRequest
            {
                BookTitle = request.BookTitle,
                Publisher = request.Publisher,
                Description = request.Description,
                Author = request.Author,
                RequestName = user.Id,
                ProcessId = newProcess.ProcessId,
                AppUserId = user.Id,
            };

            await _bookRequestRepository.AddAsync(newBookRequest);

            // Record the workflow action
            var newWorkflowAction = new WorkflowAction
            {
                ProcessId = newProcess.ProcessId,
                StepId = nextStepId.CurrentStepId,
                ActorId = user.Id,
                Action = "Submit",
                ActionDate = DateTime.UtcNow,
                Comment = request.Comments
            };

            await _workflowActionRepository.CreateAsync(newWorkflowAction);

            var actorEmails = new List<string>();
            // get requester email
            var requesterEmail = user.Email!;
            actorEmails.Add(requesterEmail);

            if (user != null)
            {
                var emailSubject = "Book Request Submitted";
                var emailBody = $"Dear {user.UserName},<br>Your book request for {newBookRequest.BookTitle} by {newBookRequest.Author} has been submitted and is awaiting approval.";

                // Sending email using the email from AspNetUsers
                await _emailService.SendEmailAsync(requesterEmail, emailSubject, emailBody);
            }

            return new BaseResponseDto
            {
                Status = "Success",
                Message = "Submitted job post request successfully"
            };
        }

        public async Task<BaseResponseDto> ReviewJobPostRequest(ReviewRequestDto reviewRequest)
        {
            // get user and role from httpcontextaccessor
            var userName = _httpContextAccessor.HttpContext!.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(userName!);
            var userRoles = await _userManager.GetRolesAsync(user!);
            var userRole = userRoles.Single();

            var process = await _processRepository.GetByIdAsync(reviewRequest.ProcessId);

            if (process == null)
            {
                return new BaseResponseDto
                {
                    Status = "Error",
                    Message = "Process does not exist"
                };
            }

            // check if process has a requiredRole, if null then return error
            if (process.CurrentStep.RequiredRole == null)
            {
                return new BaseResponseDto
                {
                    Status = "Error",
                    Message = "Process already finished"
                };
            }

            if (process.CurrentStep.RequiredRole.Name != userRole)
            {
                return new BaseResponseDto
                {
                    Status = "Error",
                    Message = "Unauthorize to review request"
                };
            }

            var newWorkflowAction = new WorkflowAction
            {
                ProcessId = process.ProcessId,
                StepId = process.CurrentStepId,
                ActorId = user.Id,
                Action = reviewRequest.Action,
                ActionDate = DateTime.UtcNow,
                Comment = reviewRequest.Comment
            };

            await _workflowActionRepository.CreateAsync(newWorkflowAction);

            // get nextStepId
            var nextStepRule = await _nextStepRuleRepository.GetFirstOrDefaultAsync(nsr => nsr.CurrentStepId == process.CurrentStepId && nsr.ConditionValue == reviewRequest.Action);
            var nextStepId = nextStepRule!.NextStepId;

            // update process
            process.Status = $"{reviewRequest.Action} by {userRole}";
            process.CurrentStepId = nextStepId;
            await _processRepository.UpdateAsync(process);

            var requests = await _bookRequestRepository.GetAllAsync();
            var reqData = requests.Where(r => r.ProcessId == process.ProcessId).Single();

            /**if (reviewRequest.Action == "Approved")
            {
                var newBookRequest = new BookRequest
                {
                    BookTitle = reqData.BookTitle,
                    Description = reqData.Description,
                    Author = reqData.Author,
                    Publisher = reqData.Publisher,
                    RequestName = user.Id,
                    ProcessId = process.ProcessId
                };

                await _bookRequestRepository.AddAsync(newBookRequest);
            }**/

            // send email to other actors
            // get other actors email
            //var workflowActions = await _workflowActionRepository.GetAllAsync();
            //var actorEmails = workflowActions.Where(a => a.ProcessId == process.ProcessId).Select(x => x.Actor.Email).Distinct().ToList();
            // get requester email
            //var requesterEmail = process.Requester.Email;
            // remove requesterEmail from actorEmails so requester only receive the email once
            //actorEmails.Remove(requesterEmail);

            /**if (user != null)
            {
                var emailSubject = "Book Request Submitted";
                var emailBody = $"Dear {user.UserName},<br>Your book request for has been reviewed and the answer is {reviewRequest.Comment}.";

                // Sending email using the email from AspNetUsers
                await _emailService.SendEmailAsync(requesterEmail, emailSubject, emailBody);
            }**/

            return new BaseResponseDto
            {
                Status = "Success",
                Message = "Request Reviewed Sucessfuly"
            };
        }

        public async Task<IEnumerable<object>> GetAllBookRequestStatuses()
        {
            // Get user and roles from HttpContextAccessor
            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var applications = new List<BookRequest>();

            foreach (var role in userRoles)
            {
                if (role == "Library User")
                {
                    // Fetch book requests created by the user (RequesterId)
                    var userApplications = await _bookRequestRepository.GetAllByUserAsync(r => r.AppUserId == user.Id);
                    applications.AddRange(userApplications);
                }
                else if (role == "Librarian" || role == "Library Manager")
                {
                    // Fetch book requests assigned to the role (based on the workflow step)
                    var roleApplications = await _bookRequestRepository.GetAllToStatusAsync(role);
                    applications.AddRange(roleApplications);
                }
            }

            // Include requests that are in progress for Library Users or completed but involve the current user
            var allApplications = applications.Select(app => new
            {
                RequestId = app.RequestId,
                BookTitle = app.BookTitle,
                ProcessId = app.ProcessId,
                Author = app.Author,
                Publisher = app.Publisher,
                ApplicantName = $"{app.Process?.Requester?.UserName}",
                Status = app.Process?.Status,
                CurrentStep = app.Process?.CurrentStep.StepName,  // Shows current step in the workflow
            }).ToList();

            return allApplications;
        }

        public async Task<ProcessDetailDto> GetProcessAsync(int processId)
        {
            var process = await _processRepository.GetByIdAsync(processId);
            if (process == null)
            {
                return null; // Handle the case where process is not found
            }

            // Fetch related BookRequest and WorkflowAction
            var bookRequest = await _bookRequestRepository.GetAsync(processId);
            var workflowActions = await _workflowActionRepository.GetByProcessIdAsync(process.ProcessId);

            // Construct the response DTO to include BookRequest and WorkflowActions
            var processDetailDto = new ProcessDetailDto
            {
                ProcessId = process.ProcessId,
                BookTitle = bookRequest.BookTitle,
                Author = bookRequest.Author,
                Publisher = bookRequest.Publisher,
                Status = process.Status,
                WorkflowActions = workflowActions.Select(action => new WorkflowActionDto
                {
                    ActionDate = action.ActionDate,
                    ActionBy = action.ActorId,
                    Action = action.Action,
                    Comments = action.Comment
                }).ToList()
            };

            return processDetailDto; // Return the DTO instead of the entity
        }

        public async Task<Dictionary<string, int>> CountStatusesByRole()
        {
            // Get user and roles from HttpContextAccessor
            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            // Dictionary to hold counts of statuses per role
            var roleStatusCount = new Dictionary<string, int>();

            foreach (var role in userRoles)
            {
                var applications = new List<BookRequest>();

                // Fetch book requests based on the user's role
                if (role == "Library User")
                {
                    // Fetch book requests created by the user (RequesterId)
                    var userApplications = await _bookRequestRepository.GetAllByUserAsync(r => r.AppUserId == user.Id);
                    applications.AddRange(userApplications);
                }
                else if (role == "Librarian" || role == "Library Manager")
                {
                    // Fetch book requests assigned to the role (based on the workflow step)
                    var roleApplications = await _bookRequestRepository.GetAllToStatusAsync(role);
                    applications.AddRange(roleApplications);
                }

                // Count statuses for the current role
                foreach (var app in applications)
                {
                    // Get the status of the current application process
                    var status = app.Process?.Status ?? "No Status";

                    // If the status doesn't already exist in the dictionary, initialize it
                    if (!roleStatusCount.ContainsKey(status))
                    {
                        roleStatusCount[status] = 0;
                    }

                    // Increment the count for the specific status
                    roleStatusCount[status]++;
                }
            }

            return roleStatusCount;
        }

    }
}
