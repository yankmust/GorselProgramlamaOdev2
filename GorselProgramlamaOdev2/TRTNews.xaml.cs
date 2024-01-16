using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace GorselProgramlamaOdev2
{
    public partial class TRTNews : ContentPage
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly ObservableCollection<Kategori> categories = new ObservableCollection<Kategori>(Kategori.liste);
        private readonly ObservableCollection<NewsItem> newsItems = new ObservableCollection<NewsItem>();

        public TRTNews()
        {
            InitializeComponent();

            CategoryCollectionView.ItemsSource = categories;
            CategoryCollectionView.SelectedItem = categories[0];

            NewsCollectionView.ItemsSource = newsItems;

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadRssFeed(categories[0]);
        }

        private async void OnCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                Kategori selectedCategory = e.CurrentSelection[0] as Kategori;
                await LoadRssFeed(selectedCategory);
            }
        }

        private async void OnNewsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                NewsItem selectedNewsItem = e.CurrentSelection[0] as NewsItem;
                await NavigateToNewsDetailPage(selectedNewsItem);
            }
        }

        private async Task NavigateToNewsDetailPage(NewsItem newsItem)
        {
            await Navigation.PushAsync(new NewsDetailPage(newsItem));
        }

        static string rss2json => "https://api.rss2json.com/v1/api.json?rss_url";
        private async Task LoadRssFeed(Kategori category)
        {
            try
            {
                string url = $"{rss2json}={category.Link}";

                using HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsondata = await response.Content.ReadAsStringAsync();

                HaberRoot root = JsonSerializer.Deserialize<HaberRoot>(jsondata);

                newsItems.Clear();
                foreach (var item in root.items)
                {
                    Console.WriteLine($"Thumbnail URL: {item.enclosure?.link}");

                    newsItems.Add(new NewsItem
                    {
                        Thumbnail = item.enclosure?.link,
                        Title = item.title,
                        PubDate = item.pubDate,
                        Content = item.content,
                        Link = item.link
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public class NewsItem
        {
            public string Thumbnail { get; set; }
            public string Title { get; set; }
            public string PubDate { get; set; }
            public string Content { get; set; }
            public string Link { get; set; }
        }

        public class Kategori
        {
            public string Baslik { get; set; }
            public string Link { get; set; }

            public static List<Kategori> liste = new List<Kategori>()
            {
                new Kategori() { Baslik = "Economy", Link = "https://www.trthaber.com/ekonomi_articles.rss"},
                new Kategori() { Baslik = "Culture and Art", Link = "https://www.trthaber.com/kultur_sanat_articles.rss"},
                new Kategori() { Baslik = "Science and Tech", Link = "https://www.trthaber.com/bilim_teknoloji_articles.rss"},
                new Kategori() { Baslik = "Special News", Link = "https://www.trthaber.com/ozel_haber_articles.rss"},
                new Kategori() { Baslik = "Sports", Link = "https://www.trthaber.com/spor_articles.rss"},
            };
        }

        public class Enclosure
        {
            public string link { get; set; }
            public string type { get; set; }
        }

        public class Feed
        {
            public string url { get; set; }
            public string title { get; set; }
            public string link { get; set; }
            public string author { get; set; }
            public string description { get; set; }
            public string image { get; set; }
        }

        public class Item
        {
            public string title { get; set; }
            public string pubDate { get; set; }
            public string link { get; set; }
            public string guid { get; set; }
            public string author { get; set; }
            public string thumbnail { get; set; }
            public string description { get; set; }
            public string content { get; set; }
            public Enclosure enclosure { get; set; }
            public List<object> categories { get; set; }
        }

        public class HaberRoot
        {
            public string status { get; set; }
            public Feed feed { get; set; }
            public List<Item> items { get; set; }
        }
    }

    public class NewsDetailPage : ContentPage
    {
        private TRTNews.NewsItem _newsItem;

        public NewsDetailPage(TRTNews.NewsItem newsItem)
        {
            _newsItem = newsItem;
            Title = newsItem.Title;

            var webView = new WebView
            {
                Source = new UrlWebViewSource { Url = newsItem.Link },
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var shareButton = new ToolbarItem
            {
                
                IconImageSource = "share.png", 
                Command = new Command(ShareClicked),
                Order = ToolbarItemOrder.Primary
            };

            ToolbarItems.Add(shareButton);

            Content = new StackLayout
            {
                Children =
                {
                    new Label { Text = newsItem.Title, FontSize = 24, FontAttributes = FontAttributes.Bold },
                    new Label { Text = newsItem.PubDate, FontSize = 16, TextColor = Color.FromRgb(128, 128, 128) },
                    webView
                }
            };
        }

        private async void ShareClicked()
        {
            await ShareUri(_newsItem.Link, Share.Default);
        }

        private async Task ShareUri(string uri, IShare share)
        {
            await share.RequestAsync(new ShareTextRequest
            {
                Uri = uri,
                Title = "Share News"
            });
        }
    }
}