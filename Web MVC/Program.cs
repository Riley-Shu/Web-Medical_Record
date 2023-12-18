using Microsoft.Extensions.Configuration;
using Sample05_Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//------------註冊服務: DbContext------------
builder.Services.AddDbContext<Sample05_Web.Models.Sample05DbContext>(
    (options) =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnDbSample05"));
    }
);
//----------------------------

//------------註冊服務: ServiceURL------------
ServiceURL serviceURL = new ServiceURL();
ConfigurationManager manager = builder.Configuration;
IConfigurationSection section = manager.GetSection("ServiceURL");
section.Bind(serviceURL);
builder.Services.AddSingleton(serviceURL);
//----------------------------

//------------註冊服務: SelectListModel------------
SelectListModel selectListModel = new SelectListModel();
builder.Services.AddSingleton(selectListModel);
//------------------------




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
