# 刪除資料
## Part1 建立基本架構&對話盒取消功能
### Patients.cshtml 
#### fieldset
```html
<fieldset id="delDialog" style="display:None">
    <div style="margin:20px"></div>
    <div><h4>確定要刪除此病患資料？</h4></div>
</fieldset>
```
#### Methods
- accessKey取得選擇項目
- 備份項目資料
- 對話盒-取消
```cs
//方法: 刪除資料
delDataHandler:function(e){
    console.log('delDataHandler');
    console.log(e);
    app.showMsg = false;

    //Part1 取得選擇之病患的資料
    let accessKey = e.target.accessKey;
    this.curPatient = this.result[accessKey];
    console.log("this.curPatient");
    console.log(accessKey);
    console.log(this.curPatient);

    //Part2 備份選擇之病患的資料 (使用Object.assign()方法)
    app.index = accessKey;
    this.sourcePatient = Object.assign({}, this.curPatient); //說明: Object.assign()是JavaScript中的一種方法，用於將一個或多個源對象的所有可枚舉自有屬性複製到目標對象中。它返回修改後的目標對象。

    console.log("this.sourcePatient");
    console.log(this.index);
    console.log(this.sourcePatient);

    //Part3 打開對話盒
    $('#delDialog').dialog
    (
        {
            title: '提醒',
            modal: true, //強佔式視窗
            buttons: [
                {
                    //取消刪除
                    text: '取消',
                    class: 'btn btn-danger',
                    click: function () {
                        //還原原始資料
                        console.log("取消刪除");
                        app.result[app.index] = app.sourcePatient;
                        console.log("還原為原始資料")
                        console.log(app.sourcePatient);
                        //關閉對話盒
                        $('#delDialog').dialog('close');
                    }
                },
                {
                    //確定刪除
                    text: '刪除',
                        class: 'btn btn-primary',
                    click: function () { }
                }
            ]
        }
    )
}
```

## Part02 對話盒刪除功能
### Patients.cshtml 
- `jQuery.ajax`方法 HTTP DELETE請求
- 關閉對話盒
```cs
//Part3 打開對話盒
$('#delDialog').dialog
(
	{
		title: '提醒',
		modal: true, //強佔式視窗
		buttons: [
			{
				(中略)
				}
			},
			{
				//確定刪除
				text: '刪除',
				class: 'btn btn-primary',
				click: function ()
				{
					let service = 'https://localhost:7241/Records/delete/' + app.curPatient.chartNo + '/rawdata';
					console.log(service);
					  

					$.ajax( //說明: jQuery.ajax()是jQuery的核心方法之一，用於發送HTTP請求
						{
							url: service, // 設定服務位址
							type: 'DELETE', // 設定 HTTP 方法
								
							//成功格式
							success: function (e) {
								console.log(e);
								console.log(accessKey);
								app.result.splice(accessKey, 1); //說明: 使用splice JavaScript方法，刪除陣列中指定索引位置的元素。accessKey表示要刪除元素的索引位置，1表示要刪除的元素數量。藉此改變原始陣列，並返回被刪除的元素。
								app.msg2 = e.msg2;
							},
							//錯誤格式
							error: function (e) {
								console.log(e);
								app.msg2 = e.msg2;
							}
						}
					);
						
					$('#delDialog').dialog('close'); //關閉對話盒
					$("#msgDialog").dialog( //訊息對話盒
						{
							title: '訊息',
							modal: true,
								width: 420,
							buttons:[{
									text: '關閉',
									class: 'btn btn-primary',
									click: function () {
										$(this).dialog("close");
									}
								}
							]
						}
					);                                         
				}
			}
		]
	}
)

```
### RecordsController.cs (Web)
- HTTP DELETE請求
```cs
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
```