<Query Kind="Program">
  <Namespace>System.Data.OleDb</Namespace>
</Query>

void Main()
{
	string con =
  @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Dev\iAuditorIntegration\Test.xlsx;" +
  @"Extended Properties='Excel 8.0;HDR=Yes;'";
	using (OleDbConnection connection = new OleDbConnection(con))
	{
		connection.Open();
		OleDbCommand command = new OleDbCommand("select * from [Sheet1$]", connection);
		using (OleDbDataReader dr = command.ExecuteReader())
		{
			while (dr.Read())
			{				
				var row1Col0 = dr[0];
				Console.WriteLine(row1Col0);				
			}
			// always call Close when done reading.
			dr.Close();
		}
	}
	

}

// Define other methods and classes here