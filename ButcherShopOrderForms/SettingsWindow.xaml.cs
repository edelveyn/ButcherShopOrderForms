using System;
using System.Windows;
using System.Windows.Forms;

namespace ButcherShopOrderForms
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public delegate void GetSettingResultHandler(object sender, EventArgs e);
        public event GetSettingResultHandler GetSettingResult;

        #region Properties

        public string KeyShop { get; set; }
        public string TemplateFilePath { get; set; }
        public string TemplateReturnFilePath { get; set; }
        public string ServiceSavePath { get; set; }
        public string UserSavePath { get; set; }

        #endregion

        public SettingsWindow()
        {
            InitializeComponent();

            this.Loaded += SettingsWindow_Loaded;
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            keyShopTextBox.Text = KeyShop;
            blankTemplatePath.Text = TemplateFilePath;
            returnBlankTemplatePath.Text = TemplateReturnFilePath;
            serviceBlankSavePath.Text = ServiceSavePath;
            userBlankSavePath.Text = UserSavePath;
        }

        private void saveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            KeyShop = keyShopTextBox.Text;
            TemplateFilePath = blankTemplatePath.Text;
            TemplateReturnFilePath = returnBlankTemplatePath.Text;
            ServiceSavePath = serviceBlankSavePath.Text;
            UserSavePath = userBlankSavePath.Text;

            GetSettingResultHandler getSetting = this.GetSettingResult;
            getSetting?.Invoke(this, EventArgs.Empty);

            this.Close();
        }

        private void blankPathButton_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Открытие файла шаблона бланков";
                dialog.Filter = "Xml files (*.xml)|*.xml";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    blankTemplatePath.Text = dialog.FileName;
                }
            }
        }

        private void returnBlankPathButton_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Открытие файла шаблона бланков возвратов";
                dialog.Filter = "Xml files (*.xml)|*.xml";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    returnBlankTemplatePath.Text = dialog.FileName;
                }
            }
        }

        private void serviceBlankPathButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Служебный путь сохранения бланка заявки";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    serviceBlankSavePath.Text = dialog.SelectedPath;
                }
            }
        }

        private void userBlankPathButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Пользовательский путь сохранения бланка заявки";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    userBlankSavePath.Text = dialog.SelectedPath;
                }
            }
        }


    }
}
