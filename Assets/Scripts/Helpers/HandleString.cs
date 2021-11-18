using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HandleString
{
    public static List<int> AllIndexesOf(this string str, string value)
    {
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }

    public static string getAfter(string strSource, string strBeforeAfter)
    {
        return strSource.Substring(strSource.LastIndexOf(strBeforeAfter) + strBeforeAfter.Length);
    }

    public static string getBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);

            return strSource.Substring(Start, System.Math.Abs(End - Start));
        }

        return "";
    }

    public static string getBetweenIndexes(string strSource, int indexStart, int indexEnd)
    {
        return strSource.Substring(indexStart, System.Math.Abs(indexStart - indexEnd));
    }

    public static string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    public static List<string> SplitAtOccurence(this string input, char separator, int occurence)
    {
        var parts = input.Split(separator);
        var partlist = new List<string>();
        var result = new List<string>();
        for (int i = 0; i < parts.Length; i++)
        {
            if (partlist.Count == occurence)
            {
                result.Add(string.Join(separator.ToString(), partlist));
                partlist.Clear();
            }
            partlist.Add(parts[i]);
            if (i == parts.Length - 1) result.Add(string.Join(separator.ToString(), partlist)); // if no more parts, add the rest
        }
        return result;
    }
}