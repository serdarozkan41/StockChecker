using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockTrackingGCard
{
    class Program
    {
        static string VatanBilgisayarBaseUrl = "https://www.vatanbilgisayar.com/";
        static string HepsiBuradaBaseUrl = "https://www.hepsiburada.com/";
        static string TrendyolBaseUrl = "https://www.trendyol.com/";
        static string MediaMarktBaseUrl = "https://www.mediamarkt.com.tr/";

        static List<Stock> stocks = new();

        static async Task Main(string[] args)
        {
            stocks = JsonConvert.DeserializeObject<List<Stock>>(File.ReadAllText("stockList.json"));

            int i = 0;

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                bool status = false;
                switch (stocks[i].Shop)
                {
                    case ShopType.VATAN_BILGISAYAR:
                        status = await CheckStockForVatanBilgisayar(stocks[i]);
                        break;
                    case ShopType.HEPSI_BURADA:
                        status = await CheckStockForHepsiBurada(stocks[i]);
                        break;
                    case ShopType.TRENDYOL:
                        status = await CheckStockForTrendyol(stocks[i]);
                        break;
                    case ShopType.MEDIAMARKT:
                        status = await CheckStockForMediaMarkt(stocks[i]);
                        break;
                }

                Console.Write($"{stocks[i].Shop} {stocks[i].Brand} {stocks[i].Model} --- ");

                if (status)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Stokta VAR");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Stokta YOK");
                }

                Console.ResetColor();
                i++;
                if (i == stocks.Count)
                {
                    i = 0;
                    await Task.Delay(5000);
                    Console.Clear();
                }
            }
        }



        static async Task<bool> CheckStockForVatanBilgisayar(Stock stock)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{VatanBilgisayarBaseUrl}{stock.StockUrl}";
                var responseMessage = await httpClient.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseHtml = await responseMessage.Content.ReadAsStringAsync();
                    if (responseHtml.Contains("STOĞA GELİNCE HABER VER") || responseHtml.Contains("TÜKENDİ"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        static async Task<bool> CheckStockForHepsiBurada(Stock stock)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{HepsiBuradaBaseUrl}{stock.StockUrl}";

                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

                httpClient.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

                var responseMessage = await httpClient.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseHtml = await responseMessage.Content.ReadAsStringAsync();
                    if (responseHtml.Contains("Bu ürün geçici olarak temin edilememektedir.") || responseHtml.Contains("Bu ürün şu an satılmamaktadır."))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    var responseHtml = await responseMessage.Content.ReadAsStringAsync();
                    return false;
                }
            }
        }

        static async Task<bool> CheckStockForTrendyol(Stock stock)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{TrendyolBaseUrl}{stock.StockUrl}";

                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

                httpClient.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

                var responseMessage = await httpClient.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseHtml = await responseMessage.Content.ReadAsStringAsync();
                    if (responseHtml.Contains("Tükendi"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        static async Task<bool> CheckStockForMediaMarkt(Stock stock)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{MediaMarktBaseUrl}{stock.StockUrl}";

                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

                httpClient.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

                var responseMessage = await httpClient.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseHtml = await responseMessage.Content.ReadAsStringAsync();
                    if (!responseHtml.Contains("Teslimat Seçeneklerini Gör")&&!responseHtml.Contains("Stoktan Gönderi"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}