//using AutoMapper;
using DbOperationsEFCore.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbOperationsEFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;
        //private readonly IMapper _mapper;

        //public CurrencyController(AppDbContext appDbContext,IMapper mapper)
        public CurrencyController(AppDbContext appDbContext)

        {
            _appDbContext = appDbContext;
            //_mapper = mapper;
        }
        [HttpGet("")]
        //public IActionResult GetAllCurrencies()
        //{
        //    //var result = _appDbContext.Currencies.ToList();


        //    //var response = _mapper.Map<List<AppCurrency>>(result);
        //    //return Ok(response);

        //    var result = (from currencies in _appDbContext.Currencies
        //                 select currencies).ToList();

        //    return Ok(result);
        //}

        public async Task<IActionResult> GetAllCurrencies()
        {
            //var result = await _appDbContext.Currencies.ToListAsync();


            //var response = _mapper.Map<List<AppCurrency>>(result);
            //return Ok(response);

            var result = await  (from currencies in _appDbContext.Currencies
                          select new Currency() { Id=currencies.Id,Title=currencies.Title}).ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAllCurrencyByIdAsync([FromRoute]int id)
        {
            var result = await _appDbContext.Currencies.FindAsync(id);   //use pk value and getting null if no records found        

            return Ok(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAllCurrencyByNameAsync([FromRoute] string name)
        {
            //var result = await _appDbContext.Currencies.Where(x=>x.Title==name).FirstOrDefaultAsync();//no error get default value null

            var result = await _appDbContext.Currencies.FirstOrDefaultAsync(x => x.Title == name);//for performance improvement

            // var result = await _appDbContext.Currencies.Where(x => x.Title == name).FirstAsync();//System.InvalidOperationException: Sequence contains no elements. 

            //var result = await _appDbContext.Currencies.Where(x => x.Title == name).SingleAsync();//System.InvalidOperationException: Sequence contains no elements. 

            //var result = await _appDbContext.Currencies.Where(x => x.Title == name).SingleOrDefaultAsync();//System.InvalidOperationException: Sequence contains more than one element.
            return Ok(result);
        }

        [HttpGet("{name}/{description}")]
        public async Task<IActionResult> GetAllCurrencyByNameAndDescAsync([FromRoute] string name, [FromRoute] string description)
        {
            var result = await _appDbContext.Currencies.FirstOrDefaultAsync(x => x.Title == name && x.Description == description);//multiple parameters
            //var result = await _appDbContext.Currencies.SingleOrDefaultAsync(x => x.Title == name && x.Description == description);//multiple parameters and no error as it uniqly identify using compsite columns
            return Ok(result);
        }

        //[HttpGet("{name}")]
        //public async Task<IActionResult> GetAllCurrencyByNameAndDescAsync1([FromRoute] string name, [FromQuery] string? description)
        //{
        //    var result = await _appDbContext.Currencies.FirstOrDefaultAsync(x => x.Title == name &&
        //    (string.IsNullOrEmpty(description) ||
        //    x.Description == description)
        //    );//multiple parameters with optional

        //    return Ok(result);
        //}


        //[HttpGet("{title}")]
        //public async Task<IActionResult> GetAllCurrencyByNameAsync1([FromRoute] string title)
        //{
        //    var result = await _appDbContext.Currencies.Where(x => x.Title == title).ToListAsync();//multiple records with same title

        //    return Ok(result);
        //}



        [HttpPost("all")]
        public async Task<IActionResult> GetAllCurrencyByIdsAsync([FromBody] List<int> Ids)
        {
            //var result = await _appDbContext.Currencies.
            //    Where(x => Ids.Contains(x.Id)).
            //    ToListAsync();
            //var result = await _appDbContext.Currencies.
            //   Where(x => Ids.Contains(x.Id)).
            //   Select(x=> new Currency()
            //   {
            //       Id=x.Id,
            //       Title=x.Title,
            //   }).               
            //   ToListAsync();

            var result = await _appDbContext.Currencies.
              Where(x => Ids.Contains(x.Id)).
              Select(x => new
              {
                  CurrencyId = x.Id,
                  CurrencyTitle = x.Title,
              }).
              ToListAsync();
            return Ok(result);
        }
    }
}
