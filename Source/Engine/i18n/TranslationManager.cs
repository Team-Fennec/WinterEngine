using ValveKeyValue;
using WinterEngine.Resource;

namespace WinterEngine.Localization;

public static class TranslationManager
{
    // Logger
    private static readonly ILog log = LogManager.GetLogger("TranslationManager");

    // default to english
    public static string CurrentLang = "english";
    static Dictionary<string, Language> languageData = new Dictionary<string, Language>();

    // adds a translation file using the resource manager
    public static void AddTranslation(string fileName)
    {
        Stream translationFile = ResourceManager.GetData(Path.Combine("resource", fileName));

        // read hjson data now
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject trnsFileData = kv.Deserialize(translationFile);

        Dictionary<string, string> tokens = new Dictionary<string, string>();
        foreach (KVObject tokenObject in (IEnumerable<KVObject>)trnsFileData["tokens"])
        {
            tokens.TryAdd(tokenObject.Name, tokenObject.Value.ToString());
        }

        if (languageData.ContainsKey(trnsFileData["Language"].ToString()))
        {
            languageData.TryGetValue(CurrentLang, out var lang);
            
            foreach (var token in lang.tokens.Keys)
            {
                if (!tokens.ContainsKey(token))
                {
                    tokens.Add(token, lang.tokens[token]);
                }
            }

            languageData[trnsFileData["Language"].ToString()].tokens = tokens;
        }
        else
        {
            languageData.Add(trnsFileData["Language"].ToString(), new Language(trnsFileData["Language"].ToString(), tokens));
        }
        translationFile.Close();

        log.Info($"Added translation file {fileName}");
    }

    public static string GetTranslatedString(string token)
    {
        languageData.TryGetValue(CurrentLang, out var lang);

        if (lang != null && lang.tokens.ContainsKey(token))
        {
            lang.tokens.TryGetValue(token, out var res);
            return res;
        }
        else
        {
            log.Warn($"Unable to find translation for {token}");
            return token;
        }
    }

    public static string GetTranslatedString(string token, string[] args)
    {
        var res = GetTranslatedString(token);

        if (res == token)
        {
            return token; // no error log we already threw one earlier.
        }
        else
        {
            // we actually got something, try and format it.
            return string.Format(res, args);
        }
    }
}

// best used as a static using
// i.e. using static WinterEngine.Localization.StringTools;
public static class StringTools
{
    public static string TRS(string token) => TranslationManager.GetTranslatedString(token);
    public static string TRS(string token, string[] args) => TranslationManager.GetTranslatedString(token, args);
}
