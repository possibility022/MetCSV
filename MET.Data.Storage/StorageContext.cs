using MET.Data.Models;
using MET.Data.Models.Profits;
using Microsoft.EntityFrameworkCore;

namespace MET.Data.Storage
{
    public class StorageContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=metcsv.db;Cache=Shared");
        }


        public DbSet<CategoryProfit> CategoryProfits { get; set; }

        public DbSet<CustomProfit> CustomProfits { get; set; }
        public DbSet<ManufacturerProfit> ManufacturerProfits { get; set; }

        public DbSet<RenameManufacturerModel> RenameManufacturer { get; set; }
    }
}
