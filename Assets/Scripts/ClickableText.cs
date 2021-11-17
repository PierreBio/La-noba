using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableText : MonoBehaviour, IPointerClickHandler
{
    public Camera m_Camera;

    private Node[] storyNodes;

    private Node currentNode;

    private IDictionary<string, string> currentVariables = new Dictionary<string, string>();

    public void Awake()
    {
        storyNodes = ImportTwison._instance.storyNodes.passages;

        currentNode = ImportTwison._instance.storyNodes.passages[17];

        currentVariables = ImportTwison._instance.variableDictionnary;

        this.displayCurrentNode();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var text = gameObject.GetComponent<TextMeshProUGUI>();

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, m_Camera);

            if (linkIndex > -1)
            {
                var linkInfo = text.textInfo.linkInfo[linkIndex];
                var linkId = linkInfo.GetLinkText();
                Debug.Log(linkId);
                linkId = linkInfo.GetLinkID();
                Debug.Log(linkId);

                changeCurrentNode(linkId);
            }
        }
    }

    private void changeCurrentNode(string newNodeName)
    {
        int indexNewNodePid = 0;

        foreach(Node node in storyNodes)
        {
            if(node.name == newNodeName)
            {
                indexNewNodePid = node.pid - 1;
                break;
            }
        }

        Node existingNode = ImportTwison._instance.storyNodes.passages[indexNewNodePid];
        currentNode = DeepCopy.DeepCopyNode(existingNode);
        Debug.Log(currentNode.pid);
        displayCurrentNode();
    }

    private void displayCurrentNode()
    {
        //AVANT LE DISPLAY ON DOIT DISSIMULER CERTAINES VALEURS (LES DECLARATIONS DE CERTAINES VARIABLES) 

        //DURANT LE CHANGEMENT DE NOEUD ON VERIFIE SI ON DOIT CHANGER LA VALEUR DE CERTAINES VARIABLES 

        //NOTAMMENT POUR EMPÊCHER OU PERMETTRE L'AFFICHAGE DE CERTAINS EMBRANCHEMENTS

        List<int> listIndexVariableDefinitions = HandleString.AllIndexesOf(currentNode.text, "(set: $");
        List<int> listEndParenthesis = HandleString.AllIndexesOf(currentNode.text, ")");
        List<string> listContentVariable = new List<string>();

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

                string contentVariable = currentNode.text.Substring(listIndexVariableDefinitions[i], System.Math.Abs((indexEndParenthesis + 1) - listIndexVariableDefinitions[i]));

                string variableName = HandleString.getBetween(contentVariable, "(set: $", " to");


                string variableValue = HandleString.getBetween(contentVariable, "to \"", "\")");

                if (currentVariables.ContainsKey(variableName)) //ON ATTRIBUT LA NOUVELLE VALEUR A NOTRE VARIABLE
                {
                    currentVariables[variableName] = variableValue;
                }

                listContentVariable.Add(contentVariable);
            }
        }

        if (listContentVariable != null)
        {
            for (var i = 0; i < listContentVariable.Count; i++)
            {
                //ON ENLEVE L'AFFICHAGE DE LA DECLARATION / ASSIGNATION DES VARIABLES
                currentNode.text = currentNode.text.Replace(listContentVariable[i], "");
            }
        }
        Debug.Log(currentNode.text);

        //GET ALL IFS
        List<int> listIndexIfConditions = HandleString.AllIndexesOf(currentNode.text, "(if: $");
        List<int> listEndifParenthesis = HandleString.AllIndexesOf(currentNode.text, ")");
        List<int> listIndexElseConditions = HandleString.AllIndexesOf(currentNode.text, "(else:)");

        if (listIndexIfConditions != null)
        {
            for (var i = 0; i < listIndexIfConditions.Count; i++)
            {
                if(i < HandleString.AllIndexesOf(currentNode.text, "(if: $").Count)
                {
                    verifyCondition(listIndexIfConditions, listEndifParenthesis, listIndexElseConditions, i);
                }
            }
        }
        Debug.Log(currentNode.text);

        GetComponent<TMPro.TextMeshProUGUI>().text = currentNode.text;
    }

    private int verifyCondition(List<int> listIndexIfConditions, List<int>  listEndifParenthesis, List<int> listIndexElseConditions, int index, int loop = 0)
    {
        int indexEndifParenthesis = 0;

        for (var j = 0; j < listEndifParenthesis.Count; j++)
        {
            if (listEndifParenthesis[j] > listIndexIfConditions[index])
            {
                indexEndifParenthesis = listEndifParenthesis[j];
                break;
            }
        }

        string contentVariable = currentNode.text.Substring(listIndexIfConditions[index], System.Math.Abs((indexEndifParenthesis + 1) - listIndexIfConditions[index]));
        string variableName = HandleString.getBetween(contentVariable, "(if: $", " is");
        string variablePotentialValue = HandleString.getBetween(contentVariable, "is \"", "\")");

        string content = HandleString.getBetween(currentNode.text, contentVariable + "[", " ]");
        if (currentVariables.ContainsKey(variableName))
        {
            if (currentVariables[variableName] == variablePotentialValue) //ON CHECK SI LA CONDITION EST VERIFIEE ET SI OUI ON DISPLAY LE TEXTE SITUE DANS LE THEN
            {
                if (content.IndexOf("(if: $") != -1)
                {
                    verifyCondition(listIndexIfConditions, listEndifParenthesis, listIndexElseConditions, index + 1, loop + 1);
                    listIndexIfConditions.RemoveAt(index + 1);
                }

                //On réactualise les nouvelles positions
                contentVariable = currentNode.text.Substring(listIndexIfConditions[index], System.Math.Abs((indexEndifParenthesis + 1) - listIndexIfConditions[index]));
                variableName = HandleString.getBetween(contentVariable, "(if: $", " is");
                variablePotentialValue = HandleString.getBetween(contentVariable, "is \"", "\")");

                content = HandleString.getBetween(currentNode.text, contentVariable + "[", " ]");

                int indexForContentToDelete = listIndexIfConditions[index] + contentVariable.Length + 1 + content.Length + 1;
                currentNode.text = currentNode.text.Remove(indexForContentToDelete, 1);
                currentNode.text = currentNode.text.Replace(contentVariable + "[", "");

                return loop;
            }
            else
            {
                string finalContentToDelete = currentNode.text.Substring(listIndexIfConditions[index], contentVariable.Length + 1 + content.Length + 2);
                currentNode.text = currentNode.text.Replace(finalContentToDelete, "");

                while((currentNode.text.IndexOf("(if: $") > currentNode.text.IndexOf(" ]")) || (currentNode.text.IndexOf("(if: $") == -1 && currentNode.text.IndexOf(" ]") != -1))
                {
                    currentNode.text = HandleString.ReplaceFirst(currentNode.text, " ]", "");
                }

                return loop;
            }
        }

        return 0;
    }
}
