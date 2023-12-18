using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Sample05_Service.Models;
using System;
using System.Linq;


namespace Sample05_Service.Controllers
{
    //API Controller: 管理後端
    //RecordsController: 取得MSSQL資料庫中的患者基本資歷
    [Route("api/[controller]")]
    [ApiController]
    public class QryVisitDateController : ControllerBase
    {
        //Data
        private Sample05DbContext _context;
        //建構子注入
        public QryVisitDateController(Sample05DbContext context)
        {
            //DbContext
            _context = context;
        }

        //Http Get取得資料，查詢對象: chartNo
        [HttpGetAttribute]
        [RouteAttribute("qry/{visitDate}/rawdata")]
        [Produces("application/json")] //指定回傳JSON格式
        [ProducesResponseType(typeof(List<ChartInfo>), 200)] //指定正確回傳
        [ProducesResponseType(typeof(Message), 400)] //指定錯誤回傳
        [DisableCors] //關閉Cors
        [EnableCors("myweb")] //開放Cors
        public IActionResult visitQry([FromRouteAttribute(Name = "visitDate")] DateTime visitDate)
        {
            //Console.WriteLine(_context);
            //Part1: LINQ查詢
            var visitDateResult = (from i in _context.PersonalInformation where i.visitDate == visitDate select i ).ToList();
            //var visit = visitDateResult.Distinct(from i in _context.PersonalInformation where chartNo select i).ToList();
            //Part2: 判斷並回傳結果
            String Date = visitDate.ToString("西元yyyy年MM月dd日");
            if (visitDateResult.Count > 0)
            {
                return this.Ok(visitDateResult);
            }
            else
            {
                Message msg = new Message()
                {
                    code = 400,
                    msg = $"錯誤:",
                    msg2 = $"查無 [{Date}] 看診的患者，請確認後重新查詢。"
                };
                return this.StatusCode(400, msg);
            }
        }
    }
}