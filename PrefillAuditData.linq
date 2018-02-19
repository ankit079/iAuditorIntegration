<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Microsoft.AspNet.WebApi.Client</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Formatting</Namespace>
  <Namespace>System.Net.Http.Handlers</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/* This program gets all audits and time of modification from iAuditor API. 
Reference of API implementation was sourced from 
https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
iAuditor Developer API (https://developer.safetyculture.io/)
*/

class Program
{
	static string path = "https://api.safetyculture.io/audits/search?field=audit_id&field=modified_at";	
	static HttpClient client = new HttpClient();
	//HttpClient is intended to be instantiated once and reused throughout the life of an application.
	static List<string> apipath = new List<string>();
	
	static void Main()
	{
		
		Program P = new Program();
		var response = P.getAuditId(path);
		
		foreach(var audit in response.Result.audits)
		{
			var eachAuditId = audit.audit_id;
			var path = string.Concat("https://api.safetyculture.io/audits/" + eachAuditId);
			apipath.Add(path);			
		}
		//apipath.Dump();		
		var eachAuditResult = P.getEachAuditResult(apipath);
		eachAuditResult.Dump();		
	}

	static void Authentication()
	{	
		client.BaseAddress = new Uri("https://api.safetyculture.io/");
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		client.Timeout = TimeSpan.FromSeconds(30);
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "f15b0066dd556c6a40e9b1572ab804d10071e5d37067ce8e0766347c43322d9d");
	}

	async Task<Result> getAuditId(string path)
	{
		Authentication();
		Result result = null;
		
		var response = await client.GetAsync(path);
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
	
	//Dummy audit path for testing
	string _apipath = "https://api.safetyculture.io/audits/audit_ba172c751d38419eaa0f61f9b50cc149";
	
	async Task<AuditData> getEachAuditResult(List<string> apipath)
	{	AuditData auditData = null;		
		var response = await client.GetAsync(_apipath);
		var result = await response.Content.ReadAsStringAsync();
		auditData = JsonConvert.DeserializeObject<AuditData>(result);		
		return auditData;
		//		foreach(var ap in apipath.Take(1))
//		{
//			var response = await client.GetAsync(ap);
//			var result = await response.Content.ReadAsStringAsync();
//			//result.Dump();
//		}						
	}
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