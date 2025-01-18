using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.Data
{
    public partial class EfDbContext
    {
        public virtual DbSet<ComCustomer> ComCustomer { get; set; }

        public virtual DbSet<SoItem> SoItem { get; set; }

        public virtual DbSet<SoOrder> SoOrder { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
