using DrinksInfo.Models;
using System.Text.Json;
using System.Web;

namespace DrinksInfo.Services
{
    public class DrinksService(HttpClient httpClient)
    {

        public async Task<List<Category>> GetCategories()
        {
            var response = await httpClient.GetAsync("list.php?c=list");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Error on the server, please try again later.");

            var jsonText = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Categories>(jsonText)!.CategoriesList;
        }

        internal async Task<List<Drink>> GetDrinksByCategory(Category category)
        {
            var response = await httpClient.GetAsync($"filter.php?c={HttpUtility.UrlEncode(category.StrCategory)}");
            List<Drink> drinks = [];

            if (response.IsSuccessStatusCode)
            {
                string rawResponse = await response.Content.ReadAsStringAsync();
                var serialize = JsonSerializer.Deserialize<Drinks>(rawResponse);

                drinks = serialize!.DrinksList;
            }
            else
            {
                throw new HttpRequestException("Error on the server, please try again in a few minutes");
            }
            return drinks;
        }

        internal async Task<DrinkDetail> GetDrink(string drinkId)
        {
            var response = await httpClient.GetAsync($"lookup.php?i={drinkId}");

            if (response.IsSuccessStatusCode)
            {
                var strResponse = await response.Content.ReadAsStringAsync();
                var drink = JsonSerializer.Deserialize<DrinkDetailObject>(strResponse).DrinkDetailList;
                return drink[0];
            }
            else
            {
                throw new HttpRequestException("Error on the server, please try again in a few minutes");
            }
        }
    }
}