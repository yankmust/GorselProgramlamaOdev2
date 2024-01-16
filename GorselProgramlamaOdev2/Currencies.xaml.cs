using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GorselProgramlamaOdev2
{
    [XmlRoot(ElementName = "Tarih_Date")]
    public class TcmbExchangeRates
    {
        [XmlElement(ElementName = "Currency")]
        public List<Currency> Currencies { get; set; }
    }

    [XmlRoot(ElementName = "Currency")]
    public class Currency
    {
        [XmlElement(ElementName = "Unit")]
        public string Unit { get; set; }

        [XmlElement(ElementName = "Isim")]
        public string Isim { get; set; }

        [XmlElement(ElementName = "CurrencyName")]
        public string CurrencyName { get; set; }

        [XmlElement(ElementName = "ForexBuying")]
        public string ForexBuying { get; set; }

        [XmlElement(ElementName = "ForexSelling")]
        public string ForexSelling { get; set; }

        [XmlAttribute(AttributeName = "Kod")]
        public string Kod { get; set; }
    }

    public partial class Currencies : ContentPage
    {
        public ObservableCollection<Currency> CurrenciesData { get; set; } = new ObservableCollection<Currency>();

        public Currencies()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadExchangeRates();
        }
        private async Task LoadExchangeRates()
        {
            string apiUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync(apiUrl);

                    XmlSerializer serializer = new XmlSerializer(typeof(TcmbExchangeRates));
                    using (var reader = new System.IO.StringReader(response))
                    {
                        TcmbExchangeRates tcmbRates = (TcmbExchangeRates)serializer.Deserialize(reader);
                        foreach (var currency in tcmbRates.Currencies)
                        {
                            CurrenciesData.Add(currency);
                        }
                       
                        collectionView.ItemsSource = CurrenciesData;
                    }
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
