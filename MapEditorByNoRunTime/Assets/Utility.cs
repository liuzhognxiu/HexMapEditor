using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Utility
{
    static public void DebugOutputGrid(string[,] grid)
    {
        string textOutput = "";
        for (int y = 0; y < grid.GetUpperBound(1); y++)
        {
            for (int x = 0; x < grid.GetUpperBound(0); x++)
            {
                textOutput += grid[x, y];
                textOutput += "|";
            }

            textOutput += "\n";
        }

        Debug.Log(textOutput);
    }

    // splits a CSV file into a 2D string array
    static public string[,] SplitCsvGrid(string csvText)
    {
        string[] lines = csvText.Split("\n"[0]);

        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[width + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];

                // This line was to replace "" with " in my output. 
                // Include or edit it as you wish.
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    // splits a CSV row 
    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
                @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
                System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
            select m.Groups[1].Value).ToArray();
    }

    static void Serializer()
    {
    }

    public static void SaveStringToFile(string filePath, string data)
    {
        StreamWriter streamWriter = new FileInfo(filePath).CreateText();
        if (streamWriter == null)
        {
            Debug.LogError(string.Format("Save file {0} Error!", filePath));
            return;
        }

        streamWriter.Write(data);
        streamWriter.Close();
    }

    public static string LoacCSV(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            Debug.LogWarning(string.Format("File {0} not exist!", filePath));
            return null;
        }

        StreamReader streamReader = fileInfo.OpenText();
        if (streamReader == null)
        {
            Debug.LogError(string.Format("Load file {0} error!", filePath));
            return null;
        }

        string result = streamReader.ReadToEnd();
        streamReader.Close();
        return result;
    }

    private static T ReadOneDataConfig<T>(string csvFile)
    {
        string Path = tablePath + csvFile + ".txt";
        string result = Utility.LoacCSV(Path);

        string[,] CSVresult = Utility.SplitCsvGrid(result);
        Utility.DebugOutputGrid(CSVresult);
        for (int i = 2; i < CSVresult.GetUpperBound(0) - 1; i++)
        {
            string ProPertyname = CSVresult[i, 0];
            string ProPerty = CSVresult[i, 1];
            string ProPertyString = "public {0} {1};";
        }

        return default(T);
    }

    private static string tablePath
    {
        get
        {
            string curPath = Application.dataPath;

            string str1 = "test/Assets";

            if (curPath.Contains(str1))
            {
                return curPath.Replace(str1, "test/Assets/CSVTable");
            }

            return string.Empty;
        }
    }

    public static Color GetColorByEnum(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.red:
                return Color.red;
            case ColorType.blue:
                return Color.blue;
            case ColorType.green:
                return Color.green;
            case ColorType.gray:
                return Color.gray;
        }

        return Color.white;
    }

    public static GUISkin GetSkin()
    {
        return (GUISkin) AssetDatabase.LoadAssetAtPath("Assets/Editor/guiskin.guiskin", typeof(GUISkin));
    }
}

public enum ColorType
{
    [Header("不可走")] red = 0,
    [Header("可走")] green = 1,
    [Header("加速")] gray = 2,
    [Header("水潭")] blue = 3,
}

public enum OptionalToggle
{
    Ignore,
    Yes,
    No
}