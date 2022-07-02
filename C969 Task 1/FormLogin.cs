using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C969_Task_1
{
    public partial class FormLogin : Form
    {
        SQLHandler Handler = new SQLHandler();

        private Translator LocalTranslator = new Translator();
        private Translator.LanguageCode SelectedLanguage;
        private string[] AvailableLanguages = new string[] { "English", "Français" };
        private string LogPath = @".\logins.log"; // Next to the .exe

        public FormLogin()
        {
            InitializeComponent();
            
            switch (CultureInfo.CurrentUICulture.Name)
            {
                case "en-US":
                    comboBoxLanguage.SelectedValue = "English";
                    break;
                case "fr-FR":
                    comboBoxLanguage.SelectedValue = "Français";
                    break;
            }
            // Setting the datasource actually calls SelectedIndexChanged, which will translate the UI
            comboBoxLanguage.DataSource = AvailableLanguages;
        }

        private void TranslateUI(Translator.LanguageCode selectedLanguage)
        {
            this.Text = LocalTranslator.Translate(SelectedLanguage, "LoginTitle");
            labelUsername.Text = LocalTranslator.Translate(SelectedLanguage, "LoginUsername");
            labelPassword.Text = LocalTranslator.Translate(SelectedLanguage, "LoginPassword");
            labelLanguage.Text = LocalTranslator.Translate(SelectedLanguage, "LoginLanguage");
            buttonLogin.Text = LocalTranslator.Translate(SelectedLanguage, "LoginButton");
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedLanguage = comboBoxLanguage.SelectedValue switch
            {
                "English" => Translator.LanguageCode.EN,
                "Français" => Translator.LanguageCode.FR,
                _ => Translator.LanguageCode.EN
            };
            TranslateUI(SelectedLanguage);
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Handler.TestLogin(textBoxUsername.Text, textBoxPassword.Text, SelectedLanguage);
                var culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
                string[] loginRecord = { $"{textBoxUsername.Text} : {DateTime.UtcNow.ToString(culture)} UTC" };

                Console.WriteLine(CultureInfo.CurrentUICulture.Name);
                Console.WriteLine(DateTime.UtcNow.ToString(culture));

                File.AppendAllLines(LogPath, loginRecord);
                var scheduling = new FormScheduling(textBoxUsername.Text);
                this.Hide();
                scheduling.ShowDialog();
            }
            catch (UnknownUserException ex)
            {
                MessageBox.Show(ex.Message, LocalTranslator.Translate(SelectedLanguage, "UnknownUserTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
