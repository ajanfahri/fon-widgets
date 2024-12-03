using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace WidgetFons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FetchFundData(new List<string> { "GBV", "YZC", "DDA" });
            // Verileri her 5 dakikada bir güncelle
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(5);
            timer.Tick += (sender, args) => FetchFundData(new List<string> { "GBV", "YZC", "DDA" });
            timer.Start();
        }

        private async void FetchFundData(List<string> fundCodes)
            {
            try { 
                using (HttpClient client = new HttpClient()) 
                { 
                    string url = $"http://localhost:3000/api/funds?codes={string.Join(",", fundCodes)}"; 
                    HttpResponseMessage response = await client.GetAsync(url); 
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync(); 
                    var funds = JsonConvert.DeserializeObject<List<Fund>>(responseBody); 
                    FundListView.ItemsSource = funds; 
                } 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show("Veri çekme hatası: " + ex.Message); 
            }
        }


            private string ExtractFundKod(string html)
            {
                // HTML parsing logic to extract fund name
                //return "Örnek Fon Adı";
                var doc = new HtmlAgilityPack.HtmlDocument(); doc.LoadHtml(html);
                var fundNameNode = doc.DocumentNode.SelectSingleNode("//*[@id='MainContent_FormViewMainIndicators_LabelFund']");
                return fundNameNode?.InnerText.Trim() ?? "Fon Adı Bulunamadı";
            }
            private string ExtractFundName(string html)
            {
                // HTML parsing logic to extract fund name
                //return "Örnek Fon Adı";
                var doc = new HtmlAgilityPack.HtmlDocument(); doc.LoadHtml(html); 
            var fundNameNode = doc.DocumentNode.SelectSingleNode("//*[@id='MainContent_FormViewMainIndicators_LabelFund']");
            return fundNameNode?.InnerText.Trim() ?? "Fon Adı Bulunamadı";
            }

            private string ExtractLastPrice(string html)
            {
            // HTML parsing logic to extract last price
            //return "18,018008 TL";
            var doc = new HtmlAgilityPack.HtmlDocument(); 
            doc.LoadHtml(html); var lastPriceNode = doc.DocumentNode.SelectSingleNode("//*[@id='MainContent_PanelInfo']/div[1]/ul[1]/li[1]/span"); 
            return lastPriceNode?.InnerText.Trim() ?? "Fiyat Bulunamadı";
            }
    

            private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) 
            { 
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left) this.DragMove(); 
            }

        
    }
    public class Fund { 
        public string FonKodu { get; set; } 
        public string FonAdi { get; set; } 
        public string SonFiyat { get; set; } 
    }
}