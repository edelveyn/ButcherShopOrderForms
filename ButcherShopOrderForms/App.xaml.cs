using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ButcherShopOrderForms
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;           // Mutex для обеспечения открытия только одного окна приложения
        string settingPath = "SettingApp.txt";  // Имя файла сохранения параметров приложения

        public App()
        {
            mutex = new System.Threading.Mutex(true, "ButcherShopOrder", out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Окно приложения уже открыто");
                this.Shutdown();
            }

            // Ициализация параметров приложения
            this.Properties["KeyShop"] = string.Empty;                          // код магазина
            this.Properties["DefaultBlankTemplatePath"] = string.Empty;         // путь файла шаблона по-умолчанию
            this.Properties["DefaultRetutnBlankTemplatePath"] = string.Empty;   // путь файла шаблона возврата по-умолчанию
            this.Properties["ServiceSaveBlankPath"] = string.Empty;             // служебный путь сохранения файлов бланков для загрузчика 1С
            this.Properties["UserSaveBlankPath"] = string.Empty;                // пользовательский путь сохранения файла бланков
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(settingPath, FileMode.Open, storage))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                            this.Properties[keyValue[0]] = keyValue[1];
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                // Handle when file is not found in isolated storage:
                // * When the first application session
                // * When file has been deleted
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(settingPath, FileMode.Create, storage))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    foreach (string key in this.Properties.Keys)
                    {
                        writer.WriteLine("{0},{1}", key, this.Properties[key]);
                    }
                }
            }
        }
    }
}
