using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using LibraryProject.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Application.Services
{
    public class BookBorrowService : IBookBorrowService
    {
        private readonly IConfiguration _configuration;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookBorrowRepository _bookBorrowRepository;

        public BookBorrowService(IConfiguration configuration, IBookRepository bookRepository, IUserRepository userRepository, IBookBorrowRepository bookBorrowRepository)
        {
            _configuration = configuration;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _bookBorrowRepository = bookBorrowRepository;
        }

        public async Task CheckOutBook(string userId, int bookId)
        {

            var maxBooks = _configuration.GetValue<int>("LibrarySettings:MaxBooksPerUser");
            var PinjamDuration = _configuration.GetValue<int>("LibrarySettings:PinjamDuration");

            var book = await _bookRepository.GetBookById(bookId); // Await the async method
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            int currentBorrowedBookCount = await _bookBorrowRepository.GetBorrowedBooksCountByUser(userId);

            // Check if the user has already borrowed the maximum number of books
            if (currentBorrowedBookCount >= maxBooks)
            {
                throw new InvalidOperationException("User has already borrowed the maximum number of books allowed.");
            }

            //Configure Tanggal Pinjam dan kembali
            DateOnly tanggalPinjam = DateOnly.FromDateTime(DateTime.Now);
            DateOnly tanggalKembali = tanggalPinjam.AddDays(PinjamDuration);

            var borrowBook = new BookBorrow
            {
                AppUserId = userId,
                BookId = bookId,
                TanggalPinjam = tanggalPinjam,
                DueDate = tanggalKembali
            };
            await _bookBorrowRepository.AddBorrowRecord(borrowBook);
        }

        public async Task CheckInBook(string userId, int bookId, DateOnly tanggalKembali)
        {
            var PinjamDuration = _configuration.GetValue<int>("LibrarySettings:PinjamDuration");
            var Penalty = _configuration.GetValue<int>("LibrarySettings:PenaltyPerHari");

            // Fetch the borrow record for the given user and book
            var borrowRecord = _bookBorrowRepository.GetBorrowRecord(userId, bookId);
            if (borrowRecord == null) throw new Exception("Borrow record not found");

            //Configure Tanggal Pinjam dan kembali
            DateOnly tanggalPinjam = DateOnly.FromDateTime(DateTime.Now);
            DateOnly DueDate = tanggalPinjam.AddDays(PinjamDuration);

            int penalty = 0;

            if (tanggalKembali > DueDate)
            {
                int overdueDays = (tanggalKembali.ToDateTime(TimeOnly.MinValue) - DueDate.ToDateTime(TimeOnly.MinValue)).Days;
                //calculate penalty
                penalty = overdueDays * Penalty;
            }
            borrowRecord.TanggalKembali = tanggalKembali;
            borrowRecord.Penalty = penalty;
            await _bookBorrowRepository.UpdateBorrowRecord(borrowRecord);
        }

        public async Task<byte[]> GenerateUserReportPdfAsync(string userId)
        {
            var userBorrow = await _bookBorrowRepository.GetBorrowsByUserIdAsync(userId);

            var user = userBorrow.FirstOrDefault()?.AppUserId;


            string htmlContent = $"<h1>Report for book borrow</h1>";
            htmlContent += "<table>";
            htmlContent += "<tr>" +
                "<th>Full Name</th>" +
                "<th>Book Title</th>" +
                "<th>Date Borrowed</th>" +
                "<th>Date Returned</th>" +
                "<th>Days Overdue</th>" +
                "<th>Penalty</th>" +
                "</tr>";

            foreach (var borrow in userBorrow)
            {

                string fullName = $"{borrow.AppUserId}";

                htmlContent += $"<tr>" +
                               $"<td>{fullName}</td>" +
                               $"<td>{borrow.BorrowId}</td>" +
                               $"<td>{borrow.BookId:yyyy-MM-dd}</td>" +
                               $"<td>{borrow.DueDate:yyyy-MM-dd}</td>" +
                               $"<td>{borrow.TanggalPinjam:yyyy-MM-dd}</td>" +
                               $"<td>{borrow.DueDate:yyyy-MM-dd}</td>" +
                               $"<td>{borrow.TanggalKembali:yyyy-MM-dd}</td>" +
                               $"<td>{borrow.Penalty:yyyy-MM-dd}</td>" +
                               $"</tr>";
            }

            htmlContent += "</table>";

            var document = new PdfDocument();
            var config = new PdfGenerateConfig
            {
                PageOrientation = PageOrientation.Landscape,
                PageSize = PageSize.A4
            };

            string cssStr = File.ReadAllText(@"./Template/report/style1.css");
            CssData css = PdfGenerator.ParseStyleSheet(cssStr);
            PdfGenerator.AddPdfPages(document, htmlContent, config, css);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, false);

            byte[] bytes = stream.ToArray();

            return bytes;
        }

    }
}
