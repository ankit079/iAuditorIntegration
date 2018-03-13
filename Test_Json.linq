<Query Kind="Program">
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SDKs\Azure\.NET SDK\v2.9\bin\plugins\Diagnostics\Newtonsoft.Json.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

//Reference: https://www.newtonsoft.com/json/help/html/SerializeWithJsonSerializerToFile.htm

void Main()
{	
	List<Item> updatedList = new List<Item>();
	var result = LoadJson();
	updatedList = result.items.Where(i =>i.label == "Site Audit Number").ToList();
	var a = updatedList.Where(i =>i.label == "Site Audit Number").Select(x=>x.responses);
	foreach(var b in a)
	{
		b.text = "3";
	}
	
	Item update1 = updatedList.Where(i =>i.label == "Site Audit Number");
	updatedList.Add(update1);
	
	var c = result.items.Where(i => i.label == "Site Functional Location").Select(x => x.responses);
	foreach (var d in c)
	{
		d.text = "FL647463";
	}
	string updatedResult = JsonConvert.SerializeObject(result);
	updatedResult.Dump();
}

// Define other methods and classes here
public AuditData LoadJson()
{
	// read file into a string and deserialize JSON to a type
	AuditData auditResult = JsonConvert.DeserializeObject<AuditData>(File.ReadAllText(@"C:\Dev\iAuditorIntegration\AuditData_Sort.json"));		
	//auditResult.Dump();
	return auditResult;
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
	public string item_id { get; set; }
	public string label { get; set; }
	public string type { get; set; }
	public List<string> children { get; set; }
	public string parent_id { get; set; }
	public Responses2 responses { get; set; }
}

public class Item
{
	public string parent_id { get; set; }
	public string item_id { get; set; }
	public string label { get; set; }
	public string type { get; set; }
	public Responses responses { get; set; }
}

public class Responses
{
	public string text { get; set; }
}

public class Responses2
{
	public string text { get; set; }
}
