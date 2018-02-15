<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Namespace>System</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var result = authorize().Dump();
}
async Task<String> authorize()
{
	HttpClient client = new HttpClient();
	var values = new Dictionary<string, string> {
  	{"username", "XXX"},
	{"password", "XXX"},
	{"grant_type", "password"}
};
	var content = new FormUrlEncodedContent(values);
	var response = await client.PostAsync("https://api.safetyculture.io/auth", content);	
	var responseString = await response.Content.ReadAsStringAsync();
	return responseString;	
}