using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//---------�t�mCORS---------
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

//------------���U�A��: DbContex------------
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

//---------�t�mCORS---------
app.UseCors("allweb");
//---------------------------

app.Run();
