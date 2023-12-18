using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Sample05_Web.Models
{
    public class Sample05DbContext : DbContext
    {
        //自訂建構子
        public Sample05DbContext(DbContextOptions options) : base(options)
        {

        }

        //對應資料表 (參考Model: Chart)
        public DbSet<ChartInfo> PersonalInformation { get; set; }

    }
}
