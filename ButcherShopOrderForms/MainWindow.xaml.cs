using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ButcherShopLibrary;


namespace ButcherShopOrderForms
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        private const string validatePassword = "1921";

        private string keyShop = string.Empty;
        private string templateFilePath = string.Empty;
        private string templateReturnFilePath = string.Empty;
        private string serviceSavePath = string.Empty;
        private string userSavePath = string.Empty;

        private ObservableCollection<ProductCategory> categories;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // восстановление значений из параметров приложения
            keyShop
                = System.Windows.Application.Current.Properties["KeyShop"].ToString();
            templateFilePath
                = System.Windows.Application.Current.Properties["DefaultBlankTemplatePath"].ToString();
            templateReturnFilePath
                = System.Windows.Application.Current.Properties["DefaultRetutnBlankTemplatePath"].ToString();
            serviceSavePath
                = System.Windows.Application.Current.Properties["ServiceSaveBlankPath"].ToString();
            userSavePath
                = System.Windows.Application.Current.Properties["UserSaveBlankPath"].ToString();


            // инициализируем данные по пути шаблона
            InitializeDataBlank(false, templateFilePath);

            // ограничим выбор даты заказа, начиная с сегодняшнего дня
            deliveryDate.BlackoutDates.AddDatesInPast();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // сохранение данных полей в параметрах приложения для последующего сохранения
            System.Windows.Application.Current.Properties["KeyShop"] = keyShop;
            System.Windows.Application.Current.Properties["DefaultBlankTemplatePath"] = templateFilePath;
            System.Windows.Application.Current.Properties["DefaultRetutnBlankTemplatePath"] = templateReturnFilePath;
            System.Windows.Application.Current.Properties["ServiceSaveBlankPath"] = serviceSavePath;
            System.Windows.Application.Current.Properties["UserSaveBlankPath"] = userSavePath;
        }

        #region Events

        private void categotyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categotyComboBox.SelectedItem == null)
            {
                blankDataGrid.ItemsSource = null;
                return;
            }
            blankDataGrid.ItemsSource = (categotyComboBox.SelectedItem as ProductCategory).Products;
        }

        private void isReturnBlank_ChangedChecked(object sender, RoutedEventArgs e)
        {
            //priceDataGridColumn.Visibility = ((bool)isReturnBlank.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            //sumDataGridColumn.Visibility = ((bool)isReturnBlank.IsChecked) ? Visibility.Visible : Visibility.Collapsed;

            // для возвратного бланка дата всегда текущий день без возможности редактирования
            bool isReturn = (bool)isReturnBlank.IsChecked;

            deliveryDate.IsEnabled = !isReturn;
            if (isReturn)
                deliveryDate.SelectedDate = DateTime.Now;
            else
                deliveryDate.SelectedDate = null;


            // проверим существование файла по пути сохранения
            string checkedPath = GetCheckingFilePath();
            InitializeDataBlank((bool)isReturnBlank.IsChecked, checkedPath);
        }

        private void ShowViewOtherBlank_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.InitialDirectory = userSavePath;
                dialog.Title = "Выбор файла заявки";
                dialog.Filter = "Xml files (*.xml)|*.xml";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewOtherBlankWindow viewOther = new ViewOtherBlankWindow(dialog.FileName);
                    viewOther.ShowDialog();
                }
            }
        }

        private void SaveBlank_Click(object sender, RoutedEventArgs e)
        {
            if (!ThereIsDataToSave())
            {
                System.Windows.MessageBox.Show(
                    "Нет данных для сохранения",
                    "Ошибка сохранения",
                    MessageBoxButton.OK);
                return;
            }

            if (deliveryDate.SelectedDate == null)
            {
                System.Windows.MessageBox.Show(
                    "Не указана желаемая дата поступления заказа",
                    "Ошибка сохранения",
                    MessageBoxButton.OK);
                return;
            }

            SaveBlankData(categotyComboBox.SelectedItem as ProductCategory);
        }

        private void ShowSettingsWindow_Click(object sender, RoutedEventArgs e)
        {
            SettingsPasswordWindow passwordWindow = new SettingsPasswordWindow();
            passwordWindow.Owner = this;

            bool? result = passwordWindow.ShowDialog();
            if (result == true)
            {
                if (passwordWindow.Password == validatePassword)
                {
                    SettingsWindow settings = new SettingsWindow();
                    settings.Owner = this;
                    settings.GetSettingResult += Settings_GetSettingResult;

                    settings.KeyShop = keyShop;
                    settings.TemplateFilePath = templateFilePath;
                    settings.TemplateReturnFilePath = templateReturnFilePath;
                    settings.ServiceSavePath = serviceSavePath;
                    settings.UserSavePath = userSavePath;

                    settings.Show();
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        "Неверный пароль. \nДействие недоступно.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Methods

        private bool ThereIsDataToSave()
        {
            if (blankDataGrid.ItemsSource == null)
            {
                return false;
            }

            var items = from data in (blankDataGrid.ItemsSource as ObservableCollection<Product>)
                        where data.Count > 0
                        select data;

            return items.Count() > 0;
        }

        private string GetCheckingFilePath() =>
            (bool)isReturnBlank.IsChecked ? templateReturnFilePath : templateFilePath;

        private void Settings_GetSettingResult(object sender, EventArgs e)
        {
            var settings = sender as SettingsWindow;

            keyShop = settings.KeyShop;
            templateFilePath = settings.TemplateFilePath;
            templateReturnFilePath = settings.TemplateReturnFilePath;
            serviceSavePath = settings.ServiceSavePath;
            userSavePath = settings.UserSavePath;

            // после изменения параметров выполним реинициализацию, т.к. могли быть изменены пути бланков 
            string checkedPath = GetCheckingFilePath();
            InitializeDataBlank((bool)isReturnBlank.IsChecked, checkedPath);
        }

        private void InitializeDataBlank(bool IsReturn, string dataPath)
        {
            if (dataPath == string.Empty)
            {
                categotyComboBox.ItemsSource = null;
                return;
            }

            // проверим существование файла по указанному пути
            FileInfo fileInfo = new FileInfo(dataPath);
            if (!fileInfo.Exists)
            {
                System.Windows.MessageBox.Show(
                    $"Не удалось найти файла по пути сохранения: {dataPath}. \nДанные не были загружены.",
                    "Ошибка инициализации данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                if (IsReturn)
                {
                    templateReturnFilePath = string.Empty;
                    categotyComboBox.ItemsSource = null;
                }
                else
                {
                    templateFilePath = string.Empty;
                    categotyComboBox.ItemsSource = null;
                }

                return;
            }

            BlankReader reader = new BlankReader();
            reader.SettingFilePath = dataPath;

            categories = reader.ProductsOfBlank();
            categotyComboBox.ItemsSource = categories;
        }

        private void SaveBlankData(ProductCategory currentCategoty)
        {
            bool isReturn = (bool)isReturnBlank.IsChecked;

            string fullServiceSavePath = $@"{serviceSavePath}\{(isReturn ? "В" : "З")} - {keyShop} - {currentCategoty.Name} - {String.Format("{0:dd.MM.yyyy}", (DateTime)deliveryDate.SelectedDate)} - {Guid.NewGuid()}.xml";
            string fullUserServiceSavePath = $@"{userSavePath}\{(isReturn ? "В" : "З")} - {keyShop} - {currentCategoty.Name} - {String.Format("{0:dd.MM.yyyy}", (DateTime)deliveryDate.SelectedDate)} - {Guid.NewGuid()}.xml";

            var items = from data in (blankDataGrid.ItemsSource as ObservableCollection<Product>)
                        where data.Count > 0
                        select new Product { Id = data.Id, Name = data.Name, Count = data.Count };

            BlankBuilder builder = new BlankBuilder(keyShop, currentCategoty);
            bool isSave = builder.SaveDataBlankOnDesktop(
                fullServiceSavePath,
                fullUserServiceSavePath,
                items.ToList(),
                (DateTime)deliveryDate.SelectedDate,
                isReturn);

            if (isSave)
            {
                System.Windows.MessageBox.Show(
                    $"Данные были успешно сохранены в файл: {fullServiceSavePath}",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show(
                    "Не удалось сохранить данные в файл.",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
