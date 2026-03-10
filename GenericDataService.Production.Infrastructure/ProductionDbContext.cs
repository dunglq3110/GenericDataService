using GenericDataService.EntityModel.Production;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.Production.Infrastructure
{
    public class ProductionDbContext : DbContext
    {
        public ProductionDbContext(DbContextOptions<ProductionDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<CommonCodeMaster> CommonCodeMasters => Set<CommonCodeMaster>();
        public DbSet<CommonCodeDetail> CommonCodeDetails => Set<CommonCodeDetail>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
