using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//---------配置CORS---------
builder.Services.AddCors(
    (corsOption) =>
    {
        corsOption.AddPolicy("allweb",
            (builder) =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });
        corsOption.AddPolicy("myweb",
            (builder) => {
                builder.WithMethods("https://localhost:7241");
                builder.WithHeaders("https://localhost:7241");
                builder.WithOrigins("https://localhost:7241");
            });
    
    }
    );
//------------------------------------------

//------------註冊服務: DbContex------------
builder.Services.AddDbContext<Sample05_Service.Models.Sample05DbContext>(
    (options) =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnDbSample05"));
    }
);
//------------------------------------------


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//---------配置CORS---------
app.UseCors("allweb");
//---------------------------

app.Run();
