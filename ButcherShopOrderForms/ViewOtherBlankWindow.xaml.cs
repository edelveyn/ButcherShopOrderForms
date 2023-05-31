using System.Collections.Generic;
using System.Windows;

using ButcherShopLibrary;

namespace ButcherShopOrderForms
{
    /// <summary>
    /// Логика взаимодействия для ViewOtherBlankWindow.xaml
    /// </summary>
    public partial class ViewOtherBlankWindow : Window
    {
        public ViewOtherBlankWindow(string path)
        {
            InitializeComponent();
            InitializeBlankData(path);
        }

        #region Methods

        private void InitializeBlankData(string blankPath)
        {
            BlankReader reader = new BlankReader();
            List<Product> productsBlank = reader.ParseBlank(blankPath);

            blankDataGrid.ItemsSource = productsBlank;
        }

        #endregion
    }
}
