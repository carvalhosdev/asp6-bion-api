using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Repository
{
    public class RepositoryContext: IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options)
            :base(options) { }

        //MAKE CHANGES ON DATABASE
        protected override void OnModelCreating(ModelBuilder modelBuilder) //todo - info
        {
            //require migration - 291
            base.OnModelCreating(modelBuilder);

            //add other class to make update on db
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
        
    }
}
