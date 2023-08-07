using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UniqChat.Models;

namespace UniqChat.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> DbOptions) : base(DbOptions)
        {


        }
        public DbSet<Users> Users { get; set; }
    }
}
