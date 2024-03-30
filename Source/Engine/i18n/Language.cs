namespace WinterEngine.Localization;

internal class Language
{
    public string langIdent;
    public Dictionary<string, string> tokens;

    public Language(string ident, Dictionary<string, string> tkns)
    {
        langIdent = ident;
        tokens = tkns;
    }
}
