<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>ExcelDataReader</NuGetReference>
  <NuGetReference>ExcelDataReader.DataSet</NuGetReference>
  <NuGetReference>Microsoft.AspNet.WebApi.Client</NuGetReference>
  <Namespace>ExcelDataReader</Namespace>
  <Namespace>ExcelDataReader.Core.NumberFormat</Namespace>
  <Namespace>ExcelDataReader.Exceptions</Namespace>
  <Namespace>ExcelDataReader.Log</Namespace>
  <Namespace>ExcelDataReader.Log.Logger</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Data.OleDb</Namespace>
  <Namespace>System.IO.Compression</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Formatting</Namespace>
  <Namespace>System.Net.Http.Handlers</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Data.Linq</Namespace>
</Query>

/* This program gets all audits and time of modification from iAuditor API. 
Reference of API implementation was sourced from 
https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
iAuditor Developer API (https://developer.safetyculture.io/)
*/
 
class Program
{
	static HttpClient client = new HttpClient();
	//HttpClient is intended to be instantiated once and reused throughout the life of an application.	
	static string authToken = File.ReadAllText(@"C:\Users\ankit\OneDrive\Desktop\Auth.txt");
	static void Main()
	{
		//client.Dispose();
		//Dummy audit path for testing
		//const string _apipath = "https://api.safetyculture.io/audits/audit_ba172c751d38419eaa0f61f9b50cc149";
		Program P = new Program();	
		
		//To create audits
		/*var auditCreated = P.StartAudit();	
		auditCreated.Dump();*/
		
		//To get all auditIds
		var result = P.getAllAuditIds();
		var apipath = P.getAllAuditIdApiPath(result.Result).ToArray();
		var prefillData = P.preFillAuditData();	
//		prefillData.Dump();
		int auditCount = 0;
		
		for (auditCount= 0; auditCount < 4; auditCount++)
		{
			var auditResult = P.getEachAuditResult(apipath[auditCount]).Result;
			var modifiedAuditData = P.modifyAudit(auditResult, prefillData, auditCount);
			string modifiedAuditDataJSON = JsonConvert.SerializeObject(modifiedAuditData,
   			Newtonsoft.Json.Formatting.Indented,
   			new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
   			modifiedAuditDataJSON.Dump();
		}	
		
	}

	static void Authentication()
	{
		client.BaseAddress = new Uri("https://api.safetyculture.io/");
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		client.Timeout = TimeSpan.FromSeconds(30);
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
	}

	async Task<string> StartAudit()
	{
		Authentication();
		var values = new Dictionary<string, string> {
  				{"template_id", "template_e1a05cecebfb461183a21b57d9685203"}};
		var content = new FormUrlEncodedContent(values);
		var response = await client.PostAsJsonAsync("https://api.safetyculture.io/audits", JsonConvert.SerializeObject(values));
		response.EnsureSuccessStatusCode();
		return response.Headers.Location.ToString();
	}

	async Task<Result> getAllAuditIds()
	{
		Authentication();
		Result result = null;
		string getAllAuditIdpath = "https://api.safetyculture.io/audits/search?field=audit_id&field=modified_at";
		var response = await client.GetAsync(getAllAuditIdpath);
		/*The GetAsync method sends the HTTP GET request. 
		When the method completes, it returns an HttpResponseMessage that contains the HTTP response. 
		If the status code in the response is a success code, the response body contains the JSON representation of the Result. */

		if (response.IsSuccessStatusCode)
		{
			result = await response.Content.ReadAsAsync<Result>();
			//Call ReadAsAsync to deserialize the JSON payload to the Result
		}
		return result;
	}

	public List<string> getAllAuditIdApiPath(Result getAllAuditIds)
	{
		List<string> allAuditsApiPath = new List<string>();
		foreach (var audit in getAllAuditIds.audits)
		{
			var eachAuditId = audit.audit_id;
			var path = string.Concat("https://api.safetyculture.io/audits/" + eachAuditId);
			allAuditsApiPath.Add(path);
		}
		return allAuditsApiPath;
	}


	//Removed list<AuditData> to process individual auditapipath from the main method

	async Task<AuditData> getEachAuditResult(string auditApiPath)
	{
		//Authentication();
		//AuditData auditData = null;
		//List<AuditData> auditResult = new List<AuditData>();
		//foreach (var auditApiPath in allAuditsApiPath)
		//{
		var response = await client.GetAsync(auditApiPath);
		var result = await response.Content.ReadAsStringAsync();
		//result.Dump();
		var auditData = JsonConvert.DeserializeObject<AuditData>(result);
		//auditResult.Add(auditData);
		//}		
		//auditData.Dump();
		return auditData;		
	}

	//NuGet Package ExcelDataReader
	//Reference (https://github.com/ExcelDataReader/ExcelDataReader)

	public List<excelPreAuditData> preFillAuditData()
	{
		string filePath = @"C:\Dev\iAuditorIntegration\PM-#13374002-v6B-WORM_Site_Audit_Sheet.XLSM";
		FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

		//Reading from a OpenXml Excel file (2007 format; *.xlsx)
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

		//DataSet - The result of each spreadsheet will be created in the result.Tables
		DataSet result = excelReader.AsDataSet();

		// Count number of rows and columns	
		//var columnCount = result.Tables[2].Columns.Count.ToString();
		//var rowCount = result.Tables[2].Rows.Count.ToString();

		DataTable table = result.Tables["Modified sitelist"];
		var colResult = table.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "Column2");
		List<excelPreAuditData> excellData = new List<excelPreAuditData>();

		//Cast operator will convert the DataRow into IEnumerable<T> to use LINQ expression

		foreach (DataRow row in result.Tables[2].Rows.Cast<DataRow>().Skip(1))
		{
			excellData.Add(new excelPreAuditData(row["Column2"].ToString(), row["Column0"].ToString(), row["Column1"].ToString(), string.Concat(row["Column5"] + "" + row["Column6"] + "" + row["Column7"]), row["Column8"].ToString()));
		}
		excelReader.Close();
		return excellData;
	}

	async Task<AuditData> modifyAudit(AuditData auditResult, List<excelPreAuditData> preFillAuditData, int auditCount)
	{
		Dictionary<string, List<Item>> value = new Dictionary<string, List<Item>>();
		excelPreAuditData[] dataArray = preFillAuditData.ToArray();
		//auditResult.items.Dump();
		string auditResult_JSON = JsonConvert.SerializeObject(auditResult.items,
				Newtonsoft.Json.Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
		//auditResult_JSON.Dump();
		var updateItem1 = auditResult.items.Where(i => i.label == "Site Functional Location").Select(x => x.responses); ;

		foreach (var item in updateItem1)
		{
			item.text = dataArray[auditCount].functionLocation.ToString();
		}
		var changedItems = auditResult.items.Where(i => i.label == "Site Functional Location").ToList();

		value.Add("items", changedItems);
		//value.Dump();
		string ignored = JsonConvert.SerializeObject(value,
		Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
		//ignored.Dump();
		
		//[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		
		//var updatedAuditResult = new AuditData();

//		string ignored = JsonConvert.SerializeObject(auditResult,
//		Newtonsoft.Json.Formatting.Indented,
//		new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
//		ignored.Dump();
		
		//updatedAuditResult =  auditResult.items
		//string output = JsonConvert.SerializeObject(auditResult);
		//output.Dump();
		
//		StringBuilder sb = new StringBuilder();
//		StringWriter sw = new StringWriter(sb);
//		using (JsonWriter writer = new JsonTextWriter(sw))
//		writer = auditResult.;
		
//				var updateItem1 = auditResult.items.Where(i => i.label == "Age of Site");				
//				updateItem1.Dump();
//				foreach (var item in updateItem1)
//				{
//					updatedAuditResult.items.responses.text = dataArray[auditCount].functionLocation.ToString();
//					item.item_id = auditResult.
//				}
//		var changedItems = updatedAuditResult.items.Where(i => i.label == "Age of Site").ToList();
//
//		value.Add("items", changedItems);
//
//		//		var updateItem1 = auditResult.items.Where(i =>i.label == "Site Functional Location").ToList();
//		//		value.Add("items", updateItem1);
//		//value.Dump();
//		string output = JsonConvert.SerializeObject(value);
//		output.Dump();	
//		var updateResponse2 = auditResult.items.Where(i => i.label == "Site Audit Number").Select(x => x.responses);
//
//		var updateItem2 = auditResult.items.Where(i => i.label == "Site Audit Number").ToList();
//		value.Add("items", updateItem2);
//		var updateItem3 = auditResult.items.Where(i => i.label == "Site Description").Select(x => x.responses);
//		foreach (var item in updateItem3)
//		{
//			item.text = dataArray[auditCount].stationName;
//		}
//
//		var updateItem4 = auditResult.items.Where(i => i.label == "Address").Select(x => x.responses);
//		foreach (var item in updateItem4)
//		{
//			item.text = dataArray[auditCount].address;
//		}
//		var updateItem5 = auditResult.items.Where(i => i.label == "AutoSave Site Name").Select(x => x.responses);
//		foreach (var item in updateItem5)
//		{
//			item.text = dataArray[auditCount].autoSaveLocation;
//		}
			
		HttpResponseMessage response = await client.PutAsJsonAsync($"https://api.safetyculture.io/audits/{auditResult.audit_id}", value);
		response.EnsureSuccessStatusCode();

		//Deserialize the updated product from the response body.
		//Use the following code to fetch the return response with updated audit data from server - Include Task<AuditData> as return type
		auditResult = await response.Content.ReadAsAsync<AuditData>();
		return auditResult;		
	}
	async Task<Object> addUser()
	{
		string addUserPath = "https://api.safetyculture.io/users";
		var values = new Dictionary<string, string> {
  				{"firstName", ""},
				{"listName",""},
				{"email",""},
				{"reset_password_required","true"}};
		var content = new FormUrlEncodedContent(values);
		var response = await client.PostAsJsonAsync(addUserPath, JsonConvert.SerializeObject(values));
		response.EnsureSuccessStatusCode();
		return response;
	}

	async Task<string> getListOfGroups()
	{
		string apiGroupPath = "https://api.safetyculture.io/groups";
		Authentication();
		var response = await client.GetAsync(apiGroupPath);
		var result = await response.Content.ReadAsStringAsync();
		return result;
	}

	async Task<string> getListOfUsers()
	{
		string groupId = "role_13527eda42bd4d28a36f020fee4dd0b8";
		string groupName = "WaterCorp";
		string path = string.Concat("https://api.safetyculture.io/groups/", groupId, "/users");
		Authentication();
		var response = await client.GetAsync(path);
		var result = await response.Content.ReadAsStringAsync();
		return result;
	}

	//	async Task shareAudit(string path)
	//	{
	//		string auditId = "";
	//		string shareAuditPath = string.Concat("https://api.safetyculture.io/audits/"+ auditId + "/share");
	//		Authentication();
	//		var values = new Dictionary<string, string> {
	//	  				{"template_id", "template_e1a05cecebfb461183a21b57d9685203"}};
	//		var content = new FormUrlEncodedContent(values);
	//		var response = await client.PostAsJsonAsync("https://api.safetyculture.io/audits", JsonConvert.SerializeObject(values));
	//		response.EnsureSuccessStatusCode();
	//		return response.Headers.Location;
	//	}	
}

//Add a model class. This class matches the data model used by the web API.

public class Audit
{
	public string audit_id { get; set; }
	public DateTime modified_at { get; set; }
}

public class Result
{
	public int count { get; set; }
	public int total { get; set; }
	public IList<Audit> audits { get; set; }
}

public class AuditData
{
	public string template_id { get; set; }
	public string audit_id { get; set; }
	public DateTime created_at { get; set; }
	public DateTime modified_at { get; set; }
	public AuditDetails audit_details { get; set; }
	public List<HeaderItem> header_items { get; set; }
	public List<Item> items { get; set; }
}

public class Authorship
{
	public string device_id { get; set; }
	public string owner { get; set; }
	public string owner_id { get; set; }
	public string author { get; set; }
	public string author_id { get; set; }
}

public class AuditDetails
{
	public int score { get; set; }
	public int total_score { get; set; }
	public int score_percentage { get; set; }
	public string name { get; set; }
	public int duration { get; set; }
	public Authorship authorship { get; set; }
	public object date_completed { get; set; }
	public DateTime date_modified { get; set; }
	public DateTime date_started { get; set; }
}

public class HeaderItem
{
	[JsonProperty]
	public string item_id { get; set; }
	public string label { get; set; }
	public string type { get; set; }		
	public Responses2 responses { get; set; }
}

public class Item
{
	public string item_id { get; set; }
	public string label { get; set; }
	public string type { get; set; }	
	public Responses responses { get; set; }
}

public class Responses
{
	//public object datetime { get; set; }
	public string text { get; set; }
}

public class Responses2
{
	public string text { get; set; }
	//public DateTime datetime { get; set; }
}

public class excelPreAuditData
{
	public string functionLocation { get; set; }
	public string auditNumber { get; set; }
	public string stationName { get; set; }
	public string address { get; set; }
	public string autoSaveLocation { get; set; }

	public excelPreAuditData(string _functionLocation, string _auditNumber, string _stationName, string _address, string _autoSaveLocation)
	{
		functionLocation = _functionLocation;
		auditNumber = _auditNumber;
		stationName = _stationName;
		address = _address;
		autoSaveLocation = _autoSaveLocation;
	}
}