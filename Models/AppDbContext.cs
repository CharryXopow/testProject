using System.Data.Entity;

namespace testProject.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=DefaultConnection") { }
    }
}