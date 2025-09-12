using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
[System.Serializable]
public class Language
{
    public string languageName;
    public int languageId;
}
public class LanguageSelector : MonoBehaviour
{
    [SerializeField]
    private List<Language> languagesList = new List<Language>();
    private Queue<Language> languages = new Queue<Language>();
    public Language selectedLanguage;
    [SerializeField]
    TextMeshProUGUI selectedLanguageText;
    private bool active = false;

    private void Start()
    {
        foreach (Language temp in languagesList)
        {
            languages.Enqueue(temp);
        }
        if (languages.Count > 0)
        {
            selectedLanguage = languages.Peek();
            UpdateLanguageText();
        }
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(ID);
    }
    public void ChangeLocale(int localeID) {
        if (active == true) return;
        StartCoroutine(SetLocale(localeID));
    }
    IEnumerator SetLocale(int _localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        active = false;
    }

    public void NextLanguage()
    {
        if (languages.Count == 0) return;
        languages.Enqueue(languages.Dequeue());
        selectedLanguage = languages.Peek();
        UpdateLanguageText();
        ChangeLocale(selectedLanguage.languageId); 
    }

    private void UpdateLanguageText()
    {
        if (selectedLanguageText != null && selectedLanguage != null)
            selectedLanguageText.text = selectedLanguage.languageName;
    }
}