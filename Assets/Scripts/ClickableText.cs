using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        currentNode = ImportTwison._instance.storyNodes.passages[0];

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
                linkId = linkInfo.GetLinkID();

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
        displayCurrentNode();
    }

    private void displayCurrentNode()
    {
        initNode(currentNode);

        //PARCOURT TOUS LES IFS
        List<int> listIndexIfConditions = HandleString.AllIndexesOf(currentNode.text, "(if: $");
        List<int> listEndifParenthesis = HandleString.AllIndexesOf(currentNode.text, ")");
        List<int> listIndexElseConditions = HandleString.AllIndexesOf(currentNode.text, "(else:)");

        List<string> listElementsToDelete = new List<string>();

        int initalListIndexIfCount = listIndexIfConditions.Count;
        if (listIndexIfConditions != null)
        {
            for (var i = 0; i < listIndexIfConditions.Count; i++)
            {
                if (i < HandleString.AllIndexesOf(currentNode.text, "(if: $").Count)
                {
                    verifyCondition(listIndexIfConditions, listEndifParenthesis, listIndexElseConditions, listElementsToDelete, i);
                }

                if (i!= 0 && i == HandleString.AllIndexesOf(currentNode.text, "(if: $").Count)
                {
                    verifyCondition(listIndexIfConditions, listEndifParenthesis, listIndexElseConditions, listElementsToDelete, i -1);
                }
            }
        }

        for (var i = 0; i < listElementsToDelete.Count; i++)
        {
            currentNode.text = currentNode.text.Replace(listElementsToDelete[i], "");
        }

        currentNode.text = currentNode.text.Replace(" ]", "");

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

        triggerMusic();

        GetComponent<TMPro.TextMeshProUGUI>().text = currentNode.text;

        triggerTyping();
    }

    private int verifyCondition(List<int> listIndexIfConditions, List<int>  listEndifParenthesis, List<int> listIndexElseConditions, List<string> listElementsToDelete, int index, int loop = 0)
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

        string content = getContentOfIf(currentNode.text, HandleString.getBetween(currentNode.text, contentVariable + "[", " ]"));

        if (currentVariables.ContainsKey(variableName))
        {
            if (currentVariables[variableName] == variablePotentialValue) //ON CHECK SI LA CONDITION EST VERIFIEE ET SI OUI ON DISPLAY LE TEXTE SITUE DANS LE THEN
            {
                if (content.IndexOf("(if: $") != -1)
                {
                    verifyCondition(listIndexIfConditions, listEndifParenthesis, listIndexElseConditions, listElementsToDelete, index + 1, loop + 1);
                    listIndexIfConditions.RemoveAt(index + 1);
                }

                listElementsToDelete.Add(contentVariable + "[");

                return loop;
            }
            else
            {
                string finalContentToDelete = currentNode.text.Substring(listIndexIfConditions[index], contentVariable.Length + 1 + content.Length + 2);

                listElementsToDelete.Add(finalContentToDelete);

                return loop;
            }
        }

        return 0;
    }

    private string getContentOfIf(string text, string contentVariable, int occurences = 0)
    {
        string result = contentVariable;

        if (contentVariable.IndexOf("(if: $") != -1)
        {
            result = contentVariable + getContentOfIf(text, HandleString.getBetween(currentNode.text, contentVariable + " ]", " ]"), occurences + 1);
        }

        return result;
    }

    private void initNode(Node node)
    {
        //PARCOURT TOUS LES STRING ENTRE // POUR METTRE DE L'ITALIQUE
        List<int> listItalicTagIndexes = HandleString.AllIndexesOf(node.text, "//");
        List<string> listItalicTextToReplace = new List<string>();
        List<string> listItalicTextReplacement = new List<string>();

        if (listItalicTagIndexes != null)
        {
            for (var i = 0; i < listItalicTagIndexes.Count; i += 2)
            {
                string toReplace = HandleString.getBetweenIndexes(
                    node.text,
                    listItalicTagIndexes[i],
                    listItalicTagIndexes[i + 1] + 2
                    );

                string replacement = "<i>" + HandleString.getBetweenIndexes(
                    node.text,
                    listItalicTagIndexes[i] + 2 + (i / 2 * 3),
                    listItalicTagIndexes[i + 1] + (i / 2 * 3)
                    ) + "</i>";

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

                if (occurence.IndexOf("->") != -1)
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

        node.text = node.text.Replace("set:$", "set: $");
        node.text = node.text.Replace("(if:$", "(if: $");
        //node.text = Regex.Replace(node.text, @"\s+", " ");
        node.text = node.text.Replace("]", " ]");
    }

    private void triggerMusic()
    {
        if(currentNode.name == "Noah nous barre la route" || currentNode.name == "Noah arrive en aide providentielle")
        {
            SoundManager.GetInstance().Play("noah_theme", SoundManager.GetInstance().gameObject);
        }

        if (currentNode.name == "voix fantomatique")
        {
            SoundManager.GetInstance().Play("noah_revelations", SoundManager.GetInstance().gameObject);
        }
    }

    private void triggerTyping()
    {
        if(gameObject.GetComponent< TMPro.Examples.TextConsoleSimulator>())
        {
            Destroy(gameObject.GetComponent<TMPro.Examples.TextConsoleSimulator>());
        }

        gameObject.AddComponent<TMPro.Examples.TextConsoleSimulator>();
    }
}
