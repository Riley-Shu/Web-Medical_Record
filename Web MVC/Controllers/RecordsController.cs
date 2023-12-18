using Microsoft.AspNetCore.Mvc;
using Sample05_Web.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Sample05_Web.Controllers
{
    public class RecordsController : Controller
    {
        //Data
        private Sample05DbContext _context;
        private ServiceURL _serviceURL;
        private SelectListModel _selectListModel;

        //建構子注入
        public RecordsController(Sample05DbContext context, ServiceURL serviceURL, SelectListModel selectListModel)
        {
            _context = context;
            _serviceURL = serviceURL;
            _selectListModel = selectListModel;
        }

        //Web
        public IActionResult Patients()
        {
            //Service
            ViewBag.qryChartNo = _serviceURL.qryChartNo;
            ViewBag.qrySex = _serviceURL.qrySex;
            ViewBag.qryBirth = _serviceURL.qryBirth;
            ViewBag.qryVisitDate = _serviceURL.qryVisitDate;

            //SelectList
            String[] sexSelect = this._selectListModel.sexSelect;
            ViewData["sexSelect"] = sexSelect;
            //foreach (var i in sexSelect)
            //{
            //    Console.WriteLine(i);

            //}
            return View();
        }

        //Patients - editDataHandler - 更新內容
        [RouteAttribute("/Records/update/rawdata")]  //設定路徑
        [HttpPutAttribute] //Http put請求
        [ConsumesAttribute("application/json")] //設定格式為json
        public IActionResult recordsUpdateService([FromBodyAttribute] ChartInfo chartInfo) //接收ChartInfo物件
        {
            //Part1 修改資料並建立訊息
            _context.PersonalInformation.Add(chartInfo); //將修改的物件加入MSSQL資料表          
            _context.Entry(chartInfo).State = EntityState.Modified; //設定狀態為修改狀態
            Message msg = new Message();

            //Part2 儲存資料並回傳訊息
            try
            {
                Int32 row = _context.SaveChanges(); // 儲存變更，並取得變更筆數
                if (row > 0)
                {
                    //有可更新資料
                    HttpResponse httpResponse = this.Response;
                    httpResponse.StatusCode = 200;
                    msg.code = 200;
                    msg.msg = "訊息:";
                    msg.msg2 = $"病歷號 [{chartInfo.chartNo}] 更新成功。";
                }
                else
                {
                    //無可更新資料
                    HttpResponse httpResponse = this.Response;
                    httpResponse.StatusCode = 400;
                    msg.code = 400;
                    msg.msg = "訊息:";
                    msg.msg2 = $"病歷號 [{chartInfo.chartNo}] 無可更新資料。";
                }
            }
            catch (DbUpdateException ex) //例外處理
            {
                msg.code = 500;
                msg.msg = "訊息:";
                msg.msg2 = $"病歷號 [{chartInfo.chartNo}] 更新資料出現嚴重錯誤。";
            }
            return this.Json(msg); //回傳msg
        }

        //Patients - delDataHandler - 刪除內容
        [RouteAttribute("/Records/delete/{cno}/rawdata")] //設定路徑
        [HttpDeleteAttribute] //HttpDelete請求
        [ProducesAttribute("application/json")] //設定格式為json
        public IActionResult recordsDeleteService([FromRouteAttribute(Name = "cno")] String chartNo)
        {
            //Part1 查詢待刪除資料並建立訊息
            var result = (from i in _context.PersonalInformation where i.chartNo == chartNo select i).FirstOrDefault();
            Message msg = new Message();

            //Part2 判斷待刪除資料是否存在，並進行後續處理
            if (result != null) //若待刪除資料存在
            {
                this._context.Entry(result).State = EntityState.Deleted; //將狀態改為刪除
                try //成功執行，儲存變更並產生訊息
                {                   
                    Int32 row = this._context.SaveChanges();
                    HttpResponse httpResponse = this.Response;
                    httpResponse.StatusCode = 200;
                    msg.code = 200;
                    msg.msg = "訊息";
                    msg.msg2 = $"病歷號 [{chartNo}] 刪除成功";
                }
                catch(DbUpdateException ex) //例外處理
                {                   
                    HttpResponse httpResponse = this.Response;
                    httpResponse.StatusCode = 400;
                    msg.code = 400;
                    msg.msg = "訊息";
                    msg.msg2 = $"病歷號 [{chartNo}] 刪除失敗";
                }
            }
            else //若待刪除資料不存在
            {               
                HttpResponse httpResponse = this.Response;
                httpResponse.StatusCode = 400;
                msg.code = 400;
                msg.msg = "訊息";
                msg.msg2 = $"查無病歷號 [{chartNo}] ";
            }
            return this.Json(msg); //回傳msg
        }
    }
}
