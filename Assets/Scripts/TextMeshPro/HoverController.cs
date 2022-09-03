using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro.Examples
{
    public class HoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Components for HoverController")]
        private TMP_MeshInfo[] m_cachedMeshInfoVertexData;
        private Canvas m_Canvas;
        private Camera m_Camera;
        public Color32 m_ColorLink;
        public Color32 m_ColorHoveredLink;
        private TextMeshProUGUI m_TextMeshPro;

        [Header("Variables describing current state")]
        private bool m_isHoveringObject;
        private int m_selectedWord = -1;

        private void Awake()
        {
            m_Canvas = gameObject.GetComponentInParent<Canvas>();
            m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();

            // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
            if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                m_Camera = null;
            else
                m_Camera = m_Canvas.worldCamera;
        }


        private void OnEnable()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }

        private void OnDisable()
        {
            // UnSubscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        }


        private void ON_TEXT_CHANGED(Object obj)
        {
            if (obj == m_TextMeshPro)
            {
                // Update cached vertex data.
                m_cachedMeshInfoVertexData = m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
            }
        }

        private void LateUpdate()
        {
            if (m_isHoveringObject)
            {
                //Check if Mouse intersects any words and if so assign a random color to that word.
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);

                // Clear previous word selection.
                if (m_selectedWord != -1 && (linkIndex == -1 || linkIndex != m_selectedWord))
                {
                    SoundManager.GetInstance().Play("mouse_out_link", GameManager.GetInstance().gameObject);

                    TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[m_selectedWord];

                    // Iterate through each of the characters of the word.
                    for (int i = 0; i < linkInfo.linkTextLength; i++)
                    {
                        int characterIndex = linkInfo.linkTextfirstCharacterIndex + i;

                        TMP_CharacterInfo cInfo = m_TextMeshPro.textInfo.characterInfo[characterIndex];

                        if (!cInfo.isVisible) continue; // Skip invisible characters.

                        int meshIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;
                        int vertexIndex = cInfo.vertexIndex;

                        // Get a reference to the vertex color
                        Color32[] vertexColors = m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32;
                        Color32 c = m_ColorLink;

                        vertexColors[vertexIndex + 0] = c;
                        vertexColors[vertexIndex + 1] = c;
                        vertexColors[vertexIndex + 2] = c;
                        vertexColors[vertexIndex + 3] = c;
                    }
                    
                    // Update Geometry
                    m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

                    m_selectedWord = -1;
                }

                if (linkIndex != -1 && linkIndex != m_selectedWord && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    SoundManager.GetInstance().Play("mouse_over_link", GameManager.GetInstance().gameObject);

                    m_selectedWord = linkIndex;

                    TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];

                    // Iterate through each of the characters of the word.
                    for (int i = 0; i < linkInfo.linkTextLength; i++)
                    {
                        int characterIndex = linkInfo.linkTextfirstCharacterIndex + i;
                        TMP_CharacterInfo cInfo = m_TextMeshPro.textInfo.characterInfo[characterIndex];

                        if (!cInfo.isVisible) continue; // Skip invisible characters.

                        int meshIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;
                        int vertexIndex = cInfo.vertexIndex;

                        Color32[] vertexColors = m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32;
                        Color32 c = m_ColorHoveredLink;

                        vertexColors[vertexIndex + 0] = c;
                        vertexColors[vertexIndex + 1] = c;
                        vertexColors[vertexIndex + 2] = c;
                        vertexColors[vertexIndex + 3] = c;
                    }

                    // Update Geometry
                    m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                }
            }
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isHoveringObject = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isHoveringObject = false;
        }
    }
}
