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
            //bookmodel.CreatedOn = DateTime.Now;
            appDbContext.Books.AddRangeAsync(booksmodel);
            await appDbContext.SaveChangesAsync();
            return Ok(booksmodel);
        }
    }
}
