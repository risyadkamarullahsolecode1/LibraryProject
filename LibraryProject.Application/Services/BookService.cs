using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace LibraryProject.Application.Services
{
    public class BookServices : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookServices(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> SearchBookLanguage(string language)
        {
            var languagebook = await _bookRepository.GetAllBooks();

            return languagebook
                .Where(book => book.Language == language)
                .ToList();
        }

        public async Task DeleteStampBook(int id, string deleteStatus)
        {
            var deleted = await _bookRepository.GetBookById(id);
            if (deleted == null)
            {
                throw new Exception($"Book with Id {id} not found");
            }
            deleted.DeleteStamp = true;
            deleted.DeleteStatus = deleteStatus;

            await _bookRepository.UpdateBook(deleted);
            await _bookRepository.SaveChangesAsync();
        }
        public async Task<int> GetTotalBook()
        {
            return await _bookRepository.GetTotalBooksAsync();
        }

        // SRS-037 information book purchase
        public async Task<byte[]> generatereportpdf()
        {
            var bookList = await _bookRepository.GetAllBooks();

            string htmlcontent = String.Empty;

            htmlcontent += "<h1> Book Report </h1>";

            htmlcontent += "<table>";

            htmlcontent += "<thead><tr><td>Id</td><td>Title</td><td>Author</td><td>Publisher</td><td>Price</td><td>Category</td></tr></thead>";

            bookList.ToList().ForEach(item => {
                htmlcontent += "<tr>";
                htmlcontent += "<td>" + item.Id + "</td>";
                htmlcontent += "<td>" + item.Title + "</td>";
                htmlcontent += "<td>" + item.Author + "</td>";
                htmlcontent += "<td>" + item.Publisher + "</td>";
                htmlcontent += "<td>" + item.Price + "</td>";
                htmlcontent += "<td>" + item.Category + "</td>";
                htmlcontent += "</tr>";
            });

            htmlcontent += "</table>";

            var document = new PdfDocument();

            var config = new PdfGenerateConfig();

            config.PageOrientation = PageOrientation.Landscape;

            config.PageSize = PageSize.A4;

            string cssStr = File.ReadAllText(@"./Template/ReportTemplates/style.css");

            CssData css = PdfGenerator.ParseStyleSheet(cssStr);

            PdfGenerator.AddPdfPages(document, htmlcontent, config, css);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, false);

            byte[] bytes = stream.ToArray();

            return bytes;
        }

        public async Task<Dictionary<string, int>> GetBookByCategory()
        {
            return await _bookRepository.GetBooksCountByCategoryAsync();
        }
    }
}
