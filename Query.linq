<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var response = getAuditId();
	response.Dump();
}

async Task<string> getAuditId()
{
	HttpClient client = new HttpClient();
	var values = new Dictionary<string, string> {
  	{"Authorization", "Bearer f15b0066dd556c6a40e9b1572ab804d10071e5d37067ce8e0766347c43322d9d"}
};
	var content = new FormUrlEncodedContent(values);
	var response = await client.PostAsync("https://api.safetyculture.io/audits/search?field=audit_id&field=modified_at", content);
	var responseString = await response.Content.ReadAsStringAsync();
	return responseString;
}

async Task getaudits()
	
{
	HttpClient client = new HttpClient();
	var responseString = await client.GetStringAsync("https://api.safetyculture.io/audits/search");
	responseString.Dump();	
}