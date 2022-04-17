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
        public DbSet<Customer> Customer { get; set; }
        public DbSet<action> action { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<CustomerContact> CustomerContact { get; set; }
        public DbSet<UserCustomer> UserCustomer { get; set; }



        //this function gonne rename the names of the tables in my database
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


     

            //ignore identity tables
            builder.Ignore<IdentityUser>();

            //usercustomer
            builder.Entity<UserCustomer>()
        .HasKey(bc => new { bc.UserId, bc.customerId });
            builder.Entity<UserCustomer>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserCustomer)
                .HasForeignKey(bc => bc.UserId);

            builder.Entity<UserCustomer>()
                .HasOne(bc => bc.customer)
                .WithMany(c => c.UserCustomer)
                .HasForeignKey(bc => bc.customerId);

            //customercontact
            builder.Entity<CustomerContact>()
       .HasKey(bc => new { bc.contactId, bc.customerId });
            builder.Entity<CustomerContact>()
                .HasOne(bc => bc.customer)
                .WithMany(b => b.CustomerContact)
                .HasForeignKey(bc => bc.customerId);

            builder.Entity<CustomerContact>()
                .HasOne(bc => bc.contact)
                .WithMany(c => c.CustomerContact)
                .HasForeignKey(bc => bc.contactId);


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