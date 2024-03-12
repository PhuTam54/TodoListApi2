using Microsoft.EntityFrameworkCore;

namespace TodoApi2
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Todo> Todo => Set<Todo>();
    }
}
