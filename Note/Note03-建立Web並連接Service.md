# Summary
- 建立ServiceURL類別
- appsetting.json加入ServiceURL路徑
- Program.cs註冊 ServiceURL服務
- MVC控制器注入ServiceURL建構子

# Step01 建立ASP.NET core Web MVC專案
- 加入MVC控制器
- 加入Razor檢視
- ## RecordsController.cs
```cs
public IActionResult Patients(String chartNo)
{
	return View();
}
```

# Step02 註冊Service服務
- 將服務註冊到Web
## ServiceURL (Web)
- 注意要加上public，不然後面會找不到
```cs
namespace Sample05_Web.Models
{
    public class ServiceURL
    {
        public String chartQry { get; set; }
    }
}
```
## appseteing.json  (Web)
```json
"ServiceURL": {
	"chartQry": "https://localhost:7138/api/Records/qry/{0}/rawdata"
}
```
## Program.cs  (Web)
```cs
//------------註冊服務: ServiceURL------------
ServiceURL serviceURL = new ServiceURL();
ConfigurationManager manager = builder.Configuration;
IConfigurationSection section = manager.GetSection("ServiceURL");
section.Bind(serviceURL);
builder.Services.AddSingleton(serviceURL);
//----------------------------
```

# Step03 ViewData綁定ServiceURL
## RecordsController.cs (Web)
- 建構子注入
- ViewData綁定
```cs
using Microsoft.AspNetCore.Mvc;
using Sample05_Web.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;



namespace Sample05_Web.Controllers
{
    public class RecordsController : Controller
    {
        private ServiceURL _serviceURL;
        public RecordsController(ServiceURL serviceURL) 
        {
            _serviceURL = serviceURL;
        }

        public IActionResult Patients()
        {
            String url = this._serviceURL.chartQry;
            ViewData["url"] = url;
            return View();
        }
    }
}
```
## Patients.cshtml (Web)
- ViewData
- console.log確認綁定
```cs
@{

    String qryChartNo = ViewData["qryChartNo"] as String;
}
<script>
    var qryChartNo = '@Html.Raw(qryChartNo)';

</script>
```

```cs
<script>
    var data = {
        chartNo: "",
        chartAPI: qryChartNo,
        result: []
    }
    var methods ={
        qryChartNoHandler: function (e) {
            console.log('qryChartNoHandler');
            console.log(this.chartNo);
            console.log(this.chartAPI);
            let reUrl = app.chartAPI.replace('{0}', this.chartNo);
            console.log(reUrl);
        }
    }
    var app = new Vue(
        {
            data: data,
            methods: methods,
        }
    );
    app.$mount('#app');
</script>
```
![image](https://github.com/Riley-Shu/Web-Medical_Record/blob/master/Note/image/N03-P01.png)


