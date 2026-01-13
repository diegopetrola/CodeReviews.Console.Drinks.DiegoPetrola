using DrinksInfo.Models;
using System.Text.Json;
using System.Web;

namespace DrinksInfo.Services
{
    public class DrinksService
    {
        private readonly Uri Url = new Uri("http://www.thecocktaildb.com/api/json/v1/1/");
        private readonly string GetCategoryUrl = "list.php?c=list";

        public async Task<List<Category>> GetCategories()
        {
            using var client = new HttpClient { BaseAddress = Url };

            var response = await client.GetAsync(GetCategoryUrl);
            List<Category> categories = [];
            if (response.IsSuccessStatusCode)
            {
                var rawResponse = response.Content;
                var jsonText = await rawResponse.ReadAsStringAsync();
                categories = JsonSerializer.Deserialize<Categories>(jsonText)!.CategoriesList;
            }
            else
            {
                throw new HttpRequestException("Error on the server, please try again in a few minutes");
            }

            return categories;
        }

        internal async Task<List<Drink>> GetDrinksByCategory(Category category)
        {
            using var client = new HttpClient { BaseAddress = Url };
            var response = await client.GetAsync($"filter.php?c={HttpUtility.UrlEncode(category.StrCategory)}");
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
            var client = new HttpClient { BaseAddress = Url };
            var response = await client.GetAsync($"lookup.php?i={drinkId}");

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