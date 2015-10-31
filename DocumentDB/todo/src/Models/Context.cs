using System.Data.Entity;

namespace todo.Models
{
    public class Context : DbContext
    {
        public Context() : base("name=Context")
        {
        }

        public DbSet<Item> Items { get; set; }
    
    }
}
