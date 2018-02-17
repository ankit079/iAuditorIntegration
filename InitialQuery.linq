<Query Kind="Program">
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SDKs\Azure\.NET SDK\v2.9\bin\plugins\Diagnostics\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{
	var response = getAuditId();
	response.Dump();
}

static HttpClient client = new HttpClient();
async Task<string> getAuditId()
{
	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "f15b0066dd556c6a40e9b1572ab804d10071e5d37067ce8e0766347c43322d9d");
	var response = await client.GetAsync("https://api.safetyculture.io/audits/search?field=audit_id&field=modified_at");
	var responseString = await response.Content.ReadAsStringAsync();
	//var res = JsonConvert.DeserializeObject(responseString);
	return responseString;
}

async Task getaudits()
	
{
	var response = await client.GetAsync("https://api.safetyculture.io/audits/search?field=audit_id&field=modified_at");
	var responseString = await client.GetStringAsync("https://api.safetyculture.io/audits/search");
	responseString.Dump();	
}