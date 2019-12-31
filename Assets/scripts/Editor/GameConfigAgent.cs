using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

/// <summary>
/// 游戏配置编辑器
/// </summary>
public class GameConfigAgent
{
    #region Attributes
    protected static string K_GAMECONFIG_FILE_PATH = "GameConfig.xlsx";
    protected static string K_TABLE_NAME = "Undefined";

    protected const string K_HEAD_KEY_ID = "ID";	//Head中首个单元格必须为该字段

    // ExcelFilePath in FileSystem
    public static string GameConfigPath { get { return Path.Combine("Assets/data/", K_GAMECONFIG_FILE_PATH); } }

    static HeaderRow m_header;

    #endregion

    #region API

    protected static List<T> AccessToDataTabel<T>(Func<DataRow, T> onEachRow)
    {
        if (onEachRow == null) return null;

        DataTable table = ExcelAgent.ReadTableFromExcel(GameConfigPath, K_TABLE_NAME);
        if (table != null)
        {
            m_header = ExcelAgent.GetHeaderRowFromTable(table, K_HEAD_KEY_ID);
            if (m_header == null) return null;

            var result = new List<T>();
            for (int i = m_header.rowIndex + 1; i < table.Rows.Count; i++)
            {
                if (!IsVaildRow(table.Rows[i])) continue;

                var item = onEachRow(table.Rows[i]);
                if (item != null) result.Add(item);
            }

            Debug.LogFormat("Update {0} count : {1}", typeof(T), result.Count);
            return result;
        }
        else
        {
            Debug.Log("Can't find tabel: " + K_TABLE_NAME);
        }

        return null;
    }

    static bool IsVaildRow(DataRow row)
    {
        if (row == null) return false;
        if (m_header == null || row.ItemArray.Length < m_header.Keys.Length) return false;

        if (row.ItemArray.Length == 0) return false;
        if (string.IsNullOrEmpty(row.ItemArray[0].ToString())) return false;

        return true;
    }

    static object GetCellValue(DataRow row, int index)
    {
        if (index < 0) return null;
        if (row != null && index < row.ItemArray.Length)
        {
            return row.ItemArray[index];
        }

        return null;
    }


    public static string GetCellString(DataRow row, string key)
    {
        return GetString(GetCellValue(row, m_header.HeaderIndex(key)));
    }

    public static float GetCellFloat(DataRow row, string key)
    {
        return GetFloat(GetCellValue(row, m_header.HeaderIndex(key)));
    }

    public static int GetCellInt(DataRow row, string key)
    {
        return GetInt(GetCellValue(row, m_header.HeaderIndex(key)));
    }

    public static bool GetCellBool(DataRow row, string key)
    {
        return GetBool(GetCellValue(row, m_header.HeaderIndex(key)));
    }

    public static string[] GetCellStringArr(DataRow row, string key, char segChar = ',')
    {
        string str = GetCellString(row, key);
        if (string.IsNullOrEmpty(str)) str = "";
        return str.Split(segChar);
    }


    static string GetString(object obj)
    {
        if (obj != null) return obj.ToString();
        return "";
    }

    static float GetFloat(object obj)
    {
        string val = GetString(obj);
        if (!string.IsNullOrEmpty(val)) return float.Parse(val);
        return 0;
    }

    static int GetInt(object obj)
    {
        string val = GetString(obj);
        if (!string.IsNullOrEmpty(val)) return int.Parse(val);
        return 0;
    }

    static bool GetBool(object obj)
    {
        string val = GetString(obj);
        if (!string.IsNullOrEmpty(val))
        {
            if (val.ToLower().Equals("1") || val.ToLower().Equals("true")) return true;
        }
        return false;
    }


    #endregion




}
