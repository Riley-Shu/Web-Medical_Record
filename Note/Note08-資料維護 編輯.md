# 修改資料
## Part1 建立基本架構
### Patients.cshtml 
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

## Part2 抓資料
### 強佔式視窗
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

```cs
        //方法: 編輯資料
        editDataHandler: function (e) {
            console.log('editDataHandler');
            console.log(e)
            let index = e.target.accessKey; //說明:index是連接表格與對話盒的accesskey
            console.log(index);
```

### 我有問題 (解決)
抓資料卡關中，result到了後面的方法就已經消失了
我綁定了很多種搜尋方式，沒辦法在硬創回去
```cs
       //方法: 編輯資料
       editDataHandler: function (e) {
           console.log('editDataHandler');
           console.log(e)
           let index = e.target.accessKey; //說明:index是連接表格與對話盒的accesskey
           console.log(index);

           this.curPatient = this.result[index];
           app.indexer = index;
           console.log(result) //問題所在
           this.sourcePatient = Object.assign({}, this.curPatient);
           console.log(curPatient)
```
![[N08-P01.png]]

#### AI: XMLHttpRequest vs axios 兩者分別是甚麼? 該如何使用? 甚麼場合分別適合兩者?
- 比較先前作品，發現有兩種HTTP請求寫法，推判可能是這部分造成
```cs
XMLHttpRequest和axios都是用於發送HTTP請求的JavaScript庫。XMLHttpRequest是一個內置的JavaScript對象，用於在Web瀏覽器中發送HTTP請求。axios是一個基於Promise的HTTP客戶端，可以在瀏覽器和Node.js中使用。以下是兩者之間的一些區別：

- XMLHttpRequest需要手動處理請求和響應，而axios使用Promise自動處理請求和響應。
- axios支持Promise，可以更容易地處理錯誤和異常情況。
- axios可以在瀏覽器和Node.js中使用，而XMLHttpRequest只能在瀏覽器中使用。
- axios可以自動轉換JSON數據，而XMLHttpRequest需要手動轉換JSON數據。

以下是使用XMLHttpRequest和axios的一些示例：

使用XMLHttpRequest：

```javascript
var xhr = new XMLHttpRequest();
xhr.open('GET', '/api/data', true);
xhr.onload = function () {
  if (xhr.status === 200) {
    console.log(xhr.responseText);
  } else {
    console.log('Request failed.  Returned status of ' + xhr.status);
  }
};
xhr.send();
```

使用axios：

```javascript
axios.get('/api/data')
  .then(function (response) {
    console.log(response.data);
  })
  .catch(function (error) {
    console.log(error);
  });
```

[在一般情況下，axios比XMLHttpRequest更容易使用，因為它使用Promise自動處理請求和響應。如果您需要在瀏覽器中發送HTTP請求，則可以使用XMLHttpRequest。如果您需要在瀏覽器和Node.js中發送HTTP請求，則可以使用axios。如果您想了解更多有關XMLHttpRequest和axios的信息，請查看上面的參考資料。](https://juejin.cn/post/6997063769541230628)[1](https://juejin.cn/post/6997063769541230628)[2](https://blog.csdn.net/xiao_yu_liu/article/details/100666516)[3](https://mini-ghost.dev/posts/axios-source-code-2/)

2023/12/03 01:29 

- 結論 無關
- 注意: 不要忘記把變數加上app或是this，會原地因為沒有該值的定義而停擺
- 注意: buttons 不要忘記s
![[N08-P02.png]]

### 強佔式視窗欄位
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
### 方法
- 依靠index，連結選擇的result資料，並呈現在強佔式窗格
```cs
//方法: 編輯資料
editDataHandler: function (e) {
    console.log('editDataHandler');
    console.log(e);
    let accessKey = e.target.accessKey; //說明:index是連接表格與對話盒的accesskey
    console.log(accessKey);
    this.curPatient = this.result[accessKey];
    console.log(this.curPatient);
    // app.indexer = index;
    // this.sourcePatient = Object.assign({}, this.curPatient);

    // var formattedDate = moment(this.curPatient.birthday).format('YYYY-MM-DD');

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
## Part3 編輯對話盒的取消功能
- 2023/12/04 01:13 開始
### 資料
```cs
    var data = {
	    (略)
        curPatient: {},
        sourcePatient: {},
        index:1
    }
```
### 方法
`Object.assign()`是JavaScript中的一種方法，用於將一個或多個源對象的所有可枚舉自有屬性複製到目標對象中。它返回修改後的目標對象。在這種情況下，`Object.assign()`方法用於將`curCustomers`對象的所有可枚舉自有屬性複製到一個新的空對象中，並將其賦值給`sourceCustomers`。這樣做的原因是為了避免對`curCustomers`對象進行任何更改，因為`sourceCustomers`對象將被用作`curCustomers`對象的副本。希望這有幫助！

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

## Part4 編輯對話盒的更新功能
### Patients.cshtml 
#### jQuery的核心ajax方法
`jQuery.ajax()`是jQuery的核心方法之一，用於發送HTTP請求。它可以用於發送各種類型的請求，例如GET、POST、PUT和DELETE。以下是一個簡單的例子，展示了如何使用`jQuery.ajax()`方法發送一個GET請求：

```javascript
$.ajax({
  url: '/example/api',
  type: 'GET',
  success: function(data) {
    console.log(data);
  },
  error: function(jqXHR, textStatus, errorThrown) {
    console.log(textStatus, errorThrown);
  }
});
```

在這個例子中，我們使用`jQuery.ajax()`方法發送一個GET請求到`/example/api` URL。當請求成功時，`success`回調函數將被調用，並且服務器返回的數據將被打印到控制台中。如果請求失敗，`error`回調函數將被調用，並且錯誤信息將被打印到控制台中。希望這有幫助！

- 結束: 2023/12/04 02:27
- 扛不下去了睡去
- 還沒做完編輯對話盒
- 留言: 
	- 
```cs
//>> 為什麼需要這句
beforeSend: function (xhr) {
    xhr.setRequestHeader("Content-Type", "application/json")
},
```

- 開始: 2023/12/04 22:25
- 找到掛Vue掛掉的原因是 $.ajax(	{}) 沒有{}
```cs
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
### 解決 我有問題: Web也放一組連結資料庫所需的 
- 注意: 要確認有確實連接上資料庫，否則會出現空物件等錯誤
![[N08-P04.png]]
##### ChartInfo.cs (Web)
- Service複製，修改namespase
##### Message.cs (Web)
- Service複製，修改namespase
##### Sample05DbContex.cs (Web)
- Service複製，修改namespase
##### Program.cs (Web)
```cs
//------------註冊服務: DbContext------------
builder.Services.AddDbContext<Sample05_Web.Models.Sample05DbContext>(
    (options) =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnDbSample05"));
    }
);
//----------------------------
```
##### RecordsController.cs (Web)
```cs
    public class RecordsController : Controller
    {
        private Sample05DbContext _context;
        private ServiceURL _serviceURL;
        private SelectListModel _selectListModel;
        public RecordsController(Sample05DbContext context, ServiceURL serviceURL, SelectListModel selectListModel)
        //public RecordsController( ServiceURL serviceURL, SelectListModel selectListModel)

        {
            _context = context;
            _serviceURL = serviceURL;
            _selectListModel = selectListModel;

        }
```

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

#### AI: FrombodyAttribute vs FromRouteAttribute
差別在哪裡? 分別該如何使用?

[`FromBodyAttribute` 和 `FromRouteAttribute` 是 ASP.NET Core Web API 中常用的兩種屬性。它們都是用來綁定 HTTP 請求中的參數，但是它們的使用方式有所不同](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)[1](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)。

- [`FromBodyAttribute` 用於綁定 HTTP 請求的主體部分。當您使用 `FromBodyAttribute` 時，您指定的參數是從請求主體中而不是從請求 URL/URI 中傳遞的。您不能在 `HttpGet` 請求中使用此屬性，只能在 `PUT`、`POST` 和 `Delete` 請求中使用。在 Web API 中，每個動作方法只能使用一個 `FromBodyAttribute` 標記](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)[1](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)[2](https://stackoverflow.com/questions/34529346/why-do-i-need-frombody-attribute-when-expecting-data-in-post-body)。
    
- [`FromRouteAttribute` 用於綁定當前請求的路由數據。它會查看您的路由參數並根據該參數提取/綁定數據。由於路由在外部調用時通常基於 URL，因此它與 `FromUri` 在以前的 Web API 版本中是可比的](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)[1](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)。
    

以下是一些使用這些屬性的示例：

C#AI 產生的代碼。請仔細檢視及使用。 。

```csharp
[Route("api/[controller]")]
[ApiController]
public class MyController : ControllerBase
{
    // GET api/my/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        return "value";
    }

    // POST api/my
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }
}
```

[在上面的代碼中，`HttpGet` 屬性使用 `FromRouteAttribute` 綁定路由參數 `id`，而 `HttpPost` 屬性使用 `FromBodyAttribute` 綁定請求主體中的字符串值](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)[1](https://stackoverflow.com/questions/37276932/what-the-difference-between-fromroute-and-frombody-in-a-web-api)。

希望這可以幫助您了解這兩種屬性的區別和使用方式。


#### AI
.net core web 應用程式 (MVC) 和 .net core Web api 都可以編輯service等http請求功能? 上述兩種差別在哪裡?
我在使用C# visual code架設網頁，當中需要http請求功能，請問我將http請求功能寫在api專案，並連接mvc專案比較好? 還是通通寫在mvc專案中就好?

[`.NET Core Web API` 和 `.NET Core MVC` 都可以編輯 `HTTP` 請求功能，但是它們的主要區別在於它們的目的和用途](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[1](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[2](https://www.c-sharpcorner.com/UploadFile/2b481f/difference-between-mvc-and-web-api/)。
- [`.NET Core Web API` 是一種專門用於創建 `RESTful` 服務的框架。它提供了一種簡單的方式來創建和公開 `API`，以便其他應用程序可以使用它們。它通常用於創建 `Web API`，並且可以使用多種格式（如 `JSON`、`XML` 等）來傳輸數據](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[1](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[2](https://www.c-sharpcorner.com/UploadFile/2b481f/difference-between-mvc-and-web-api/)。
- [`.NET Core MVC` 是一種用於創建 Web 應用程序的框架。它提供了一種模型-視圖-控制器（`MVC`）的設計模式，用於創建動態 Web 頁面。它通常用於創建 Web 應用程序，並且可以使用多種格式（如 `HTML`、`JSON`、`XML` 等）來傳輸數據](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[1](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[2](https://www.c-sharpcorner.com/UploadFile/2b481f/difference-between-mvc-and-web-api/)。

[如果您需要創建一個 `Web API`，那麼使用 `.NET Core Web API` 是最好的選擇。如果您需要創建一個動態的 Web 頁面，那麼使用 `.NET Core MVC` 是最好的選擇。如果您需要在同一應用程序中創建這兩種類型的應用程序，那麼您可以將它們放在同一個項目中，並使用它們來創建您需要的功能](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[1](https://stackoverflow.com/questions/11350853/asp-net-webapi-vs-mvc)[2](https://www.c-sharpcorner.com/UploadFile/2b481f/difference-between-mvc-and-web-api/).

希望這可以幫助您了解這兩種框架的區別和用途。


#### 解決 我有問題 - 不能把put功能給api，沒有json功能
![[N08-P05.png]]
- 不能把put功能給api，沒有json功能

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