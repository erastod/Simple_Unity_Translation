// LocalizationManager.cs – MIT License
// ------------------------------------
// Simple XML-driven, runtime-switchable localisation system for Unity.
//
// • Reads XML text you feed it (from StreamingAssets, a web request, Lua, etc.)
// • Persists the last language with PlayerPrefs.
// • Fires an event when the language changes so UI can refresh.
//
// XML layout:
//
// <Localization>
//   <Entry>
//     <Key>welcome</Key>
//     <English>Hello!</English>
//     <Spanish>¡Hola!</Spanish>
//     <!-- add any language tags you need -->
//   </Entry>
//   ...
// </Localization>
//
// Usage (minimal):
//
//   await Addressables.LoadAssetAsync<TextAsset>("Loc/en.xml");
//   LocalizationManager.Instance.LoadXml(textAsset.text);
//   var greeting = LocalizationManager.Instance["welcome"];
//
// ------------------------------------
// © 2025 Your Name – released under the MIT License
// ------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace IndieTools.Localization
{
    /// <summary>
    /// Lightweight, singleton-style localisation service.
    /// </summary>
    public sealed class LocalizationManager : MonoBehaviour
    {
        //--------------------------------------------------------------------
        // Singleton boilerplate
        //--------------------------------------------------------------------
        public static LocalizationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _currentLanguage = PlayerPrefs.GetString(PrefKey, DefaultLanguage);
        }

        //--------------------------------------------------------------------
        // Public API
        //--------------------------------------------------------------------

        /// <summary>Raised every time <see cref="CurrentLanguage"/> changes *after*
        /// translations have been loaded.</summary>
        public event Action LanguageChanged;

        /// <summary>Returns the currently selected language code (e.g. "English").</summary>
        public string CurrentLanguage => _currentLanguage;

        /// <summary>Read-only indexer for quick access: <code>Loc["key"]</code></summary>
        public string this[string key] => Get(key, fallback: key);

        /// <summary>
        /// Loads an XML string in the layout shown at the top of this file.
        /// Call once during start-up (or whenever you receive new localisation).
        /// </summary>
        public void LoadXml(string xml)
        {
            _entries.Clear();

            try
            {
                var doc = XDocument.Parse(xml);

                foreach (var e in doc.Root.Elements("Entry"))
                {
                    var key = e.Element("Key")?.Value;
                    if (string.IsNullOrEmpty(key)) continue;

                    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var langElem in e.Elements())
                    {
                        if (langElem.Name.LocalName == "Key") continue;
                        dict[langElem.Name.LocalName] = langElem.Value;
                    }

                    _entries[key] = dict;
                }

                _xmlLoaded = true;
                LanguageChanged?.Invoke();
                Debug.Log($"[Localization] Loaded {_entries.Count:N0} entries.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] Failed to parse XML: {ex.Message}");
                _xmlLoaded = false;
            }
        }

        /// <summary>
        /// Changes the language. Triggers <see cref="LanguageChanged"/> if XML has
        /// been loaded and the language actually changed.
        /// </summary>
        public void SetLanguage(string languageCode)
        {
            if (string.Equals(_currentLanguage, languageCode, StringComparison.OrdinalIgnoreCase))
                return;

            _currentLanguage = languageCode;
            PlayerPrefs.SetString(PrefKey, _currentLanguage);
            PlayerPrefs.Save();

            if (_xmlLoaded) LanguageChanged?.Invoke();
            Debug.Log($"[Localization] Switched language → {_currentLanguage}");
        }

        /// <summary>
        /// Looks up a translation; returns <paramref name="fallback"/> if not found.
        /// </summary>
        public string Get(string key, string fallback = "")
        {
            if (!_xmlLoaded) return fallback;

            if (_entries.TryGetValue(key, out var langs) &&
                langs.TryGetValue(_currentLanguage, out var value) &&
                !string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Final fallback: try "English" or first available language
            if (langs != null && langs.TryGetValue(DefaultLanguage, out var defaultValue) &&
                !string.IsNullOrEmpty(defaultValue))
            {
                return defaultValue;
            }

            return fallback;
        }

        //--------------------------------------------------------------------
        // Internal data
        //--------------------------------------------------------------------
        private readonly Dictionary<string, Dictionary<string, string>> _entries
            = new(StringComparer.OrdinalIgnoreCase);

        private string _currentLanguage;
        private bool   _xmlLoaded;

        private const string PrefKey        = "Loc_SelectedLanguage";
        private const string DefaultLanguage = "English";
    }
}
