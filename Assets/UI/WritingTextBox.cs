using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WritingTextBox : MonoBehaviour
{
        [SerializeField]
        private TMPro.TextMeshProUGUI textBox;

        private string text;

        //[SerializeField]
        private float textPromptSecondsPerCharacter = 0.01f;

        public UnityEvent onTextFinishedWritingEvent;
        public UnityEvent onCloseTextBox;

        private IEnumerator animateTextCoroutine = null; // If coroutine is running, this will not be null.

        void Awake()
        {
            text = textBox.text;
        }
        void Close()
        {
            gameObject.SetActive(false);
            onCloseTextBox.Invoke();
        }

        void OnEnable()
        {
            animateTextCoroutine = AnimateTextCoroutine(text, textPromptSecondsPerCharacter);
            StartCoroutine(animateTextCoroutine);
        }
        IEnumerator AnimateTextCoroutine(string message, float secondsPerCharacter = 0.1f)
        {
        //Set text to blank
        textBox.text = "";

            //then over time, add letters until complete
            for (int currentChar = 0; currentChar < message.Length; currentChar++)
            {
                textBox.text += message[currentChar];
                yield return new WaitForSeconds(secondsPerCharacter);
            }
            animateTextCoroutine = null;
            onTextFinishedWritingEvent.Invoke();
        }
}
