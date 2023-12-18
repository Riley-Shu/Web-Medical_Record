using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample05_Service.Models;

namespace Sample05_Service.Controllers
{
    //API Controller: 管理後端
    //RecordsController: 取得MSSQL資料庫中的患者基本資歷
    [Route("api/[controller]")]
    [ApiController]
    public class QrySexController : ControllerBase
    {
        //Data
        private Sample05DbContext _context;
        //建構子注入
        public QrySexController(Sample05DbContext context)
        {
            //DbContext
            _context = context;
        }

        //Http Get取得資料，查詢對象: chartNo
        [HttpGetAttribute]
        [RouteAttribute("qry/{sex}/rawdata")]
        [Produces("application/json")] //指定回傳JSON格式
        [ProducesResponseType(typeof(List<ChartInfo>), 200)] //指定正確回傳
        [ProducesResponseType(typeof(Message), 400)] //指定錯誤回傳
        [DisableCors] //關閉Cors
        [EnableCors("myweb")] //開放Cors
        public IActionResult chartQry([FromRouteAttribute(Name = "sex")] String sex)
        {
            //Console.WriteLine(_context);
            //Part1: LINQ查詢
            var sexResult = (from i in _context.PersonalInformation where i.sex == sex select i).ToList();
            //Part2: 判斷並回傳結果
            if (sexResult.Count > 0)
            {
                return this.Ok(sexResult);
            }
            else
            {
                Message msg = new Message()
                {
                    code = 400,
                    msg = $"錯誤:",
                    msg2 = $"查無此性別 [{sex}]，請確認後重新查詢。"
                };
                return this.StatusCode(400, msg);
            }
        }
    }
}
