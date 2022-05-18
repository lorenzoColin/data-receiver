using data_receiver.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace data_receiver.Data
{
    //this file is the connection to my database
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<action> action { get; set; }
        //public DbSet<Contact> Contact { get; set; }
        //public DbSet<CustomerContact> CustomerContact { get; set; }
        public DbSet<UserCustomer> UserCustomer { get; set; }
        public DbSet<UserCustomerAction> UserCustomerAction { get; set; }

        



        //this function gonne rename the names of the tables in my database
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


     

        //ignore identity tables
        builder.Ignore<IdentityUser>();

        //usercustomer
        //    builder.Entity<UserCustomer>()
        //.HasKey(bc => new { bc.UserId });
            //builder.Entity<UserCustomer>()
            //    .HasOne(bc => bc.us)
            //    .WithMany(b => b.UserCustomer)
            //    .HasForeignKey(bc => bc.UserId);


            //customercontact
       //    builder.Entity<UserCustomerAction>()
       //.HasKey(bc => new { bc.actionId,bc.usercustomerId });
            builder.Entity<UserCustomerAction>()
                .HasOne(bc => bc.UserCustomer)
                .WithMany(b => b.UserCustomerAction)
                .HasForeignKey(bc => bc.usercustomerId);

            builder.Entity<UserCustomerAction>()
                .HasOne(bc => bc.action)
                .WithMany(c => c.UserCustomerAction)
                .HasForeignKey(bc => bc.actionId);


            builder.HasDefaultSchema("Identity");
           

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

         
    }
       
  }
}