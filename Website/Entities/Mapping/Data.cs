using Microsoft.EntityFrameworkCore;

using App.Domain;
using System.Reflection.Emit;

namespace App.Data
{
    public partial class EfDbContext
    {
        private void Data_Mapping(ModelBuilder builder)
        {
            builder.Entity<SoItem>(entity =>
            {
                entity.HasKey(e => e.SoItemId);
                entity.Property(e => e.SoItemId).ValueGeneratedOnAdd();
                entity.Property(e => e.ItemName).HasDefaultValueSql("('')");
                entity.Property(e => e.Quantity).HasDefaultValueSql("((-99))");
                entity.Property(e => e.SoOrderId).HasDefaultValueSql("((-99))");
            });

            builder.Entity<SoOrder>(entity =>
            {
                entity.HasKey(e => e.SoOrderId); 
                entity.Property(e => e.SoOrderId).ValueGeneratedOnAdd();
                entity.Property(e => e.Address).HasDefaultValueSql("('')");
                entity.Property(e => e.ComCustomerId).HasDefaultValueSql("('-99')");
                entity.Property(e => e.OrderDate).HasDefaultValueSql("('1900-01-01')");
                entity.Property(e => e.OrderNo).HasDefaultValueSql("('')");
            });
        }
    }
}
