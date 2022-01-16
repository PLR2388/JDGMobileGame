﻿using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class OptionMenu : MonoBehaviour
    {
        private const string French = "French";
        private const string FrenchInFrench = "Français";
        private const string English = "English";

        [SerializeField] private Slider volumeSlider;

        [SerializeField] private Dropdown languageDropdown;

        // Start is called before the first frame update
        private void Start()
        {
            languageDropdown.ClearOptions();

            var englishData = new Dropdown.OptionData(English);
            var frenchData = new Dropdown.OptionData(FrenchInFrench);

            var firstCurrentLanguage = Lean.Localization.LeanLocalization.GetFirstCurrentLanguage();

            switch (firstCurrentLanguage)
            {
                case English:
                {
                    languageDropdown.options.Add(englishData);
                    languageDropdown.options.Add(frenchData);
                }
                    break;
                case French:
                {
                    languageDropdown.options.Add(frenchData);
                    languageDropdown.options.Add(englishData);
                }
                    break;
            }

            languageDropdown.SetValueWithoutNotify(0);

            languageDropdown.onValueChanged.AddListener(delegate { LanguageDropDownItemSelected(languageDropdown); });
            languageDropdown.RefreshShownValue();
        }

        private static void LanguageDropDownItemSelected(Dropdown dropdown)
        {
            var currentLanguage = dropdown.options[dropdown.value].text;

            currentLanguage = currentLanguage switch
            {
                English => English,
                FrenchInFrench => French,
                _ => currentLanguage
            };

            Lean.Localization.LeanLocalization.SetCurrentLanguageAll(currentLanguage);
        }
    }
}