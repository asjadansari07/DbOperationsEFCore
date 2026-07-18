using DbOperationsEFCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DbOperationsEFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(AppDbContext appDbContext) : ControllerBase
    {

        [HttpGet("")]
        public async Task<IActionResult> GetBooksAsync()
        {
            //var books = appDbContext.Books.ToListAsync();
            //return Ok(books);

            //var books = appDbContext.Books
            //    .Select(x=> new 
            //    {
            //    x.Id,MyBookTitile= x.Title, x.Description,x.NoOfPages
            //    }
            //    )
            //    .ToListAsync();//anonymous object


            //var books = appDbContext.Books
            //   .Select(x => new Book()
            //   {
            //       Id=x.Id,
            //       Title=x.Title,
            //       Description=x.Description,
            //       NoOfPages = x.NoOfPages
            //   }
            //   )
            //   .ToListAsync();

            //var books = appDbContext.Books
            //  .Select(x => new Book()
            //  {
            //      Id = x.Id,
            //      Title = x.Title,
            //      Description = x.Description,
            //      NoOfPages = x.NoOfPages,
            //      Author = x.Author,
            //      Language=x.Language,
            //  }
            //  )
            //  .ToListAsync();

            var books = appDbContext.Books
             .Select(x => new
             {
                 Id = x.Id,
                 Title = x.Title,
                 Description = x.Description,
                 NoOfPages = x.NoOfPages,
                 Author = x.Author != null ? x.Author.Name : "NA",
                 Language = x.Language.Title != null ? x.Language.Title : "NA",
             }
             )
             .ToListAsync();//navigation properties(other tables)
            return Ok(books);
        }

        [HttpGet("EagerLoading")]
        public async Task<IActionResult> GetBooksEagerLoadingAsync()
        {
            var books = appDbContext.Books
                .Include(x => x.Author)
                .ToListAsync();


            var book = appDbContext.Books
               .Include(x => x.Author)
               .FirstOrDefaultAsync();


            var book1 = appDbContext.Books
               .Where(x => x.Id == 4)
               .Include(x => x.Author)
               .FirstOrDefaultAsync();

            var book2 = appDbContext.Books
              .Include(x => x.Author)
              .ThenInclude(x => x.Email)
              .ThenInclude(x => new { countryName = "India", countryCode = "IN" })
              .FirstOrDefaultAsync();

            var book3 = appDbContext.Books
             .Include(x => x.Author)
             .Include(x => x.Language)
             .ToListAsync();//error object length is larger than the maximum allowed depth of 32(language has 1-* relationship).comment Books property in language class

            return Ok(books);
        }

        [HttpGet("ExplicitLoading")]
        public async Task<IActionResult> GetBooksExplicitLoadingAsync()
        {
            var book = await appDbContext.Books.FirstOrDefaultAsync();
            await appDbContext.Entry(book).Reference(x => x.Author).LoadAsync();//explicit loading 1-1


            //await appDbContext.Entry(book).Reference(x=>x.Author).Reference(x => x.Language).LoadAsync();//can't use reference twice

            //await appDbContext.Entry(book).Reference(x => x.Language).LoadAsync();//can't use reference twice, use single
            //await appDbContext.Entry(book).Reference(x=>x.Author).LoadAsync();//can't use reference twice,use single



            var languages = await appDbContext.Languages.ToListAsync();
            //foreach (var language in languages)
            //{
            //    await appDbContext.Entry(language).Collection(x=>x.Books).LoadAsync();//explicit loading 1-*
            //}

            foreach (var language in languages)
            {
                await appDbContext.Entry(language).Collection(x => x.Books)
                    .Query()
                    .Where(x => x.NoOfPages == 30)
                    .LoadAsync();//explicit loading 1-* with condition
            }


            return Ok(book);
        }

        [HttpGet("LazyLoading")]
        public async Task<IActionResult> GetBooksLazyLoadingAsync()
        {
            var book = await appDbContext.Books.FirstOrDefaultAsync();

            //var author = book.Author;//get author details as null(need to connec to db again), need to enable lazy loading using proxy package


            var author = book.Author;//after installing proxy package and register in program and marked related classes virtual, get data in this(again call db for author data)

            return Ok(book);
        }

        [HttpGet("ExecuteSQLQuery")]
        public async Task<IActionResult> GetBooksWithSqlQueryAsync()
        {
            //var authors = await appDbContext.Authors.FromSql($"select * from authors where id=2").ToListAsync();

            //var id = 1;
            //var authors = await appDbContext.Authors.FromSql($"select * from authors where id={id}").ToListAsync();


            //var authors = await appDbContext.Authors.FromSql($"select * from authors").Where(x => x.Id >1).ToListAsync();

            var colname = "id";
            var colvalue = 1;
            //var authors = await appDbContext.Authors.FromSql($"select * from authors where {colname}={colvalue}").ToListAsync();//not working with colvalue


            var param = new SqlParameter("colvalue", colvalue);
            var authors1 = await appDbContext.Authors.FromSqlRaw($"select * from authors where {colname}=@colvalue",param).ToListAsync();




            return Ok(authors1);
        }

        [HttpGet("ExecuteProcedure")]
        public async Task<IActionResult> ExecuteProcedureAsync()
        {
            //var books=await appDbContext.Authors.FromSql($"exec sp_getallbooks").ToListAsync();

            //var books1 = await appDbContext.Authors.FromSql($"exec sp_getallbooksbyid 1").ToListAsync();
            //var books2= await appDbContext.Authors.FromSql($"exec sp_getallbooksbyid {1}").ToListAsync();

            var param = new SqlParameter("@bookId", 1);

            var books3 = await appDbContext.Authors.FromSql($"exec sp_getallbooksbyid {param}").ToListAsync();

            return Ok(books3);
        }

        [HttpGet("ExecuteQueryWithoutEntity")]
        public async Task<IActionResult> ExecuteQueryWithoutEntityAsync()
        {

            //var authors = await appDbContext.Database.SqlQuery<Book>($"select * from authors").ToListAsync();

            //var authors = await appDbContext.Database.SqlQuery<Book>($"select * from authors").Where(x=>x.Id>1).ToListAsync();

            //var authors = await appDbContext.Database.SqlQuery<int>($"select id from authors").ToListAsync();
            var authors = await appDbContext.Database.ExecuteSqlAsync($"select * from authors");//no need to use tolist as it will run in db

            return Ok(authors);
        }

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
        public async Task<IActionResult> UpdteBook([FromRoute] int id, [FromBody] Book bookmodel)
        {
            //var book = appDbContext.Books.Find(id);//find by id
            var book = appDbContext.Books.FirstOrDefault(x => x.Id == id);

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
            .SetProperty(p => p.Title, p => p.Title + "updated title in bulk")
            .SetProperty(p => p.Description, "updated desc in bulk"));
            return Ok();
        }


        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBookAsync([FromRoute] int bookId)
        {
            //var book = await appDbContext.Books.FindAsync(bookId);//find by id

            //if (book == null)
            //{
            //    return NotFound();
            //}
            //appDbContext.Books.Remove(book);

            var book = new Book { Id = bookId };//need to create object like this as we are not getting model(type) from input
            appDbContext.Entry(book).State = EntityState.Deleted;
            await appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{bulk}")]
        public async Task<IActionResult> DeleteBookInBulkAsync()
        {

            //var book = new Book { Id = bookId };//need to create object like this as we are not getting model(type) from input
            //appDbContext.Entry(book).State = EntityState.Deleted;
            //await appDbContext.SaveChangesAsync();

            //var books =await appDbContext.Books.Where(x=>x.Id>3).ToListAsync();//find by id

            //if (books == null)
            //{
            //    return NotFound();
            //}

            //appDbContext.Books.RemoveRange(books);
            //await appDbContext.SaveChangesAsync();

            //var books = await appDbContext.Books.ExecuteDeleteAsync();//all records will be deleted
            var books = await appDbContext.Books.Where(x => x.Id < 8).ExecuteDeleteAsync();//can be used for single or bulk delete
            return Ok();
        }
    }
}
