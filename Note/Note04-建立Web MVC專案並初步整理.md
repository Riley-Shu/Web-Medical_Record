# Step01 初步整理版面
## Layout.cshtml (Web)
```html
<!DOCTYPE html>
<html lang="en">
<head >
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - 病歷查詢</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/Sample05_Web.styles.css" asp-append-version="true" />
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)


</head>
<body style="background-color:ivory">
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">病歷查詢</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2023 - Sample - <a asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </div>
    </footer>

</body>
</html>
```

# Step02 掛載Vue
## Layout.cshtml (Web)
- Vue
```html
<script src="~/js/vue.js"></script>
<script src="~/js/vue.min.js"></script>
```
## Patients.cshtml
- 測試渲染
```cs
@*
    For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
*@
@{
}
<fieldset id="app">
    <legend>病歷查詢</legend>
    <div>
        <label>依照病歷號</label>
        <input type="text" v-model:value="chartNo"/>
        <button v-on:click="qryChartNoHandler">查詢</button>
    </div>
    <div>{{100/5}}</div>

    <div>
        <div>記錄數: </div>
        <table>
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
                <tr>
                    <td></td>
                </tr>
            </tbody>
        </table>
    </div>
</fieldset>


<script>
    var data = {
        chartNo: "",
        result: []
    }
    var methods ={
        qryChartNoHandler: function (e) {
            console.log('qryChartNoHandler');
            console.log(this.chartNo);
        }
    }

    var app = new Vue(
        {
            data: data,
            methods: methods
            
        }
    );
    app.$mount('#app');
</script>
```
![[N04-P01.png]]

# Step03 使用axios取得SQL資料並呈現於前端
#JavaScript/axios
- 取得資料
- 將資料呈現於頁面
## Layout.cshtml
- axios
- [Getting Started | Axios Docs (axios-http.com)](https://axios-http.com/docs/intro)
- axios 是一個基於 Promise 的 HTTP 客戶端，可以在瀏覽器和 Node.js 中使用。它提供了一個簡單的 API，可以讓開發人員輕鬆地進行 HTTP 請求。

```html
<script src="https://cdn.jsdelivr.net/npm/axios/dist/axios.min.js"></script>
```
## Patients.cshtml
- axios取得資料
- v-for加入表格
```html
<tbody>
	<tr v-for="item in result">
		<td>{{item.ChartNo}}</td>
		<td>{{item.name}}</td>
		<td>{{item.sex}}</td>
		<td>{{item.birthday}}</td>
		<td>{{item.visitDate}}</td>
	</tr>
</tbody>
```

```cs
var methods ={
qryChartNoHandler: function (e) {
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
		}
	)
	.catch(
			(r) => {
				console.log("catch")
				console.log(r)
				console.log(r.data)
			})
```

![[N04-P02.png]]