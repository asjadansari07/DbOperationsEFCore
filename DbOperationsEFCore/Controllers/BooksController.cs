using DbOperationsEFCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            //var book = appDbContext.Books.Find(id);//find by id
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


        [HttpPut("")]
        public async Task<IActionResult> UpdteBookWithSingleQuery([FromBody] Book bookmodel)
        {
           appDbContext.Books.Update(bookmodel);//drawback is need to pass mandatory fields for model and database, no bluk update
            //appDbContext.Entry(bookmodel).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await appDbContext.SaveChangesAsync();
            return Ok(bookmodel);
        }

        [HttpPut("bulk")]
        public async Task<IActionResult> UpdteBookInBulk()
        {
            //var books=appDbContext.Books.ToList();//get all books hit db 1st time

            //foreach (var item in books)
            //{
            //    item.Title = "updated title in bulk ony by one";//n+1 query issue-update each record one by one, not good for performance
            //}
            //await appDbContext.SaveChangesAsync();


            await appDbContext.Books
             //.Where(x=>x.NoOfPages==20) 
             .ExecuteUpdateAsync(x => x
            .SetProperty(p => p.Title, p.Title+"updated title in bulk")
            .SetProperty(p => p.Description, "updated desc in bulk"));
            return Ok();
        }
    }
}
