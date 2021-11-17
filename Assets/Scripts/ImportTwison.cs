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