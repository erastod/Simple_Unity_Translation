// LocalizedText.cs – MIT License
// ------------------------------
// Attach to any UI.Text component and assign a key in the Inspector.
// When the language changes, the text updates automatically.
//
// Optional: call SetKey("newKey") at runtime to swap the translation.
//
// © 2025 Your Name

using UnityEngine;
using UnityEngine.UI;

namespace BoliVene.Localization
{
    /// <summary>
    /// Binds a UI.Text to a localisation entry.
    /// </summary>
    [AddComponentMenu("Localization/Localized Text")]
    [RequireComponent(typeof(Text))]
    [DisallowMultipleComponent]
    public sealed class LocalizedText : MonoBehaviour
    {
        [Tooltip("Key as it appears in the <Key> element of your XML file.")]
        [SerializeField] private string localizationKey = "";

        private Text   _text;
        private string _originalContent;

        // ------------------------------------------------------------------
        // Unity lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            _text            = GetComponent<Text>();
            _originalContent = _text.text;
        }

        private void OnEnable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.LanguageChanged += Refresh;
                Refresh(); // make sure we start in the right language
            }
        }

        private void OnDisable()
        {
            if (LocalizationManager.Instance != null)
                LocalizationManager.Instance.LanguageChanged -= Refresh;
        }

        // ------------------------------------------------------------------
        // Public helpers
        // ------------------------------------------------------------------

        /// <summary>Change the key at runtime and refresh.</summary>
        public void SetKey(string key)
        {
            localizationKey = key;
            Refresh();
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private void Refresh()
        {
            if (LocalizationManager.Instance == null || string.IsNullOrEmpty(localizationKey))
                return;

            _text.text = LocalizationManager.Instance.Get(localizationKey, _originalContent);
        }
    }
}
