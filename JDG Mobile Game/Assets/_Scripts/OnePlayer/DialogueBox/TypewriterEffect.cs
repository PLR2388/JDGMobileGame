using System.Collections;
using TMPro;
using UnityEngine;

namespace OnePlayer.DialogueBox
{
    /// <summary>
    /// Applies a typewriter effect to the given text over time.
    /// </summary>
    public class TypewriterEffect : MonoBehaviour
    {
        [Tooltip("Default typing speed")]
        [SerializeField] private float defaultTypewriterSpeed = 25f;
        private float currentTypewriterSpeed;
        
        /// <summary>
        /// Initializes the current typing speed to its default value.
        /// </summary>
        private void Start()
        {
            currentTypewriterSpeed = defaultTypewriterSpeed;
        }

        /// <summary>
        /// Adapts the typing speed based on desired duration and text length.
        /// </summary>
        /// <param name="duration">The desired duration for the text to be fully typed.</param>
        /// <param name="lengthText">The length of the text being typed.</param>
        public void AdaptSpeedToLength(float duration, int lengthText)
        {
            if (duration > 0 && lengthText > 0)
            {
                currentTypewriterSpeed = lengthText / duration;
            }
            else
            {
                currentTypewriterSpeed = defaultTypewriterSpeed;
            }
        }

        /// <summary>
        /// Begins the typewriter effect on the provided text and label.
        /// </summary>
        /// <param name="textToType">The text to apply the typewriter effect on.</param>
        /// <param name="textLabel">The TMP_Text component to display the typed text.</param>
        /// <returns>Coroutine to handle the typing effect over time.</returns>
        public Coroutine Run(string textToType, TMP_Text textLabel)
        {
            if (textLabel != null) return StartCoroutine(TypeText(textToType, textLabel));
            Debug.LogError("Text label is null!");
            return null;
        }

        /// <summary>
        /// Coroutine that types out the text character by character at the specified speed.
        /// </summary>
        /// <param name="textToType">The text to type out.</param>
        /// <param name="textLabel">The TMP_Text component to display the typed text.</param>
        /// <returns>An enumerator needed for the coroutine to operate.</returns>
        private IEnumerator TypeText(string textToType, TMP_Text textLabel)
        {
            textLabel.text = string.Empty;

            float t = 0;
            int charIndex = 0;

            while (charIndex < textToType.Length)
            {
                t += Time.deltaTime * currentTypewriterSpeed;
                charIndex = Mathf.FloorToInt(t);
                charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

                textLabel.text = textToType.Substring(0, charIndex);

                yield return null;
            }

            textLabel.text = textToType;
        }

    }
}
