using System.Collections;
using System.Collections.Generic;
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

            //Rest of your Awake code
            storyNodes = JsonUtility.FromJson<Nodes>(jsonFile.text);

            foreach (Node node in ImportTwison._instance.storyNodes.passages)
            {
                if (node.links != null)
                {
                    List<int> listIndexVariableDefinitions = HandleString.AllIndexesOf(node.text, "(set: $");
                    List<int> listEndParenthesis = HandleString.AllIndexesOf(node.text, ")");

                    if(listIndexVariableDefinitions != null)
                    {
                        for (var i = 0; i < listIndexVariableDefinitions.Count; i++)
                        {
                            int indexEndParenthesis = 0;

                            for (var j = 0; j < listEndParenthesis.Count; j++)
                            {
                                if(listEndParenthesis[j] > listIndexVariableDefinitions[i])
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

                            //ENLEVE L'AFFICHAGE DE LA DECLARATION DES VARIABLES
                            //node.text = node.text.Replace(contentVariable, "");
                        }
                    }

                    //PARCOURT TOUS LES STRING ENTRE // POUR METTRE DE L'ITALIQUE
                    List<int> listItalicTagIndexes = HandleString.AllIndexesOf(node.text, "//");
                    List<string> listItalicTextToReplace = new List<string>();
                    List<string> listItalicTextReplacement = new List<string>();

                    if (listItalicTagIndexes != null)
                    {
                        for (var i = 0; i < listItalicTagIndexes.Count; i+=2)
                        {
                            string toReplace = HandleString.getBetweenIndexes(
                                node.text, 
                                listItalicTagIndexes[i], 
                                listItalicTagIndexes[i+1] + 2
                                );

                            string replacement = "<i>" + HandleString.getBetweenIndexes(
                                node.text, 
                                listItalicTagIndexes[i] + 2 + (i/2 * 3),
                                listItalicTagIndexes[i + 1] + (i/2 * 3)
                                ) + "</i>";

                            Debug.Log(replacement);

                            listItalicTextToReplace.Add(toReplace);
                            listItalicTextReplacement.Add(replacement);
                        }
                    }

                    for (var i = 0; i < listItalicTextReplacement.Count; i++)
                    {
                        node.text = node.text.Replace(listItalicTextToReplace[i], listItalicTextReplacement[i]);
                    }

                    //PARCOURS TOUS LES STRING ENTRE [[ ]] POUR MODIFIER LE FORMAT ET METTRE DE LA COULEUR + UN LIEN
                    List<int> listIndexStart = HandleString.AllIndexesOf(node.text, "[[");
                    List<int> listIndexEnd = HandleString.AllIndexesOf(node.text, "]]");

                    List<string> listRawTextClickable = new List<string>();
                    List<string> listLinkClickable = new List<string>();
                    List<string> listTextClickable = new List<string>();

                    if (listIndexStart != null)
                    {
                        for (var i = 0; i < listIndexStart.Count; i++)
                        {
                            string rawOccurence = HandleString.getBetweenIndexes(node.text, listIndexStart[i], listIndexEnd[i] + 2);
                            string linkOccurence = HandleString.getBetweenIndexes(node.text, listIndexStart[i] + 2, listIndexEnd[i]);
                            string occurence = HandleString.getBetweenIndexes(node.text, listIndexStart[i] + 2, listIndexEnd[i]);

                            if(occurence.IndexOf("->") != -1)
                            {
                                linkOccurence = HandleString.getAfter(occurence, "->");
                                occurence = occurence.Substring(0, occurence.IndexOf("->"));
                            }

                            listRawTextClickable.Add(rawOccurence);
                            listLinkClickable.Add(linkOccurence);
                            listTextClickable.Add(occurence);
                        }
                    }

                    for (var i = 0; i < listRawTextClickable.Count; i++)
                    {
                        node.text = node.text.Replace(
                            listRawTextClickable[i],
                            "<link=" + listLinkClickable[i] + "><b><color=blue>" + listTextClickable[i] + "</color></b></link>"
                            );
                    }

                    //PARCOURT TOUS LES LIENS
                    /*foreach (Link link in node.links)
                    {
                        if (link != null)
                        {
                            Debug.Log(link.name);
                        }
                    }*/
                }
            }
        }
        else
        {
            Destroy(this);
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
}