using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UsefulWebApps.Data
{
    public class EFCoreDbContext : IdentityDbContext
    {
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                //set table names to lower case
                //the problem was development PC runs Windows, and the server runs Linux, Linux having a case-sensitive MySQL installation
                //the table names for Identity Core in the MySql database are all lower case but EF Core setting them as CamelCase
                //So just set them as lower. 
                //wasnt a problem in Windows Dev as that is NOT case sensitive but production linux server is case sensitive
                entity.SetTableName(entity.GetTableName().ToLower());
            }
        }
    }
}
