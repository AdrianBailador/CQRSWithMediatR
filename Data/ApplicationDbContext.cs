using Microsoft.EntityFrameworkCore;
using CQRSWithMediatR.Models;

namespace CQRSWithMediatR.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}
