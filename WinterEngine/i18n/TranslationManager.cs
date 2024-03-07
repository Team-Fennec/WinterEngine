using Hjson;
using System;
using System.Text;

namespace WinterEngine.Localization;

public static class TranslationManager {
    // Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(TranslationManager));
    
    // default to english
    public string CurrentLang = "english";
    Dictionary<string, Language> languageData = new Dictionary<string, Language>();
    
    // adds a translation file using the resource manager
    public static void AddTranslation(string fileName) {
        StreamReader translationFile = ResourceManager.OpenResource(Path.Combine("resource", fileName));
        
        // read hjson data now
        JsonValue trnsFileData = HjsonValue.Load(translationFile.BaseStream);
        
        Dictionary<string, string> tokens = new Dictionary<string, string>();
        foreach (var tokenObject in trnsFileData.Qo("tokens")) {
            tokens.TryAdd(tokenObject.Key, tokenObject.Value.Qstr());
        }
        
        languageData.Add(trnsFileData.Qstr("language"), new Language(trnsFileData.Qstr("language"), tokens));
        translationFile.Close();
        
        log.Info($"Added translation file {fileName}");
    }
    
    public static string GetTranslatedString(string token) {
        languageData.TryGetValue(CurrentLang, out var lang);
        
        if (lang != null && lang.tokens.ContainsKey(token)) {
            lang.tokens.TryGetValue(token, out var res);
            return res;
        } else {
            log.Warn($"Unable to find translation for {token}");
            return token;
        }
    }
    
    public static string GetTranslatedString(string token, string[] args) {
        var res = GetTranslatedString(token);
        
        if (res == token) {
            return token; // no error log we already threw one earlier.
        } else {
            // we actually got something, try and format it.
            return string.Format(res, args);
        }
    }
}

// best used as a static using
// i.e. using static WinterEngine.Localization.StringTools;
public static class StringTools {
    public static string TRS(string token) => TranslationManager.GetTranslatedString(token);
    public static string TRS(string token, string[] args) => TranslationManager.GetTranslatedString(token, args);
}