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
            {
                using var client = new HttpClient { BaseAddress = Url };
                var response = await client.GetAsync($"filter.php?c={HttpUtility.UrlEncode(category.strCategory)}");
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
        }

        //internal void GetDrink(string drink)
        //{
        //    var client = new RestClient("http://www.thecocktaildb.com/api/json/v1/1/");
        //    var request = new RestRequest($"lookup.php?i={drink}");
        //    var response = client.ExecuteAsync(request);

        //    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        string rawResponse = response.Result.Content;

        //        var serialize = JsonConvert.DeserializeObject<DrinkDetailObject>(rawResponse);

        //        List<DrinkDetail> returnedList = serialize.DrinkDetailList;

        //        DrinkDetail drinkDetail = returnedList[0];

        //        List<object> prepList = new();

        //        string formattedName = "";

        //        foreach (PropertyInfo prop in drinkDetail.GetType().GetProperties())
        //        {

        //            if (prop.Name.Contains("str"))
        //            {
        //                formattedName = prop.Name.Substring(3);
        //            }

        //            if (!string.IsNullOrEmpty(prop.GetValue(drinkDetail)?.ToString()))
        //            {
        //                prepList.Add(new
        //                {
        //                    Key = formattedName,
        //                    Value = prop.GetValue(drinkDetail)
        //                });
        //            }
        //        }

        //        TableVisualisationEngine.ShowTable(prepList, drinkDetail.strDrink);


        //    }
        //}
    }
}