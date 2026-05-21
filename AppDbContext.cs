using Microsoft.EntityFrameworkCore;

namespace DbContextPoolDebugger;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
