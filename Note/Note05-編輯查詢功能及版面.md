# Part01 修飾表格&隱藏欄位
## Patients.cshtml
-  v-show="showQry"
- table style="background-color:azure"
```html
 <fieldset v-show="showQry">
 <div>
     <div>記錄數: {{result.length}}</div>
     <table style="background-color:azure">
         <thead>
             <tr>
                 <td>病歷號</td>
                 <td>姓名</td>
                 <td>性別</td>
                 <td>生日</td>
                 <td>看診日期</td>
             </tr>
         </thead>
         <tbody>
             <tr v-for="item in result">
                 <td>{{item.chartNo}}</td>
                 <td>{{item.name}}</td>
                 <td>{{item.sex}}</td>
                 <td>{{item.birthday}}</td>
                 <td>{{item.visitDate}}</td>
             </tr>
         </tbody>
     </table>
 </div>
 </fieldset>
```

```cs
 var data = {
     chartNo: "",
     chartAPI: qryChartNo,
     result: [],
     showQry:false

 }
 var methods ={
     qryChartNoHandler: function (e) {
         app.showQry = false;
         console.log('qryChartNoHandler');
         console.log(this.chartNo);
         console.log(this.chartAPI);
         let reUrl = app.chartAPI.replace('{0}', this.chartNo);
         console.log(reUrl);
         axios.get(reUrl)
         .then(
             (r) => { 
             console.log("then")
             console.log(r)
                     app.result = r.data
                     app.showQry = true;
             }
         )
         .catch(
                 (r) => {
                     console.log("catch")
                     console.log(r)
                     console.log(r.data)
                 })
     }
 }
```

![[N05-P01.png]]

# Part02 錯誤訊息
## Message.cs (Service)
- 建立Message類別為範本
```cs
namespace Sample05_Service.Models
{
    public class Message
    {
        public Int32 code { get; set; }
        public String msg { get; set; }
    }
}
```
## qryChartNoController.cs (Service)
```cs
//Http Get取得資料，查詢對象: chartNo
[HttpGetAttribute]
[RouteAttribute("qry/{chartNo}/rawdata")]
[Produces("application/json")] //指定回傳JSON格式
[ProducesResponseType(typeof(List<Chart>),200)] //指定正確回傳
[ProducesResponseType(typeof(Message), 400)] //指定錯誤回傳
[DisableCors] //關閉Cors
[EnableCors("myweb")] //開放Cors
public IActionResult chartQry([FromRouteAttribute(Name="chartNo")] String chartNo)
{
	//Part1: LINQ查詢
	var chartNoResult = (from i in _context.PersonalInformation where i.chartNo == chartNo select i).ToList();
	//Part2: 判斷並回傳結果
	if (chartNoResult.Count > 0)
	{
		return this.Ok(chartNoResult);
	}
	else
	{
		Message msg = new Message()
		{
			code = 400,
			msg = $"查無此病歷號:{chartNo}，請確認後重新查詢。"
		};
		return this.StatusCode(400,msg);
	}
}
```
- Postman測試
![[N05-P02.png]]
## Patients.cshtml 
- 寫進前端
```html
<fieldset v-show="showMsg">
	@* 錯誤訊息欄位 *@
	<div>
		<h4>{{msg}}</h4>
	</div>
</fieldset>
```

```CS
var data = {
    chartNo: "",
    chartAPI: qryChartNo,
    result: [],
    showQry:false,
    msg:'',
    showMsg:false
}
var methods ={
    qryChartNoHandler: function (e) {
        //Part1: 刷新頁面
        app.showQry = false;
        app.showMsg = false;

        //Part2: 取得ServiceURL，並使用axios取得資料
        // console.log(this.chartNo);
        // console.log(this.chartAPI);
        let reUrl = app.chartAPI.replace('{0}', this.chartNo);
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
				app.showMsg= true;
			}
        )
    }
}
```
![[N05-P03.png]]

# Part03 使用moment改變日期格式
- #JavaScript/moment
- https://momentjs.com/ 
- `moment.js` 是一個 JavaScript 函式庫，用於解析、驗證、操作和格式化日期和時間。它支持多種語言環境、相對時間和日曆時間，以及各種格式。可以使用它來創建日期和時間對象，並對它們進行各種操作，例如添加或減去時間、格式化日期和時間、比較日期和時間等等。
## Layout.cshtml
```html
    <script src="~/js/moment.js"></script>
    <script src="~/js/moment.min.js></script>
    <script src="~/js/moment-with-locales.js"></script>
    <script src="~/js/moment-with-locales.min.js"></script>
```
## Patients.cshtml 
- 使用 `Vue.filter`，您需要在 Vue 實例之外定義過濾器
```cs
    Vue.filter("dateFormatter",
        function (date) {
            return moment(date).format("YYYY-MM-DD");
        }
    );
```

- 在 Vue 模板中，使用 `{{ }}` 語法來應用過濾器。
```html
<td>{{item.birthday| dateFormatter}}</td>
<td>{{item.visitDate| dateFormatter}}</td>
```

![[N05-P04.png]]

# Part04 使用moment顯示現在時間
## Patients.cshtml 
- 頁面
```cs
<div><h6>{{today()}}</h6></div> //this
```
- function
```cs
today:function(){
	return moment().format("日期: YYYY-MM-DD 時間:HH:mm:ss")
}
```
