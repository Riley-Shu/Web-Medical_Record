# Part01 建立Service並連接資料庫
- API控制器Postman測試是否能從資料庫取得資料
## QryBirthController.cs (Service)
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
    public class QryBirthController : ControllerBase
    {
        //Data
        private Sample05DbContext _context;
        //建構子注入
        public QryBirthController(Sample05DbContext context)
        {
            //DbContext
            _context = context;
        }

        //Http Get取得資料，查詢對象: chartNo
        [HttpGetAttribute]
        [RouteAttribute("qry/{birthday}/rawdata")]
        [Produces("application/json")] //指定回傳JSON格式
        [ProducesResponseType(typeof(List<ChartInfo>), 200)] //指定正確回傳
        [ProducesResponseType(typeof(Message), 400)] //指定錯誤回傳
        [DisableCors] //關閉Cors
        [EnableCors("myweb")] //開放Cors
        public IActionResult birthQry([FromRouteAttribute(Name = "birthday")] DateTime? birthday)
        {
            //Console.WriteLine(_context);
            //Part1: LINQ查詢
            var birthResult = (from i in _context.PersonalInformation where i.birthday == birthday select i).ToList();
            //Part2: 判斷並回傳結果
            if (birthResult.Count > 0)
            {
                return this.Ok(birthResult);
            }
            else
            {
                Message msg = new Message()
                {
                    code = 400,
                    msg = $"錯誤訊息: 查無此病歷號 [{birthday}]，請確認後重新查詢。"
                };
                return this.StatusCode(400, msg);
            }
        }
    }
}
```
## QryVisitDateController.cs (Service)
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
        [RouteAttribute("qry/{VisitDate}/rawdata")]
        [Produces("application/json")] //指定回傳JSON格式
        [ProducesResponseType(typeof(List<ChartInfo>), 200)] //指定正確回傳
        [ProducesResponseType(typeof(Message), 400)] //指定錯誤回傳
        [DisableCors] //關閉Cors
        [EnableCors("myweb")] //開放Cors
        public IActionResult birthQry([FromRouteAttribute(Name = "VisitDate")] DateTime? VisitDate)
        {
            //Console.WriteLine(_context);
            //Part1: LINQ查詢
            var visitDateResult = (from i in _context.PersonalInformation where i.VisitDate == VisitDate select i).ToList();
            //Part2: 判斷並回傳結果
            if (visitDateResult.Count > 0)
            {
                return this.Ok(visitDateResult);
            }
            else
            {
                Message msg = new Message()
                {
                    code = 400,
                    msg = $"錯誤訊息: 查無此病歷號 [{VisitDate}]，請確認後重新查詢。"


                };
                return this.StatusCode(400, msg);
            }
        }
    }

}
```
# Step2 Service連線到Web
- appsetting.json加入ServiceURL路徑
- 建立ServiceURL字串
- ## appsetting.json (Web)
```json
"ServiceURL": {
	"qryChartNo": "https://localhost:7138/api/QryChartNo/qry/{0}/rawdata",
	"qrySex": "https://localhost:7138/api/QrySex/qry/{0}/rawdata",
	"qryBirth": "https://localhost:7138/api/QryBirth/qry/{0}/rawdata",
	"qryVisitDate": "https://localhost:7138/api/QryVisitDate/qry/{0}/rawdata"
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
       public String qryBirth { get; set; }
	   public String qryVisitDate { get; set; }
    }
}
```
## RecordsController.cs (Web)
```cs
ViewBag.qryBirth = _serviceURL.qryBirth;
ViewBag.qryVisitDate = _serviceURL.qryVisitDate;
```

# Step3 編輯前端基本功能
- MVC控制器+ View ViewData 綁定URL字串
- 前端基本內容
- 掛載並測試Vue, axios，取得Service資料並使用Vue渲染呈現
## Patients.cshtml 
- ViewBag + 加入全域變數
```cs
@{
    //String qryChartNo = ViewData["qryChartNo"] as String;
    String qryChartNo = ViewBag.qryChartNo as String;
    String qrySex = ViewBag.qrySex as String;
    String[] selectList = ViewData["sexSelect"] as String[];
    String qryBirth = ViewBag.qryBirth as String;
    String qryVisitDate = ViewBag.qryVisitDate as String;
}
<script>
    //全域變數
    var qryChartNo = '@Html.Raw(qryChartNo)';
    var qrySex = '@Html.Raw(qrySex)';
    var qryBirth = '@Html.Raw(qryBirth)';
    var qryVisitDate = '@Html.Raw(qryVisitDate)'

    // foreach (String i in selectList)
    // {
    //    Console.WriteLine(i);
    // };
</script>
```
- 增加欄位:
- 增加資料 & 方法
```cs
<tbody>
    <tr>
        <td style="padding:3px">根據</td>
        <td>生日</td>
        <td style="padding:6px"></td>
        <td> <input type="text" v-model:value="birthday" style="width:185px;height:27px" /></td>
        <td>
             <button v-on:click="qrybirthdayHandler">查詢</button>
        </td>
    </tr>
</tbody>
<tbody>
    <tr>
        <td style="padding:3px">根據</td>
        <td>看診日期</td>
        <td style="padding:6px"></td>
        <td> <input type="text" v-model:value="visitDate" style="width:185px;height:27px" /></td>
        <td>
            <button v-on:click="qryVisitDateHandler">查詢</button>
        </td>
    </tr>
</tbody>
```

```cs
 var data = {
    chartNo: "",
    chartAPI: qryChartNo,
    result: [],
    showQry:false,
    msg:'',
    showMsg:false,
    sex:'male',
    sexAPI:qrySex,
    birthday:'',
    birthAPI: qryBirth,
    visitDate:'',
    visitAPI: qryVisitDate
}
```

```cs
//方法: 根據生日查詢
qrybirthdayHandler: function () {
    //Part1: 刷新頁面
    app.showQry = false;
    app.showMsg = false;

    //Part2: 取得ServiceURL，並使用axios取得資料
    let reUrl = app.birthAPI.replace('{0}', this.birthday);
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
},
//方法: 根據看診日查詢
qryVisitDateHandler: function () {
    //Part1: 刷新頁面
    app.showQry = false;
    app.showMsg = false;

    //Part2: 取得ServiceURL，並使用axios取得資料
    let reUrl = app.visitAPI.replace('{0}', this.visitDate);
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
![image](https://github.com/Riley-Shu/Web-Medical_Record/blob/master/Note/image/N07-P01.png)

# Step04 使用jQery UI Datepicker提供日期選擇器
- [Download Builder | jQuery UI](https://jqueryui.com/download/)

> **jQuery UI** 是一個基於 **jQuery JavaScript 函式庫**的小部件和交互庫，提供抽象化、可自訂主題的 GUI 控制項與動畫效果。它可以用來建構互動式的網際網路應用程式。jQuery UI 的控制項包括手風琴式選單、自動完成、按鈕、日期選擇器、對話框、選單、進度條、滑動條、微調選擇器、頁籤、工具提示等。此外，它還提供了各種效果，如顯示、下拉、爆炸、淡入等等。
> 
> - **互動**：拖曳（Draggable）、拖放（Droppable）、調整大小（Resizable）、選取（Selectable）、排序（Sortable）等。
> - **控制項**：手風琴式選單（Accordion）、自動完成（Autocomplete）、按鈕（Button）、日期選擇器（Datepicker）、對話框（Dialog）、選單（Menu）、進度條（Progressbar）、滑動條（Slider）、微調選擇器（Spinner）、頁籤（Tabs）、工具提示（Tooltip）等。
> - **效果**：顏色動畫（Color Animation）、切換 Class、新增 Class、移除 Class、開關 Class、效果（Effects）、切換（Toggle）、隱藏、顯示等。
> - **工具**：位置（Position）等。
> 
> 來源：https://zh.wikipedia.org/zh-tw/JQuery_UI
## Layout.cshtml
```html
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.js"></script>
    <script src="~/js/jquery-ui.min.js"></script>
```
## Patients.cshtml
#JavaScript/jQuery/控制項 

```cs
<script>
    //全域變數
    var qryChartNo = '@Html.Raw(qryChartNo)';
    var qrySex = '@Html.Raw(qrySex)';
    var qryBirth = '@Html.Raw(qryBirth)';
    var qryVisitDate = '@Html.Raw(qryVisitDate)'

    //jQuery
    $(document).ready(
        function () {
            $('#txtBirthday').datepicker(
                {
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: "yy-mm-dd",
                    yearRange: "1985:2030",
                    onSelect: function (e) {
                        app.birthday = e;
                    }
                }
            );
            $('#txtVisitDate').datepicker(
                {
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: "yy-mm-dd",
                    yearRange: "1985:2030",
                    onSelect: function (e) {
                        app.visitDate = e;
                    }
                }
            );
        }
    );
</script>
```

```cs
<tbody>
	<tr>
		<td style="padding:3px">根據</td>
		<td style="text-align:center">生日</td>
		<td style="padding:6px"></td>                       
		<td><input type="text" v-model:value="birthday" style="width:185px;height:27px" id="txtBirthday"/></td>
		<td><button v-on:click="qrybirthdayHandler">查詢</button></td>
	</tr>
</tbody>
<tbody>
	<tr>
		<td style="padding:3px">根據</td>
		<td style="text-align:center">看診日期</td>
		<td style="padding:6px"></td>
		<td> <input type="text" v-model:value="visitDate" style="width:185px;height:27px" id="txtVisitDate" /></td>
		<td><button v-on:click="qryVisitDateHandler">查詢</button></td>
	</tr>
</tbody>
```
