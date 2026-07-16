using DbOperationsEFCore.Data;
using Microsoft.AspNetCore.Mvc;

namespace DbOperationsEFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(AppDbContext appDbContext) : ControllerBase
    {
        [HttpPost("")]
       public async Task<IActionResult> AddNewBook([FromBody] Book bookmodel)
        {
            //bookmodel.CreatedOn = DateTime.Now;
            appDbContext.Books.Add(bookmodel);
            await appDbContext.SaveChangesAsync();
            return Ok(bookmodel);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> AddNewBooks([FromBody] List<Book> booksmodel)
        {
            appDbContext.Books.AddRangeAsync(booksmodel);
            await appDbContext.SaveChangesAsync();
            return Ok(booksmodel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdteBook([FromRoute] int id,[FromBody] Book bookmodel)
        {
            //var book = appDbContext.Books.Find(id);
            var book=appDbContext.Books.FirstOrDefault(x=>x.Id==id);

            if (book == null)
            {
                return NotFound();
            }
            book.Title = bookmodel.Title;
            book.Description = bookmodel.Description;
            await appDbContext.SaveChangesAsync();
            return Ok(book);
        }
    }
}
