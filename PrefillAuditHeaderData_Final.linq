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
		Program P = new Program();
		Authentication();
		//var auditIds = P.getAllAuditIdsFromPerticularDay().Dump();
		//var result = P.getAllAuditIds();
		//var apipath = P.getAllAuditIdApiPath(result.Result).ToArray();
		//apipath.Dump();
		//	var users = P.getListOfUsers();
		//	users.Dump();
						var prefillData = P.preFillAuditData();	
		//				prefillData.Dump();

//						int auditCount = 53;
//						for (auditCount= 53; auditCount < 106; auditCount++)
//						{
//							//Step 1
//							var startAuditData = P.StartAudit(prefillData, auditCount);	
//							startAuditData.Dump();
//		
//							//Step 2 - Modify Audit
//							
//							//var auditResult = P.getEachAuditResult(apipath[auditCount]).Result;
//							//var finalresult = P.modifyAudit(auditResult,prefillData,auditCount);
//							//finalresult.Dump();
//						}
		//		//P.StartAudit(prefillData, auditCount);


		//Step 3: To get all auditIds
		//		var result = P.getAllAuditIds();
		//		result.Dump();
		
		//var data = P.getAllAuditIdsFromPerticularDay();
		//data.Dump();
		//Step 4: Share Audit
		var data = P.shareAudit();		
	}

	static void Authentication()
	{
		client.BaseAddress = new Uri("https://api.safetyculture.io/");
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		client.Timeout = TimeSpan.FromSeconds(30);
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
		//client.Dispose();
	}

	async Task<string> StartAudit(List<excelPreAuditData> preFillAuditData, int auditCount)
	//public void StartAudit(List<excelPreAuditData> preFillAuditData,int auditCount)
	{
		//Authentication();	
		excelPreAuditData[] dataArray = preFillAuditData.ToArray();
		//		var headerauditFeeds = new List<StartAuditFeedItems>
		//		{
		//			new StartAuditFeedItems {item_id = "f3245d39-ea77-11e1-aff1-0800200c9a66",
		//									label = "Title Page",
		//									type = "textsingle",
		//									responses = new Responses1{text =dataArray[auditCount].functionLocation.ToString()}}			}
		//		}

		var auditFeeds = new List<StartAuditFeedItems>
		{
			new StartAuditFeedItems{item_id = "fb41b026-72b3-43c2-9202-0a51a838d5e0",
									label = "Site Functional Location",
									type = "textsingle",
									responses = new Responses1{text =dataArray[auditCount].functionLocation.ToString()}},
									
//			new StartAuditFeedItems{item_id = "01638c1e-68e4-4bc2-a9df-ecbd472e49a6",
//									label = "Site Audit Number",
//									type = "textsingle",
//									responses = new Responses1{text = dataArray[auditCount].auditNumber.ToString()}},
									
			new StartAuditFeedItems{item_id = "641cb666-cf76-452d-b9a4-7588c8489e7e",
									label = "Site Description",
									type = "textsingle",
									responses = new Responses1{text =dataArray[auditCount].stationName.ToString()}},

			new StartAuditFeedItems{item_id = "9d26c1a6-4736-4ce8-bc4a-776ab173b8d5",
									label = "Address",
									type = "text",
									responses = new Responses1{text =dataArray[auditCount].address.ToString()}},
									
			new StartAuditFeedItems{item_id = "16a3ad4d-61e1-4ba5-9d13-62182d1c2ed3",
									label = "AutoSave Site Name",
									type = "textsingle",
									responses = new Responses1{text =dataArray[auditCount].autoSaveLocation.ToString()}}
		};

		JObject rss =
			new JObject(
					//new JProperty("header_items",
					new JObject(
						new JProperty("template_id", "template_e1a05cecebfb461183a21b57d9685203"),
						//new JProperty("audit_id", "http://james.newtonking.com"),
						//new JProperty("description", "James Newton-King's blog."),
						new JProperty("header_items",
							new JArray(
								from p in auditFeeds
								orderby p.item_id
								select new JObject(
									new JProperty("item_id", p.item_id),
									new JProperty("label", p.label),
									new JProperty("type", p.type),
									new JProperty("responses",
										new JObject(
											new JProperty("text", p.responses.text))))))));

		//rss.ToString().Dump();

		/*var values = new Dictionary<string, string> {
  				{"template_id", "template_e1a05cecebfb461183a21b57d9685203"}};
		var content = new FormUrlEncodedContent(values);		
		var response = await client.PostAsJsonAsync("https://api.safetyculture.io/audits", JsonConvert.SerializeObject(values));*/

		var response = await client.PostAsJsonAsync("https://api.safetyculture.io/audits", JsonConvert.SerializeObject(rss));
		//response.Dump();
		response.EnsureSuccessStatusCode();
		return response.Headers.Location.ToString();
	}

	async Task<List<Audit>> getAllAuditIdsFromPerticularDay()
	{
		//Authentication();
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
		return result.audits.Where(x => x.modified_at.Hour == 16).ToList(); //
	}

	async Task<Result> getAllAuditIds()
	{
		//Authentication();
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
	
	async Task<Object> shareAudit()
	{
		var data = new List<Object>();
		//Authentication();
		var auditIds = getAllAuditIdsFromPerticularDay().Result;
		var userDetails = getListOfUsers().Result;
		userDetails.Dump();
		auditIds.Dump();
		//		var allocatedUser = new AllocatedUser();
		//		allocatedUser.id = userDetails.Where(d => d.firstname == "Michael")
		//									  .Select(x => x.user_id).FirstOrDefault();
		//		allocatedUser.permission = "edit";
		//		var values = new Dictionary<string, AllocatedUser> {
		//				  				{"shares", allocatedUser}};
		//		values.Dump();
		//		var testJson = JsonConvert.SerializeObject(values).Dump();

		JObject rss =
	new JObject(
		new JProperty("shares",
					//new JObject(				
					//	new JProperty("item",
					new JArray(
						from d in userDetails
						where d.firstname == "Scott"
						select new JObject(
							new JProperty("id", d.user_id),
							new JProperty("permission", "edit")
							))));
		//rss.ToString().Dump();

		try
		{
			foreach (var auditid in auditIds)
			{
				string shareAuditPath = string.Concat("https://api.safetyculture.io/audits/" + auditid.audit_id + "/share");
				shareAuditPath.Dump();
				var response = await client.PostAsJsonAsync(shareAuditPath, JsonConvert.SerializeObject(rss));
				response.EnsureSuccessStatusCode();
				data.Add(response);
				//response.Dump();				
			}
		}
		catch (Exception ex)
		{
			throw;
		}
		return data;
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
		//		var settings = new JsonSerializerSettings
		//		{
		//			NullValueHandling = NullValueHandling.Ignore,
		//			MissingMemberHandling = MissingMemberHandling.Ignore
		//		};
		//		var auditData = JsonConvert.DeserializeObject<AuditData>(result, settings);

		//auditResult.Add(auditData);
		//}		
		//auditData.Dump();
		return auditData;
	}

	//NuGet Package ExcelDataReader
	//Reference (https://github.com/ExcelDataReader/ExcelDataReader)

	public List<excelPreAuditData> preFillAuditData()
	{
		string filePath = @"C:\Dev\integration\PM-#18894481-v1-WORM_SIte_List_2018.XLSX";
		FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

		//Reading from a OpenXml Excel file (2007 format; *.xlsx)
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

		//DataSet - The result of each spreadsheet will be created in the result.Tables
		DataSet result = excelReader.AsDataSet();
		//result.Dump();
		// Count number of rows and columns	
		//var columnCount = result.Tables[2].Columns.Count.ToString();
		//var rowCount = result.Tables[2].Rows.Count.ToString();

		DataTable table = result.Tables["Modified sitelist"];
		//var colResult = table.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "Column2");
		List<excelPreAuditData> excellData = new List<excelPreAuditData>();
		//colResult.Dump();
		//Cast operator will convert the DataRow into IEnumerable<T> to use LINQ expression

		foreach (DataRow row in result.Tables[0].Rows.Cast<DataRow>().Skip(1))
		{
			excellData.Add(new excelPreAuditData(row["Column0"].ToString(), row["Column1"].ToString(), 
												 row["Column2"].ToString(), row["Column3"].ToString(), 
												 row["Column4"].ToString()));
		}
		excelReader.Close();
		//To query the relevant data
		return excellData;
		/*return excellData.Where((d => (d.address == "BENTLEY") || 
										(d.address == "ST JAMES") || 
										(d.address == "CANNINGTON") || 
										(d.address == "QUEENS PARK"))).ToList();*/
	}

	async Task<HttpResponseMessage> modifyAudit(AuditData auditResult, List<excelPreAuditData> preFillAuditData, int auditCount)
//	public void modifyAudit(AuditData auditResult, List<excelPreAuditData> preFillAuditData, int auditCount)
	{
		excelPreAuditData[] dataArray = preFillAuditData.ToArray();
		
		var auditFeeds = new List<ModifyAuditFeedItems>
		{
			new ModifyAuditFeedItems{item_id = "fb41b026-72b3-43c2-9202-0a51a838d5e0",									
									type = "textsingle",
									responses = new Responses1{text =dataArray[auditCount].functionLocation.ToString()}},									
									
			new ModifyAuditFeedItems{item_id = "641cb666-cf76-452d-b9a4-7588c8489e7e",									
									type = "textsingle",
									responses = new Responses1{text =dataArray[auditCount].stationName.ToString()}},

			new ModifyAuditFeedItems{item_id = "9d26c1a6-4736-4ce8-bc4a-776ab173b8d5",									
									type = "text",
									responses = new Responses1{text =dataArray[auditCount].address.ToString()}},

			new ModifyAuditFeedItems{item_id = "16a3ad4d-61e1-4ba5-9d13-62182d1c2ed3",									
									type = "textsingle",
									responses = new Responses1{text =dataArray[auditCount].autoSaveLocation.ToString()}}
		};

		JObject rss =
			new JObject(
					//new JProperty("header_items",
					new JObject(
						new JProperty("archived", false),
						//new JProperty("audit_id", "http://james.newtonking.com"),
						//new JProperty("description", "James Newton-King's blog."),
						new JProperty("header_items",
							new JArray(
								from p in auditFeeds
								orderby p.item_id
								select new JObject(
									new JProperty("item_id", p.item_id),
									//new JProperty("label", p.label),
									new JProperty("type", p.type),
									new JProperty("responses",
										new JObject(
											new JProperty("text", p.responses.text))))))));
			//rss.ToString().Dump();
			auditResult.audit_id.Dump();
				var response = await client.PutAsJsonAsync($"https://api.safetyculture.io/audits/{auditResult.audit_id}", JsonConvert.SerializeObject(rss));
				response.EnsureSuccessStatusCode();				
				return response;
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
		//Authentication();
		var response = await client.GetAsync(apiGroupPath);
		var result = await response.Content.ReadAsStringAsync();
		return result;
	}

	async Task<List<User>> getListOfUsers()
	{
		//Authentication();
		string groupId = "role_13527eda42bd4d28a36f020fee4dd0b8"; //groupName = "WaterCorp";
		string path = string.Concat("https://api.safetyculture.io/groups/", groupId, "/users");
		var response = await client.GetAsync(path);
		var result = await response.Content.ReadAsStringAsync();
		var userDetails = JsonConvert.DeserializeObject<UserDetails>(result);
		return userDetails.users.ToList();
	}
	
//	async Task exportAudit()
//	{
//		string audit_id = ""
//		string apiCall = string.Concat("https://api.safetyculture.io/audits/" + audit_id + "/export?format=pdf&timezone=Australia/Brisbane"); 
//	}
}

//Add a model class. This class matches the data model used by the web API.

public class User
{
	public string email { get; set; }
	public string firstname { get; set; }
	public string lastname { get; set; }
	public string user_id { get; set; }
	public string status { get; set; }
}

public class UserDetails
{
	public int total { get; set; }
	public int offset { get; set; }
	public int limit { get; set; }
	public List<User> users { get; set; }
}

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
	//public Responses responses { get {return this.responses;} set{ if (value != this.responses) { this.responses = null; } this.responses = value; } }
}

public class Responses
{
	//public DateTime datetime { get; set; }
	public string text { get; set; }
}

public class Responses2
{
	public string text { get; private set; }
	//public DateTime datetime { get; set; }
}

public class excelPreAuditData
{
	public string stationName { get; }
	public string functionLocation { get; }
	public string address { get; set; }
	public string autoSaveLocation { get; set; }
	public string viewXlocation { get; set; }

	public excelPreAuditData(string _stationName, string _functionLocation, string _address, string _autoSaveLocation, string _viewXlocation)
	{
		stationName = _stationName;
		functionLocation = _functionLocation;			
		address = _address;	
		autoSaveLocation = _autoSaveLocation;
		viewXlocation = _viewXlocation;
	}
}

public class StartAuditFeedItems
{
	public string item_id { get; set; }
	public string label { get; set; }
	public string type { get; set; }
	public Responses1 responses { get; set; }
	//public Responses responses { get {return this.responses;} set{ if (value != this.responses) { this.responses = null; } this.responses = value; } }
}

public class ModifyAuditFeedItems
{
	public string item_id { get; set; }	
	public string type { get; set; }
	public Responses1 responses { get; set; }	
}

public class Responses1
{
	//public DateTime datetime { get; set; }
	public string text { get; set; }
}

public class AllocatedUser
{
	public string id { get; set; }
	public string permission { get; set; }
}