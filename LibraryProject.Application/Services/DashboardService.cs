using LibraryProject.Application.Dtos;
using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Services
{
    public class DashboardService:IDashboardService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookBorrowRepository _bookBorrowRepository;
        private readonly IBookRequestService _bookRequestService;
        private readonly IBookBorrowService _bookBorrowService;

        public DashboardService(IBookRepository bookRepository, IUserRepository userRepository, IBookBorrowRepository bookBorrowRepository, IBookRequestService bookRequestService, IBookBorrowService bookBorrowService)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _bookBorrowRepository = bookBorrowRepository;
            _bookRequestService = bookRequestService;
            _bookBorrowService = bookBorrowService;
        }

        public async Task<KpiReportDto> GetReport()
        {
            var totalBooks = await _bookRepository.GetTotalBooksAsync();

            var overduebooks = await _bookBorrowService.GetOverdueBorrowsByUser();

            var category = await _bookRepository.GetBooksCountByCategoryAsync();
            var member = await _bookBorrowRepository.GetBorrowCountsGroupedByMemberAsync();

            // Convert IEnumerable<(string, int)> to Dictionary<string, int>
            var members = (await _bookBorrowRepository.GetBorrowCountsGroupedByMemberAsync())
                .ToDictionary(x => x.AppUserId, x => x.BorrowCount);

            var workflow = await _bookRequestService.GetAllBookRequestStatuses();
            var process = await _bookRequestService.CountStatusesByRole();

            return new KpiReportDto
            {
                TotalBook = totalBooks,
                OverdueBooks = overduebooks,
                Category = category,
                ActiveMember = members,
                WorkflowStatus = workflow,
                TotalProcess = process,
            };

        }
    }
}
