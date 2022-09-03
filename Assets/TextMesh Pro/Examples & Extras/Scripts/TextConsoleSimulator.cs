using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    public class TextConsoleSimulator : MonoBehaviour
    {

        private int m_nbAdditionalCharactersToShow;

        private TMP_Text m_TextComponent;
        private bool hasTextChanged;

        void Awake()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
        }


        void Start()
        {
            m_nbAdditionalCharactersToShow = GameManager.GetInstance().TypingCharactersToShowPerFrame;

            GameManager.GetInstance().onChangeCharactersDisplaySpeed += SetNbAdditionalCharactersToShow;
            StartCoroutine(RevealCharacters(m_TextComponent));
        }

        void SetNbAdditionalCharactersToShow(int value)
        {
            m_nbAdditionalCharactersToShow = value;
            Debug.Log("Passed Here : " + value);
        }


        void OnEnable()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }

        void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        }

        private void OnDestroy()
        {
            GameManager.GetInstance().onChangeCharactersDisplaySpeed -= SetNbAdditionalCharactersToShow;
        }


        // Event received when the text object has changed.
        void ON_TEXT_CHANGED(Object obj)
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();

            hasTextChanged = true;
        }


        /// <summary>
        /// Method revealing the text one character at a time.
        /// </summary>
        /// <returns></returns>
        IEnumerator RevealCharacters(TMP_Text textComponent)
        {
            textComponent.ForceMeshUpdate();

            int totalVisibleCharacters = m_TextComponent.textInfo.characterCount; // Get # of Visible Character in text object
            int visibleCount = 0;

            while (true)
            {
                if (hasTextChanged)
                {
                    totalVisibleCharacters = m_TextComponent.textInfo.characterCount; // Update visible character count.
                    hasTextChanged = false; 
                }

                //if (visibleCount > totalVisibleCharacters)
                //{
                //    yield return new WaitForSeconds(1.0f);
                //    visibleCount = 0;
                //}

                textComponent.maxVisibleCharacters = visibleCount > totalVisibleCharacters ? totalVisibleCharacters : visibleCount; // How many characters should TextMeshPro display?

                visibleCount += m_nbAdditionalCharactersToShow;

                yield return null;
            }
        }
    }
}