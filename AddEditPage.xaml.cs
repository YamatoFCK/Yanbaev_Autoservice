using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yanbaev_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Service _currentService = new Service();

        public AddEditPage(Service SelectedServise)
        {
            InitializeComponent();

            if (SelectedServise != null)
                _currentService = SelectedServise;
                           
            DataContext = _currentService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Title))
                errors.AppendLine("Укажите название услуги");

            if (_currentService.Duration <= 0 || _currentService.Duration > 240)
                errors.AppendLine("Длительность должна быть от 1 до 240 минут");

            if (_currentService.DiscountInt < 0 || _currentService.DiscountInt > 100)
                errors.AppendLine("Скидка должна быть от 0 до 100");

            if (_currentService.Duration <= 0)
                errors.AppendLine("Укажите длительность услуги");

            if (!string.IsNullOrWhiteSpace(_currentService.Title))
            {
                string title = _currentService.Title.Trim();

                bool exists = Yanbaev_AvtoservisEntities.GetContext().Service.ToList()
                    .Any(p => p.ID != _currentService.ID &&
                              p.Title != null &&
                              string.Equals(p.Title.Trim(), title, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    errors.AppendLine("Уже существует такая услуга");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentService.ID == 0)
                Yanbaev_AvtoservisEntities.GetContext().Service.Add(_currentService);

            try
            {
                Yanbaev_AvtoservisEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
