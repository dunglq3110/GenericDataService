using GenericDataService.GenericService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService
{
    public class GenericServiceDbContext : DbContext
    {
        public GenericServiceDbContext(DbContextOptions<GenericServiceDbContext> options) : base(options) { }

        public DbSet<ServicePermission> ServicePermissions => Set<ServicePermission>();
        public DbSet<ConnectionRegistry> ConnectionRegistries => Set<ConnectionRegistry>();
        public DbSet<UserEventRegistration> UserEventRegistrations => Set<UserEventRegistration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
