using System;
using System.Text;

namespace WinterEngine.Localization;

internal record class Language {
    public string langIdent;
    public Dictionary<string, string> tokens;
    
    public Language(string ident, Dictionary<string, string> tkns) {
        langIdent = ident;
        tokens = tkns;
    }
}
