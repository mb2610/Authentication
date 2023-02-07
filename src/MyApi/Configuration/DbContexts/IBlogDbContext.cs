using Microsoft.EntityFrameworkCore;
using MyApi.Configuration.DbContexts.Entities;

namespace MyApi.Configuration.DbContexts;

public interface IBlogDbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Tag> Tags { get; set; }
}