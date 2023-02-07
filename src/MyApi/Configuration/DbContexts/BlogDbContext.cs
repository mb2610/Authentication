using Microsoft.EntityFrameworkCore;
using MyApi.Configuration.Constants;
using MyApi.Configuration.DbContexts.Entities;

namespace MyApi.Configuration.DbContexts;

public class BlogDbContext : DbContext, IBlogDbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    
        ConfigureIdentityContext(builder);
    }
    private void ConfigureIdentityContext(ModelBuilder builder)
    {
        builder.Entity<Article>().ToTable(TableConsts.Articles);
        builder.Entity<Tag>().ToTable(TableConsts.Tags);
    }
}