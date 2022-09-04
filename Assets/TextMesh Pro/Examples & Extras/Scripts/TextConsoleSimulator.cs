using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    public class TextConsoleSimulator : MonoBehaviour
    {

        private int m_nbAdditionalCharactersToShow;
        private int m_lastDotIndex;


        private TMP_Text m_TextComponent;
        private bool hasTextChanged;

        void Awake()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
        }


        void Start()
        {
            m_lastDotIndex = 0;
            m_nbAdditionalCharactersToShow = GameManager.GetInstance().TypingCharactersToShowPerFrame;

            GameManager.GetInstance().onChangeCharactersDisplaySpeed += SetNbAdditionalCharactersToShow;
            StartCoroutine(RevealCharacters(m_TextComponent));
        }

        void SetNbAdditionalCharactersToShow(int value)
        {
            m_nbAdditionalCharactersToShow = value;
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

                textComponent.maxVisibleCharacters = visibleCount > totalVisibleCharacters ? totalVisibleCharacters : visibleCount; // How many characters should TextMeshPro display?

                if (StopSentenceOrParagraph(textComponent))
                {
                    m_lastDotIndex = textComponent.maxVisibleCharacters;
                    StartCoroutine(GameManager.GetInstance().StopTextDisplayForSeconds(.5f));
                }

                visibleCount += SetMaxVisibleCharactersToDisplay(textComponent, totalVisibleCharacters);

                yield return null;
            }
        }


        int SetMaxVisibleCharactersToDisplay(TMP_Text _textComponent, int _totalVisibleCharacters)
        {
            for (int i = 0; i < m_nbAdditionalCharactersToShow; i++)
            {
                int possibleIndex = _textComponent.maxVisibleCharacters + i > 0 && _textComponent.maxVisibleCharacters <= _totalVisibleCharacters ? _textComponent.maxVisibleCharacters + i : 0;

                bool lastVisibleCharacterIsDot = _textComponent.textInfo.characterInfo[possibleIndex].character == '.';
                
                if (lastVisibleCharacterIsDot)
                {
                    return i + 1;
                }
            }

            return m_nbAdditionalCharactersToShow;
        }


        /// <summary>
        /// @TODO : Make stop sentence for any speed depending on m_nbAdditionalCharactersToShow
        /// </summary>
        /// <param name="textComponent"></param>
        /// <returns></returns>
        private bool StopSentenceOrParagraph(TMP_Text textComponent)
        {

            int currentIndex = textComponent.maxVisibleCharacters - 1 > 0 ? textComponent.maxVisibleCharacters - 1 : 0;
            bool lastVisibleCharacterIsDot = textComponent.textInfo.characterInfo[currentIndex].character == '.' && m_lastDotIndex != textComponent.maxVisibleCharacters;
            return lastVisibleCharacterIsDot;
        }
    }
}