using Microsoft.EntityFrameworkCore;

namespace DbOperationsEFCore.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {
            
        }
    }
}
