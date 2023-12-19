TAG: #JavaScript/jQuery/控制項  #JavaScript/jQuery/方法
# Part01 建立基本架構
## Patients.cshtml 
- 按鈕和方法
```html
<td style="text-align:center"> 
	<button class=" btn btn-success" v-on:click="editDataHandler">編輯</button>
	<button class=" btn btn-danger" v-on:click="delDataHandler">刪除</button>
</td>
```
- 做強佔式視窗
```html
<fieldset id="editDialog" style="display:None"> @*編輯資料強佔式對話盒 (jQuery Widget: Dialog)*@
    <table>
        <tr>
            <td>病歷號</td>
            <td> <input class="text" readonly/></td>
        </tr>
        <tr>
            <td>姓名</td>
            <td> <input class="text" /></td>
        </tr>
        <tr>
            <td>性別</td>
            <td> <input class="text" /></td>
        </tr>
        <tr>
            <td>生日</td>
            <td> <input class="text" /></td>
        </tr>
        <tr>
            <td>看診日期</td>
            <td> <input class="text" /></td>
        </tr>
    </table>
</fieldset>
```
- 方法: 編輯資料
```cs
//方法: 編輯資料
editDataHandler: function (e) {
	console.log('editDataHandler');
	$('#editDialog').dialog(
		{
			title:'編輯病患資料',
			modal:true
		})
},
```

# Part02 取得選定資料
## Patients.cshtml 
### Fieldset
- 給result index作為 accesskey，用以連接表格和強佔式對話盒
```html
<tbody>
	<tr v-for="item,index in result"> @* 說明: index是連接表格與對話盒的accesskey *@
		<td style="text-align:center" ><button>詳細</button> </td> @* 觸發方法檢視詳細資料 *@
		<td style="text-align:center">{{item.chartNo}}</td>
		<td style="text-align:center">{{item.name}}</td>
		<td style="text-align:center">{{item.sex}}</td>
		<td style="text-align:center">{{item.birthday| dateFormatter}}</td>
		<td style="text-align:center">{{item.visitDate| dateFormatter}}</td>
		<td style="text-align:center"> 
			<button class=" btn btn-success" v-on:click="editDataHandler" v-bind:accesskey="index">編輯</button> @* 觸發方法編輯資料 *@
			<button class=" btn btn-danger" v-on:click="delDataHandler" v-bind:accesskey="index">刪除</button> @* 觸發方法刪除資料 *@
		</td>
    </tr>
</tbody>
``` 
### Data
```cs
var data = {
    chartNo: "",
    chartAPI: qryChartNo,
    result: [],
    showQry:false,
    msg:'',
    msg2:'',
    showMsg:false,
    sex:'male',
    sexAPI:qrySex,
    birthday: '1985-05-01',
    birthAPI: qryBirth,
    visitDate: '1999-01-05',
    visitAPI: qryVisitDate,
    
    curPatient: {},
}
```
### Methods
- 依靠index，連結選擇的result資料，並呈現在強佔式窗格
- 建立editDialog
- 注意：
	- 不要忘記把變數加上app或是this，會原地因為沒有該值的定義而停擺
	- buttons 不要忘記s
```cs
//方法: 編輯資料
editDataHandler: function (e) {
    console.log('editDataHandler');
    console.log(e);
    let accessKey = e.target.accessKey; //說明:index是連接表格與對話盒的accesskey
    console.log(accessKey);
    this.curPatient = this.result[accessKey];
    console.log(this.curPatient);
    $('#editDialog').dialog(
        {
            title:'編輯病患資料',
            modal:true,
            buttons:[
                {
                    text:'取消',
                    class:'btn btn-danger'
                },
                {
                    text: '更新',
                    class: 'btn btn-primary'
                }
            ]
        })
},
//方法: 刪除資料
delDataHandler:function(){
    console.log('delDataHandler');
}
```

# Part3 編輯對話盒的取消功能
## Patients.cshtml 
### Data
```cs
    var data = {
	    (略)
        curPatient: {},
        sourcePatient: {},
        index:1
    }
```
### Methods
- 使用`Object.assign()`方法備份對象
- 編輯 `取消` 按鈕
> `Object.assign()`是JavaScript中的一種方法，用於將一個或多個源對象的所有可枚舉自有屬性複製到目標對象中。它返回修改後的目標對象。在這種情況下，`Object.assign()`方法用於將`curCustomers`對象的所有可枚舉自有屬性複製到一個新的空對象中，並將其賦值給`sourceCustomers`。這樣做的原因是為了避免對`curCustomers`對象進行任何更改，因為`sourceCustomers`對象將被用作`curCustomers`對象的副本。

```cs
//Part2 備份選擇之病患的資料
app.index = accessKey;
this.sourcePatient = Object.assign({}, this.curPatient);
console.log("this.sourcePatient");
console.log(this.index);
console.log(this.sourcePatient);
```

```cs
//Part3 打開對話盒
$('#editDialog').dialog(
    {
        title:'編輯病患資料',
        modal:true, //強佔式視窗
        buttons:[
            {
                //取消編輯
                text:'取消',
                class:'btn btn-danger',
                click: function () {
                    //還原原始資料
                    console.log("取消更新");
                    app.result[app.index] = app.sourcePatient;
                    console.log("還原為原始資料")
                    console.log(app.sourcePatient);
                    //關閉對話盒
                    $('#editDialog').dialog('close');
                }
            },
			{
            //更新內容
            text: '更新',
            class: 'btn btn-primary',
            click: function () { 
                //
            }
        }
    ]
})
```
![[N08-P03.png]]


# Part4  編輯對話盒的更新功能
- 使用`jQuery.ajax()`發送HTTP請求
- 注意： `$.ajax({}) `忘記`{}`的話，Vue會掛掉
#JavaScript/jQuery/方法 
> `jQuery.ajax()`是jQuery的核心方法之一，用於發送HTTP請求。它可以用於發送各種類型的請求，例如GET、POST、PUT和DELETE。以下是一個簡單的例子，展示了如何使用`jQuery.ajax()`方法發送一個GET請求：
## Patients.cshtml 
```js
//更新內容
text: '更新',
class: 'btn btn-primary',
click: function () { 
	//說明: jQuery.ajax()是jQuery的核心方法之一，用於發送HTTP請求
	$.ajax(
	{
		url: 'update/rawdata',
		type: 'PUT',
		data: JSON.stringify(app.curPatient), //說明: JSON.stringify()是JavaScript中的一種方法，用於將JavaScript對象轉換為JSON字符串。
		success: function (e) { 
			console.log(e);
			app.sourcePatient = Object.assign({}, app.curPatient);
		},
		error: function (e) {
			console.log(e);
		}
	});
}
```
## Web專案連接資料庫
#我有問題 `jQuery.ajax()` PUT路徑404
![[N08-P04.png]]
- 不能把put功能給api，沒有json功能
![[N08-P05.png]]
#我有答案 要確認有確實連接上資料庫，否則會出現空物件等錯誤

### 安裝資料庫套件
- Microsoft.EntityFrameworkCore: 
	- 它提供了一個 Object-Relational Mapping (ORM) 的框架，讓開發人員可以使用物件導向的方式來操作資料庫。
- Microsoft.EntityFrameworkCore.SqlServer: 
	- 提供了一些針對 SQL Server 資料庫的特定功能
- Microsoft.Data.SqlClient:  
	- 提供了一個低階的 API，讓開發人員可以直接使用 SQL 語句來操作資料庫。
### 連接資料庫
1. 安裝套件
2. 加入連接
3. 建立連接字串
4. 利用 DbContextOptions 類別，配置 DbContext 操作資料庫。

![[Portfolios/Sample05_Paitent_Web/@ATT/N02-P01.png]]
#### appsetting.json (Service)
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
#### ChartInfo.cs (Web)
- 建立Model，提供對應資料表
```cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample05_Web.Models
{
    //參考自MSSQL Table
    [Table("PersonalInformation")]
    public class Chart
    {
        [Column(name: "ChartNo")]
        [Required]
        [Key]
        public String ChartNo { get; set; }

        [Column(name: "Name")]
        [Required]
        public String Name { get; set; }

        [Column(name: "Sex")]
        public String Sex { get; set; }

        [Column(name: "Birthday")]
        public DateTime? Birthday { get; set; }

        [Column(name: "VisitDate")]
        [Required]
        public DateTime? VisitDate { get;}
    }
}
```
#### Sample05DbContext.cs (Web)
- 建立Model，繼承DbContext，控制資料庫功能
```cs
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
        //對應資料表
        public DbSet<Chart> PersonalInformation { get;set;}
    }
}
```
### 註冊服務
#### Program.cs (Web)
-  `Microsoft.EntityFrameworkCore.SqlServer.UseSqlServer`
```cs
//------------註冊服務: DbContex------------
builder.Services.AddDbContext<Sample05_Web.Models.Sample05DbContext>(
    (options) =>
    {  options.UseSqlServer(builder.Configuration.GetConnectionString("ConnDbSample05"));
    }
);
//------------------------------------------
```
### 控制器連接資料庫
#### RecordsController.cs (Web)
- 確認後端有連接上資料庫
```cs
using Microsoft.AspNetCore.Mvc;
using Sample05_Web.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;


namespace Sample05_Web.Controllers
{
    public class RecordsController : Controller
    {
        private Sample05DbContext _context;
        public RecordsController(Sample05DbContext context) 
        {
            _context = context;
        }

        public IActionResult Patient()
        {
            //參考自DbContext
            String chartNo = "36165";
            Console.WriteLine(_context);
            var charts = (from i in _context.PersonalInformation where i.ChartNo == chartNo select i).ToList();
            Console.WriteLine("1");
            if (charts != null){

                foreach (var item in charts)
                {
                    Console.WriteLine("2");
                    Console.WriteLine(item);
                }
            }
            return View();
        }
    }
}
```

![[Pasted image 20231129214052.png]]

## Web控制器HTTP請求
### RecordsController.cs (Web)
- Http Put
- 儲存資料變更
- 回傳訊息
```cs
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
			msg.msg2 = $"病歷號 {chartInfo.chartNo}更新成功";
		}
		else
		{
			//無可更新資料
			HttpResponse httpResponse = this.Response;
			httpResponse.StatusCode = 400;
			msg.code = 400;
			msg.msg = "訊息:";
			msg.msg2 = $"病歷號 {chartInfo.chartNo} 無可更新資料";
		}
	}
	catch (DbUpdateException ex) //例外處理
	{
		msg.code = 500;
		msg.msg = "訊息:";
		msg.msg2 = $"病歷號 {chartInfo.chartNo} 更新資料出現嚴重錯誤";
	}
	return this.Json(msg); //回傳msg
}
```

#### #我問問題 FrombodyAttribute vs FromRouteAttribute

`FromBodyAttribute` 和 `FromRouteAttribute` 是 ASP.NET Core Web API 中常用的兩種屬性。它們都是用來綁定 HTTP 請求中的參數，但是它們的使用方式有所不同

- `FromBodyAttribute` 
	- 用於綁定 HTTP 請求的主體部分。指定的參數是從請求主體中而不是從請求 URL/URI 中傳遞的。
	- 不能在 `HttpGet` 請求中使用此屬性，只能在 `PUT`、`POST` 和 `Delete` 請求中使用。
	- 在 Web API 中，每個動作方法只能使用一個 `FromBodyAttribute` 標記。
- `FromRouteAttribute` 
	- 用於綁定當前請求的路由數據。它會查看路由參數並根據該參數提取/綁定數據。
	- 由於路由在外部調用時通常基於 URL，因此它與 `FromUri` 在以前的 Web API 版本中是可比的
    

## 完成檢視
### Patients.cshtml 
- ajax方法發送HTTP請求，與控制器連接
- 另外設計訊息對話盒
- 順便改調其他的訊息呈現方式，將所有`app.showMsg = true;`改為對話盒
```cs
//更新內容
text: '更新',
class: 'btn btn-primary',
click: function () 
{
	let service = 'https://localhost:7241/Records/update/rawdata';
	console.log(service);                 
	$.ajax( //說明: jQuery.ajax()是jQuery的核心方法之一，用於發送HTTP請求
		{
			url: service, // 設定服務位址
			type: 'PUT', // 設定 HTTP 方法
			data: JSON.stringify(app.curPatient), //說明: JSON.stringify()是JavaScript中的一種方法，用於將JavaScript對象轉換為JSON字符串。
			beforeSend: function (xhr) { 
				xhr.setRequestHeader("Content-Type", "application/json") // 設定 HTTP Request Header，使其能正確傳送
			},
			//成功格式
			success: function (e) { 
				console.log(e);
				app.msg2 = e.msg2;
				app.sourcePatient = Object.assign({}, app.curPatient); // 複製 curPatient 物件
				// app.showMsg = true;
			},
			//錯誤格式
			error: function (e) {
				console.log(e);
				app.msg2 = e.msg2;
				// app.showMsg = true;
			}
		}
	);
	$('#editDialog').dialog('close'); //關閉對話盒
	$("#msgDialog").dialog( //訊息對話盒
	{
		title: '更新訊息',
		modal: true,
		buttons: 
		[{
			text:'關閉',
			class: 'btn btn-primary',

			click: function () 
			{
				$(this).dialog("close");
			}
		}]
	}
	);
}
```
