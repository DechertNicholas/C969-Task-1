using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1
{
    public class Translator
    {
        private Dictionary<string, string> EnglishTable = new Dictionary<string, string>();
        private Dictionary<string, string> FrenchTable = new Dictionary<string, string>();
        public enum LanguageCode
        {
            EN,
            FR
        }

        public Translator()
        {
            EnglishTable.Add("LoginTitle", "Login");
            FrenchTable.Add("LoginTitle", "Connexion");

            EnglishTable.Add("LoginUsername", "Username");
            FrenchTable.Add("LoginUsername", "Nom d'utilisateur");

            EnglishTable.Add("LoginPassword", "Password");
            FrenchTable.Add("LoginPassword", "Mot de passe");

            EnglishTable.Add("LoginLanguage", "Language");
            FrenchTable.Add("LoginLanguage", "Langue");

            EnglishTable.Add("LoginButton", "Login");
            FrenchTable.Add("LoginButton", "Connexion");

            EnglishTable.Add("UnknownUserTitle", "Unknown User");
            FrenchTable.Add("UnknownUserTitle", "Utilisateur inconnu");

            EnglishTable.Add("UnknownUser", "Unable to find a matching username and password");
            FrenchTable.Add("UnknownUser", "Impossible de trouver un nom d'utilisateur et un mot de passe correspondants");
        }

        public string Translate(LanguageCode lang, string fieldName )
        {
            switch (lang)
            {
                case LanguageCode.EN:
                    {
                        return EnglishTable[fieldName];
                    }
                case LanguageCode.FR:
                    {
                        return FrenchTable[fieldName];
                    }
                default:
                    {
                        throw new ArgumentException($"Language code is not valid: {lang}");
                    }
            }
        }
    }
}
