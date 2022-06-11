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
            builder.UseSerialColumns();





          


            //ignore identity tables
            builder.Ignore<IdentityUser>();

            builder.Entity<action>().HasData(
            new {  id =1 , actionName = "Currentbudget", description = "example: At 50% of the month you want to email an update with the status of the budget." },
            new {  id =2 , actionName = "Latest_videocall", description = "example: after 3 months I want to receive an email after the last video call" },
            new {  id =3 , actionName = "Latest_contact", description = "example: after 3 months I want to receive an email after the last contact call" }

            );
            builder.Entity<IdentityRole>().HasData(
                new { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", RoleName = "admin", NormalizedName = "ADMIN" }); 

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