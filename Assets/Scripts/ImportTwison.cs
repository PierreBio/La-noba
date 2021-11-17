using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ImportTwison : MonoBehaviour
{
    public TextAsset jsonFile;

    public static ImportTwison _instance;

    public Nodes storyNodes;

    public IDictionary<string, string> variableDictionnary = new Dictionary<string, string>();


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            storyNodes = JsonUtility.FromJson<Nodes>(jsonFile.text);

            initialize();
        }
        else
        {
            Destroy(this);
        }
    }

    public void initialize()
    {
        foreach (Node node in ImportTwison._instance.storyNodes.passages)
        {
            if (node.links != null)
            {
                //PARCOURT TOUTES LES VARIABLES
                List<int> listIndexVariableDefinitions = HandleString.AllIndexesOf(node.text, "(set: $");
                List<int> listEndParenthesis = HandleString.AllIndexesOf(node.text, ")");

                if (listIndexVariableDefinitions != null)
                {
                    for (var i = 0; i < listIndexVariableDefinitions.Count; i++)
                    {
                        int indexEndParenthesis = 0;

                        for (var j = 0; j < listEndParenthesis.Count; j++)
                        {
                            if (listEndParenthesis[j] > listIndexVariableDefinitions[i])
                            {
                                indexEndParenthesis = listEndParenthesis[j];
                                break;
                            }
                        }

                        string contentVariable = node.text.Substring(listIndexVariableDefinitions[i], System.Math.Abs((indexEndParenthesis + 1) - listIndexVariableDefinitions[i]));

                        string variableName = HandleString.getBetween(contentVariable, "(set: $", " to");
                        string variableValue = HandleString.getBetween(contentVariable, "to \"", "\")");

                        if (!variableDictionnary.ContainsKey(variableName)) //SI LE DICTIONNAIRE N'A PAS ENCORE ENREGISTRE CETTE VARIABLE ALORS ON L'ENREGISTRE
                        {
                            variableDictionnary.Add(new KeyValuePair<string, string>(variableName, variableValue));
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class Link
{
    public int pid;
    public string name;
    public string link;
}

[System.Serializable]
public class Node
{
    public int pid;
    public string text;
    public Link[] links;
    public string name;
}

[System.Serializable]
public class Nodes
{
    public Node[] passages;
}

public static class DeepCopy
{
    public static Node DeepCopyNode(Node n)
    {
        Node temp = new Node();
        temp.pid = n.pid;
        temp.text = n.text;
        temp.links = n.links;
        temp.name = n.name;

        return temp;
    }
}

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