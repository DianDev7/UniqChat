using Microsoft.EntityFrameworkCore;
using UniqChat.Models;

namespace UniqChat.Data
{
    //DATABASE CONTEXT
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        //USER TABEL
        public DbSet<Users> Users { get; set; }
        //UserChatHistory TABEL
        public DbSet<UserChatHistory> UserChatHistory { get; set; }
        //AddAllConnectedUsers TABEL
        public DbSet<AddAllConnectedUsers> AddAllConnectedUsers { get; set; }
        //GlobalChatHistory TABEL
        public DbSet<GlobalChatHistory> GlobalChatHistory { get; set; }
    }
}
