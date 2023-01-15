using System.Collections;
using TMPro;
using UnityEngine;

namespace OnePlayer.DialogueBox
{
    public class TypewriterEffect : MonoBehaviour
    {
        private float typewriterSpeed = 25f;

        public void AdaptSpeedToLength(float duration, int lengthText)
        {
            if (duration > 0 && lengthText > 0)
            {
                typewriterSpeed = lengthText / duration;
            }
            else
            {
                typewriterSpeed = 25f;
            }
        }

        public Coroutine Run(string textToType, TMP_Text textLabel)
        {
           return StartCoroutine(TypeText(textToType, textLabel));
        }

        private IEnumerator TypeText(string textToType, TMP_Text textLabel)
        {
            textLabel.text = string.Empty;

            float t = 0;
            int charIndex = 0;

            while (charIndex < textToType.Length)
            {
                t += Time.deltaTime * typewriterSpeed;
                charIndex = Mathf.FloorToInt(t);
                charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

                textLabel.text = textToType.Substring(0, charIndex);

                yield return null;
            }

            textLabel.text = textToType;
        }

    }
}
