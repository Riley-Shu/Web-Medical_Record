# Part01 建立Service並連接資料庫
## QrySexController.cs (Service)
```cs

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
                    msg = $"錯誤訊息: 查無此病歷號 [{sex}]，請確認後重新查詢。"
                };
                return this.StatusCode(400, msg);
            }
        }
    }
}

```

# Step2 Service連線到Web
- appsetting.json加入ServiceURL路徑
- 建立ServiceURL類別
- ~~Program.cs註冊 ServiceURL服務~~
- MVC控制器注入ServiceURL建構子
## appsetting.json (Web)
```json
    "ServiceURL": {
        "qryChartNo": "https://localhost:7138/api/QryChartNo/qry/{0}/rawdata",
        "qrySex": "https://localhost:7138/api/QrySex/qry/{0}/rawdata"
    }
```
## ServiceURL (Web)
```cs
namespace Sample05_Web.Models
{
    public class ServiceURL
    {
       public String qryChartNo { get; set; }
       public String qrySex { get; set; }
    }
}
```
## RecordsController.cs (Web)
```cs
        public IActionResult Patients()
        {
            //參考自DbContext
            //Console.WriteLine(_context);
            //String qryChartNo = this._serviceURL.chartQry;
            //Console.WriteLine(qryChartNo);
            //ViewData["qryChartNo"] = qryChartNo;
            ViewBag.qryChartNo = _serviceURL.qryChartNo;
            ViewBag.qrySex = _serviceURL.qrySex;
            return View();
        }
```

# Step4 編輯前端基本功能 
- MVC控制器+ View ViewData 綁定URL字串
- 前端基本內容
- 掛載並測試Vue, axios，取得Service資料並使用Vue渲染呈現
## Patients
- 加入全域變數
```cs
@{
    //String qryChartNo = ViewData["qryChartNo"] as String;
    String qryChartNo = ViewBag.qryChartNo as String;
    String qrySex = ViewBag.qrySex as String;
}
<script>
    var qryChartNo = '@Html.Raw(qryChartNo)';
    var qrySex = '@Html.Raw(qrySex)';
</script>
```
- fieldset: 新增查詢性別的欄位，用table對齊
```html
<fieldset> @* 查詢欄位 *@
<div>
    <table> @* 說明: 藉由table來將欄位對齊 *@
        <tbody>
            <tr>
                    <td>根據 病歷號</td>
                    <td> <input type="text" v-model:value="chartNo" /></td>
                    <td>
                        <button v-on:click="qryChartNoHandler">查詢</button>
                    </td>
            </tr>
        </tbody>
            <tbody>
                <tr>
                    <td>根據 性 別</td>
                    <td> <input type="text" v-model:value="sex" /></td>
                    <td>
                        <button v-on:click="qrySexHandler">查詢</button>
                    </td>
                </tr>
            </tbody>
    </table>
</div>
```
- 新增查詢性別的方法: 基本上與查詢病歷號相似，更改變數為sex就好
```cs
 //方法: 根據性別查詢
 qrySexHandler:function(){
     //Part1: 刷新頁面
     app.showQry = false;
     app.showMsg = false;

     //Part2: 取得ServiceURL，並使用axios取得資料
     let reUrl = app.sexAPI.replace('{0}', this.sex);
     console.log(reUrl);
     axios.get(reUrl)
         .then(
             (r) => {
                 //正確
                 console.log("then");
                 console.log(r);
                 app.result = r.data;
                 app.showQry = true;
             }
         )
         .catch(
             (r) => {
                 //錯誤
                 console.log("catch");
                 console.log(r);
                 app.msg = r.response.data.msg;
                 app.showMsg = true;
             }
         )

 }
```

![[N06-P01.png]]

# Step05 input改為SelectList
1. 建立Model類別
2. 註冊為服務
3. 控制器注入建構子，並加入ViewData
4. View加入ViewData
## SelectListModel (Web)
```cs
namespace Sample05_Web.Models
{
    public class SelectListModel
    {
        public String[] sexSelect = new String[]
        {
            "male",
            "female"
        };
    }
}

```
## Program.cs (Web)
```cs
//------------註冊服務: SelectListModel------------
SelectListModel selectListModel = new SelectListModel();
builder.Services.AddSingleton(selectListModel);
//------------------------
```
## RecordsController.cs (Web)
- 加入data，建構子注入
- ViewData
```cs
using Microsoft.AspNetCore.Mvc;
using Sample05_Web.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;



namespace Sample05_Web.Controllers
{
    public class RecordsController : Controller
    {
        //private Sample05DbContext _context;
        private ServiceURL _serviceURL;
        private SelectListModel _selectListModel;
        //public RecordsController(Sample05DbContext context,ServiceURL serviceURL)
        public RecordsController( ServiceURL serviceURL, SelectListModel selectListModel)

        {
            //_context = context;
            _serviceURL = serviceURL;
            _selectListModel = selectListModel;
        }

        public IActionResult Patients()
        {
            //參考自DbContext
            //Console.WriteLine(_context);
            //String qryChartNo = this._serviceURL.chartQry;
            //Console.WriteLine(qryChartNo);
            //ViewData["qryChartNo"] = qryChartNo;

            //Service
            ViewBag.qryChartNo = _serviceURL.qryChartNo;
            ViewBag.qrySex = _serviceURL.qrySex;

            //SelectList
            String[] sexSelect = this._selectListModel.sexSelect;
            ViewData["sexSelect"] = sexSelect;
            foreach (var i in sexSelect)
            {
                Console.WriteLine(i);
            }
            return View();
        }
    }
}
```
## Patients.cshtml 
- ViewData
- Select
- 注意: 不要打錯變數的名稱
```cs
@{
    //String qryChartNo = ViewData["qryChartNo"] as String;
    String qryChartNo = ViewBag.qryChartNo as String;
    String qrySex = ViewBag.qrySex as String;
    String[] selectList = ViewData["sexSelect"] as String[];
}
<script>
    //全域變數
    var qryChartNo = '@Html.Raw(qryChartNo)';
    var qrySex = '@Html.Raw(qrySex)';

    // foreach (String i in selectList)
    // {
    //    Console.WriteLine(i);
    // };
</script>
```

```html
<select v-model:value="sex" style="width:185px;height:27px">
	@foreach (var item in selectList)
	{
		<option value="@item" selected> @item </option>
	}
</select>
```

![[N06-P02.png]]