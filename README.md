# Simple_Unity_Translation
Simple scripts to translate a game strings into multiple languages in unity.
How to use:
# BoliVene - Runtime Localization for Unity
Lightweight **XML-driven** localisation that you can drop into any Unity project in minutes.  
Perfect for small teams and solo devs who need translations **without** the usual bloat.

---

## ✨  Features
| ✔ | What it does |
|---|--------------|
| **Zero dependencies** | Plain C# + LINQ-to-XML (works from 5.x to 2025-LTS). |
| **Runtime language switch** | One call updates the whole UI. |
| **Event-driven** | UI refreshes only when needed—no polling. |
| **Extensible** | Add any language tag in your XML—no code changes. |
| **MIT-licensed** | Free for commercial & open-source projects. |

---

## 🚀  Quick Start

1. **Clone / copy** the two scripts into your project  


2. **Add** `LocalizationManager` to a GameObject in your first scene  
(or create a prefab and mark **Don’t Destroy On Load**).

3. **Create** an XML file (e.g. `StreamingAssets/loc.xml`):

<Localization>
  <Entry>
    <Key>welcome</Key>
    <English>Hello!</English>
    <Spanish>¡Hola!</Spanish>
  </Entry>
  <Entry>
    <Key>quit</Key>
    <English>Quit</English>
    <Spanish>Salir</Spanish>
  </Entry>
</Localization>

4. Load & switch language at runtime:


var path = System.IO.Path.Combine(Application.streamingAssetsPath, "loc.xml");
var xml  = System.IO.File.ReadAllText(path);

LocalizationManager.Instance.LoadXml(xml);      
LocalizationManager.Instance.SetLanguage("Spanish");

5. Bind any UnityEngine.UI.Text:
-- In the Inspector → add LocalizedText & set key = "welcome"



🛠️ API Reference
LocalizationManager
Member	Description
LoadXml(string xml)	Parse XML and cache all translations.
SetLanguage(string code)	Change language (code = tag name, e.g. "Spanish").
string CurrentLanguage	Returns the active language.
event Action LanguageChanged	Subscribe to refresh custom widgets.
string Get(string key, string fallback="")	Manual lookup (UI toolkit, TMP, etc.).

LocalizedText
Simple UI helper:
Attach to any Text component → set Localization Key → done.

Public	Description
void SetKey(string key)	Change key at runtime and refresh immediately.

➕ Adding More Languages
Pick a tag name (e.g. <German>).

Add that tag under each <Entry> in your XML.

Call SetLanguage("German").
That’s it—no source changes needed.

❓ FAQ
Q: Can I use TextMeshPro?
A: Yes—replace UnityEngine.UI.Text with TMPro.TMP_Text in LocalizedText.cs.

Q: Can I pull XML from the web?
A: Absolutely. Download it (UnityWebRequest, Addressables, etc.) then feed the string into LoadXml.

Q: What about plurals, gender, formatting?
A: Store formatted strings ({0} coins) in XML and use string.Format() after retrieval. The manager stays lean on purpose.
