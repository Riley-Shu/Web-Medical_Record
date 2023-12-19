# Summary
1. 安裝資料庫套件
2. 建立DbContext類別
	- 加入連接字串 ConnectionStrings (appsetting.josn)
	- 加入資料表範例 (項目參照資料庫)
3. Program.cs 註冊DbContext服務: AddDbContext
	- UseSqlServer 須先加入套件
	- 若有需要，註冊CORS服務
4. API控制器
	- 注入DbContext
	- 設定路徑
	- Postman測試是否能從資料庫取得資料

# Step01 建立 Web API 專案

# Step02 安裝資料庫套件
- Microsoft.EntityFrameworkCore: 
	- 它提供了一個 Object-Relational Mapping (ORM) 的框架，讓開發人員可以使用物件導向的方式來操作資料庫。
- Microsoft.EntityFrameworkCore.SqlServer: 
	- 提供了一些針對 SQL Server 資料庫的特定功能
- Microsoft.Data.SqlClient:  
	- 提供了一個低階的 API，讓開發人員可以直接使用 SQL 語句來操作資料庫。

# Step03 連接資料庫
1. 安裝套件
2. 加入連接
3. 建立連接字串
4. 利用 DbContextOptions 類別，配置 DbContext 操作資料庫。(`Microsoft.EntityFrameworkCore`)
	例如: 資料庫連線字串、資料庫提供者、資料庫初始化策略等等

![[Portfolios/Sample05_Paitent_Web/@ATT/N02-P01.png]]
## appsetting.json (Service)
- 連接字串
```json
    "ConnectionStrings": {
        "ConnectionString": "Server=;Database=Sample05;User id=oo;password=Password;application name=HR"
    }
```
- 注意: 
	若出現【憑證鏈結是不受信任的授權單位發出的】之錯誤訊息
	可嘗試在連接字串中加入 `TrustServerCertificate=true;`，以便讓用戶端信任 SQL Server 的憑證。
  ![[Portfolios/Sample05_Paitent_Web/@ATT/N02-P02.png]]
## ChartInfo.cs (Service)
- 建立Model，提供對應資料表
```cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample05_Service.Models
{
    //參考自MSSQL Table
    [Table("PersonalInformation")]
    public class Chart
    {
        [Column(name: "ChartNo")]
        [Required]
        [Key]
        public String chartNo { get; set; }

        [Column(name: "Name")]
        [Required]
        public String name { get; set; }

        [Column(name: "Sex")]
        public String sex { get; set; }

        [Column(name: "Birthday")]
        public DateTime? birthday { get; set; }

        [Column(name: "VisitDate")]
        [Required]
        public DateTime? visitDate { get;}
    }
}
```
## Sample05DbContext.cs (Service)
- 建立Model，繼承DbContext，控制資料庫功能
```cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Sample05_Service.Models
{
    public class Sample05DbContext : DbContext
    {
        //自訂建構子
        public Sample05DbContext(DbContextOptions options) : base(options)
        {
        
        }

        //對應資料表 (參考Model: Chart)
        public DbSet<Chart> PersonalInformation { get; set; }
    }
}
```
## Program.cs (Service)
- 註冊服務
-  `Microsoft.EntityFrameworkCore.SqlServer.UseSqlServer`
```cs
//------------註冊服務------------
builder.Services.AddDbContext<Sample05_Service.Models.Sample05DbContext>(
    (options) =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnDbSample05"));
    }
);
```
## qryChartNoController.cs (Service)
- 確認後端有連接上資料庫
```cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample05_Service.Models;

namespace Sample05_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class qryChartNoController : ControllerBase
    {
        private Sample05DbContext _context;
        public RecordsController(Sample05DbContext context)
        {
            _context = context;
            Console.WriteLine(_context);
        }

        [HttpGetAttribute]
        [RouteAttribute("qry/{chartNo}/rawdata")]
        [Produces("application/json")] //what is this

        public IActionResult Patient(String chartNo)
        {
            //參考自DbContext
            Console.WriteLine(_context);
            var result = (from i in _context.PersonalInformation where i.ChartNo == chartNo select i).ToList();
            return this.Ok(result);
        }
    }
}
```

![[N02-P03.png]]

![[N02-P04.png]]

# Step04 配置CORS

- 由於Web和Service設置在不同專案 (網域)中，違反 相同來源原則。故需要啟用跨原始來源要求 (CORS)
- 參考: [在 ASP.NET Core 中啟用跨原始來源要求 (CORS) | Microsoft Learn](https://learn.microsoft.com/zh-tw/aspnet/core/security/cors?view=aspnetcore-7.0)
## Program.cs (Service)
- 加入所有允許 allweb
- 加入特定允許 mvcweb
```cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//= = = = = 配置CORS = = = = = 
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
                builder.WithMethods();
                builder.WithHeaders();
                builder.WithOrigins();
            });
    
    }
    );
//= = = = = = = =

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

//Cors
app.UseCors("allweb");
//======

app.Run();
```
