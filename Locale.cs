//using System.Globalization;
//using System.Resources;

//public static class Locale
//{
//Старый код 
//    private static readonly ResourceManager _resourceManager = new ResourceManager("Tor_Manager_.Properties.Resources.Locale", typeof(Locale).Assembly);

//    public static void SetCulture(string culture)
//    {
//        CultureInfo cultureInfo = new CultureInfo(culture);
//        CultureInfo.CurrentUICulture = cultureInfo;

//    }

//    public static string GetString(string name)
//    {
//        return _resourceManager.GetString(name, CultureInfo.CurrentUICulture);
//    }
//}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Xml;

public static class Locale
{
    private static Dictionary<string, Dictionary<string, string>> _localizationData;

    static Locale()
    {
        _localizationData = new Dictionary<string, Dictionary<string, string>>();
        LoadResources(Tor_Manager_.Properties.Resources.Locale_en, "en");
        LoadResources(Tor_Manager_.Properties.Resources.Locale_ru, "ru");
    }

    private static void LoadResources(string resxContent, string culture)
    {
        var resourceDictionary = new Dictionary<string, string>();

        using (var stringReader = new StringReader(resxContent))
        using (var resxReader = new ResXResourceReader(stringReader))
        {
            foreach (DictionaryEntry entry in resxReader)
            {
                if (entry.Key is string key && entry.Value is string value)
                {
                    resourceDictionary[key] = value;
                }
            }
        }

        _localizationData[culture] = resourceDictionary;
    }

    public static void SetCulture(string culture)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(culture);
    }

    public static string GetString(string name)
    {
        var currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        if (_localizationData.ContainsKey(currentCulture) && _localizationData[currentCulture].ContainsKey(name))
        {
            return _localizationData[currentCulture][name];
        }
        return name; // Возвращаем ключ, если перевод не найден
    }
}
