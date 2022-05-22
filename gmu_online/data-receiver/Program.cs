using data_receiver.Data;
using data_receiver.Models;
using data_receiver.MybackgroundService;
using Google.Api.Ads.AdWords.Lib;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);





// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHostedService<BackgroundTask>();
//add my own appClass instead of identityuser and the default identityrole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

//service to loggedin user
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<UserManager<ApplicationUser>>();


//adds
//builder.Services.AddSingleton<AdWordsAppConfig>(scope => new AdWordsAppConfig(builder.Configuration.GetSection("AdWordsApi")));
//builder.Services.AddScoped<GoogleAdWordsService>();



//add a service that can send emails
builder.Services
        .AddFluentEmail("lorenzo8399test@gmail.com")
        .AddRazorRenderer(Directory.GetCurrentDirectory())
        .AddSmtpSender(new System.Net.Mail.SmtpClient { Host = "smtp.gmail.com",Port = 587,EnableSsl =true,Credentials = new NetworkCredential("lorenzo8399test@gmail.com","Gufm2775") } )
        .AddRazorRenderer(typeof(Program));

builder.Services.AddSingleton<AdWordsAppConfig>(scope => new AdWordsAppConfig(builder.Configuration.GetSection("AdWordsApi")));
builder.Services.AddScoped<GoogleAdWordsService>();
builder.Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


//this configuration is gmail configuration

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.MapRazorPages();

app.Run();
