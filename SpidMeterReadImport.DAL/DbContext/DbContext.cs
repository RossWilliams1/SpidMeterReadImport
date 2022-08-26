using Microsoft.EntityFrameworkCore;
using SpidMeterReadImport.DAL.Models;

namespace SpidMeterReadImport.DAL.DataContext
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Spid> Spid { get; set; }
        public DbSet<SpidMeterRead> SpidMeterRead { get; set; }
    }
}
