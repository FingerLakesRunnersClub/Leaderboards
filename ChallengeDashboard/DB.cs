using Microsoft.EntityFrameworkCore;

namespace ChallengeDashboard
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {
        }
    }
}