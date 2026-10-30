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
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        private ClientService _currentClientService = new ClientService();

        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
                _currentService = SelectedService;

            DataContext = _currentService;
            ComboClient.ItemsSource = Yanbaev_AvtoservisEntities.GetContext().Client.ToList();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.SelectedDate == null)
                errors.AppendLine("Укажите дату услуги");

            if (string.IsNullOrWhiteSpace(TBStart.Text))
                errors.AppendLine("Укажите время начала услуги");

            DateTime time;
            if (!DateTime.TryParseExact(TBStart.Text, "HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out time))
                errors.AppendLine("Введите время в формате ЧЧ:ММ");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID =
                (ComboClient.SelectedItem as Client).ID;

            _currentClientService.ServiceID = _currentService.ID;

            _currentClientService.StartTime =
                StartDate.SelectedDate.Value.Date.Add(time.TimeOfDay);

            if (_currentClientService.ID == 0)
                Yanbaev_AvtoservisEntities.GetContext().ClientService.Add(_currentClientService);

            try
            {
                Yanbaev_AvtoservisEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBEnd == null)
                return;

            DateTime startTime;

            if (DateTime.TryParseExact(TBStart.Text, "HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out startTime))
            {
                TBEnd.Text = startTime.AddMinutes(_currentService.Duration).ToString("HH:mm");
            }
            else
            {
                TBEnd.Text = "";
            }
        }

        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c =>
                (c >= '0' && c <= '9') || c == ':');
        }

        private void TBStart_Pasting(
            object sender,
            DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            string text =
                e.DataObject.GetData(DataFormats.UnicodeText) as string;

            if (string.IsNullOrEmpty(text) ||
                text.Any(c => (c < '0' || c > '9') && c != ':'))
            {
                e.CancelCommand();
            }
        }
    }
}
