using Excel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Excel 文件处理器
/// </summary>
public class ExcelAgent
{

	#if UNITY_EDITOR
	/// <summary>
	///	加载Excel表的数据
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
    public static DataSet LoadExcelToDataSet(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogFormat("<color=red># Can't Find excel file at path: {0}</color>", filePath);
            return null;
        }
        if (!CheckExcelAvailable(filePath))
            return null;

        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result;

    }


	/// <summary>
	/// 从Excel表内读取数据
	/// </summary>
	/// <param name="excelPath"></param>
	/// <param name="tableName"></param>
	/// <returns></returns>
	public static DataTable ReadTableFromExcel(string excelPath, string tableName)
	{
		DataSet ds = LoadExcelToDataSet(excelPath);
		if(ds != null)
		{
			if(ds.Tables.Contains(tableName)) return ds.Tables[tableName]; 
		}
		Debug.LogError(">>> Can't Find		"+tableName+" 	Excel from "+ excelPath);
		return null;
	}

	public static HeaderRow GetHeaderRowFromTable(DataTable table,  string headerKey = "ID")
	{
		int i = 0;
		while(i < table.Rows.Count)
		{
			DataRow row = table.Rows[i];

			if(row.ItemArray[0].ToString().Equals(headerKey))
			{
				return (new HeaderRow(table, i));
			}
			i++;
		}
		return null;
	} 


	#endif

	

	public static bool IsFileInUse(string fileName)  
 	{  
        bool inUse = true;  
  
        FileStream fs = null;  
        try  
        {  
  
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,  
  
            FileShare.None);  
  
            inUse = false;  
        }  
        catch  
        {  
  
        }  
        finally  
        {  
            if (fs != null) fs.Close();  
        }  
        return inUse;//true表示正在使用,false没有使用  
	}  




	public static bool CheckExcelAvailable(string filePath)
	{
		bool res = IsFileInUse(filePath);
		if(res) 
		{
			string title = "Excel File is opened!";
			string message =string.Format( "Can't open excel file at path: <color=blue>{0}</color>\nYou should close it first." ,filePath);
			
			if(EditorUtility.DisplayDialog(title, message, "Open Path", "Cancel"))
			{
				Application.OpenURL(Path.GetDirectoryName(filePath));
			}
		}


		return !res;
	}



}



public class HeaderRow
{

	private Dictionary<string, HeaderRowItem> headItems;

	public int rowIndex = 0;


	public DataTable table;

	public HeaderRow(DataTable t, int index)
	{
		table = t;

		headItems = new Dictionary<string, HeaderRowItem>();
		rowIndex = index;
		int i = 0;
		while(i < table.Rows[index].ItemArray.Length)
		{
			HeaderRowItem item = new HeaderRowItem();
			item.key = table.Rows[index].ItemArray[i].ToString();
			item.index = i;
			headItems[item.key] = item;
			i++;
		}

	}

	public string[] Keys { get { return new List<string>(headItems.Keys).ToArray(); }}



	public int HeaderIndex(string key)
	{
		if(HasKey(key)) return headItems[key].index;
		return -1;
	}


	public bool HasKey(string key) 
	{
		return headItems.ContainsKey(key);
	}

}


public class HeaderRowItem
{
	public string key;
	public int index;
}
