using LibraryProject.Application.Dtos;
using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Mappers
{
    public static class BookMapper
    {
        public static BookDto ToBookDto(this Book book)
        {
            return new BookDto
            {
                Title = book.Title,
                ISBN = book.Isbn,
                Author = book.Author,
                Category = book.Category,
                Publisher = book.Publisher,
                Description = book.Description,
                Language = book.Language,
                Location = book.Location
            };
        }
    }
}
