using Broker_Projekt_Zaliczeniowy.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace Broker_Projekt_Zaliczeniowy.Services
{
    public class MarketDataService
    {
        private readonly ProjektBdContext _context;
        private readonly HttpClient _httpClient;
        private string ApiKey = "1TW4EX7C1KASCYRV";
        //private string ApiKey = "demo"; 

        public MarketDataService(ProjektBdContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<string?> UpdateMarketDataAsync(string ticker)
        {
            var asset = await _context.Assets
                .FirstOrDefaultAsync(a => a.Ticker == ticker);

            if (asset == null) return $"Błąd: Nie znaleziono tickera {ticker} w bazie.";

            string queryUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={ticker}&outputsize=compact&apikey={ApiKey}";

            try
            {
                var response = await _httpClient.GetAsync(queryUrl);
                var json = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    if (root.TryGetProperty("Error Message", out _))
                        return $"Błąd API: Nie znaleziono tickera {ticker} w AlphaVantage.";

                    if (root.TryGetProperty("Information", out _))
                        return "Limit zapytań API osiągnięty. Spróbuj później.";

                    if (root.TryGetProperty("Time Series (Daily)", out JsonElement timeSeries))
                    {
                        var newRecords = new List<MarketDatum>();
                        var existingDates = await _context.MarketData
                            .Where(m => m.AssetId == asset.Id)
                            .Select(m => m.DataDate)
                            .ToListAsync();

                        var existingDatesSet = new HashSet<DateOnly>(existingDates);

                        foreach (JsonProperty day in timeSeries.EnumerateObject())
                        {
                            if (!DateOnly.TryParseExact(day.Name, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
                                continue;

                            if (!existingDatesSet.Contains(date))
                            {
                                var dayData = day.Value;

                                var record = new MarketDatum
                                {
                                    AssetId = asset.Id,
                                    DataDate = date,
                                    Open = ParseDecimal(dayData, "1. open"),
                                    High = ParseDecimal(dayData, "2. high"),
                                    Low = ParseDecimal(dayData, "3. low"),
                                    Close = ParseDecimal(dayData, "4. close")
                                };

                                newRecords.Add(record);
                            }
                        }

                        if (newRecords.Any())
                        {
                            await _context.MarketData.AddRangeAsync(newRecords);
                            await _context.SaveChangesAsync();
                            return null;
                        }
                        else
                        {
                            return null; // Brak nowych danych to nie błąd
                        }
                    }
                    else
                    {
                        return "Błąd: Nie znaleziono danych 'Time Series' w odpowiedzi JSON.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }


        private decimal ParseDecimal(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement value))
            {
                if (decimal.TryParse(value.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}