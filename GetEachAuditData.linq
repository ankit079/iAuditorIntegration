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
//		client.DefaultRequestHeaders.Accept.Clear();
//		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
	
	async Task<string> getEachAuditResult(List<string> apipath)
	{	
		var response = await client.GetAsync(apipath[0].ToString());
		var result = await response.Content.ReadAsStringAsync();
		return result;
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