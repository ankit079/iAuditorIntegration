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
	
	static void Main()
	{
		
		Program P = new Program();
		P.preFillAuditData();
		//result.Dump();
	}

	static void Authentication()
	{	
		client.BaseAddress = new Uri("https://api.safetyculture.io/");
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		client.Timeout = TimeSpan.FromSeconds(30);
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "f15b0066dd556c6a40e9b1572ab804d10071e5d37067ce8e0766347c43322d9d");
	}

	async Task<Uri> CreateEmptyAuditFromTemplate()
	{
		Authentication();
		var values = new Dictionary<string, string> {
  				{"template_id", "template_e1a05cecebfb461183a21b57d9685203"}};
		var content = new FormUrlEncodedContent(values);
		var response = await client.PostAsJsonAsync("https://api.safetyculture.io/audits", JsonConvert.SerializeObject(values));
		response.EnsureSuccessStatusCode();
		return response.Headers.Location;
	}
	
	//NuGet Package ExcelDataReader
	//Reference (https://github.com/ExcelDataReader/ExcelDataReader)
	
	public void preFillAuditData()
	{
		string filePath = @"C:\Dev\iAuditorIntegration\PM-#13374002-v6B-WORM_Site_Audit_Sheet.XLSM";
		FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

		//Reading from a OpenXml Excel file (2007 format; *.xlsx)
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
	
		//DataSet - The result of each spreadsheet will be created in the result.Tables
		DataSet result = excelReader.AsDataSet();

		//Data Reader methods
//		while (excelReader.Read())
//		{
//			//excelReader.GetInt32(0);
//
//		}
		// Count number of rows and columns	
		var columnCount = result.Tables[2].Columns.Count.ToString();
		var rowCount = result.Tables[2].Rows.Count.ToString();
		DataTable table =result.Tables["Modified sitelist"];
		var colResult = table.Columns.Cast<DataColumn>().Where(x =>x.ColumnName == "Column2");	//StartsWith("C", StringComparison.InvariantCultureIgnoreCase)
		//colResult.Dump();
		int i =0;
//		foreach(DataColumn col in result.Tables[2].Columns)
//		{
			//Cast operator will convert the DataRow into IEnumerable<T> to use LINQ expression
			
			foreach (DataRow row in result.Tables[2].Rows.Cast<DataRow>().Skip(1))
			{
				Console.WriteLine(row["Column2"]);
				i = i+1;
			}
		
//		}
			Console.WriteLine(i);		
		
		// Use the Select method to find all rows matching the filter.
//		DataRow[] foundRows = table.Select();
		//foundRows.Dump();
//		// Print column 0 of each returned row.
//		for (int i = 0; i < foundRows.Length; i++)
//		{
//			Console.WriteLine(foundRows[i][0]);
//		}
//		foreach (DataColumn col in result.Tables[2].Columns)
//		{					
//			//col.Dump();
//			foreach (DataRow row in result.Tables[2].Rows)
//			{
//				var value = row[col.ColumnName].ToString().Dump();
//				//Console.WriteLine(row[col.ColumnName].ToString());
//			}
//		}
		//Free resources (IExcelDataReader is IDisposable)
		excelReader.Close();
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
		
		if(response.IsSuccessStatusCode)
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

	//Dummy audit path for testing
	//string _apipath = "https://api.safetyculture.io/audits/audit_ba172c751d38419eaa0f61f9b50cc149";
	
	async Task<List<AuditData>> getEachAuditResult(List<string> allAuditsApiPath)
	{	AuditData auditData = null;	
		List<AuditData> auditResult = new List<AuditData>();
		foreach (var auditApiPath in allAuditsApiPath)
		{
			var response = await client.GetAsync(auditApiPath);
			var result = await response.Content.ReadAsStringAsync();
			auditData = JsonConvert.DeserializeObject<AuditData>(result);
			auditResult.Add(auditData);
		}		
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
		string path = string.Concat("https://api.safetyculture.io/groups/",groupId,"/users");
		Authentication();		
		var response = await client.GetAsync(path);
		var result = await response.Content.ReadAsStringAsync();		
		return result;
	}

	//	async Task ShareAudit(string path)
	//	{
	//		string auditSharepath = "https://api.safetyculture.io/audits/audit_e7b35a42aa2841ad9baf3184bd902b47/share";	
	//		Authentication();
	//		var values = new Dictionary<string, string> {
	//  				{"template_id", "template_e1a05cecebfb461183a21b57d9685203"}};
	//		var content = new FormUrlEncodedContent(values);
	//		var response = await client.PostAsJsonAsync("https://api.safetyculture.io/audits", JsonConvert.SerializeObject(values));
	//		response.EnsureSuccessStatusCode();
	//		return response.Headers.Location;
	//
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
	public string item_id { get; set; }
	public string label { get; set; }
	public string type { get; set; }
	public List<string> children { get; set; }
	public string parent_id { get; set; }
	public Responses2 responses {get; set;}
}

public class Item
{
	public string parent_id { get; set; }
	public string item_id { get; set; }
	public string label { get; set; }
	public string type { get; set; }
	public Responses responses {get; set;}
}

public class Responses
{
	public string text {get; set;}
}

public class Responses2
{
	public string text {get; set;}
}