using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Windows;

namespace StoreManagement
{
    public partial class App : Application
    {
        private readonly string filename = "Store.json";
        public static ObservableCollection<Customer> Customers { get; private set; } = new();
        public static ObservableCollection<Product> Products { get; private set; } = new();

        // called on startup
        // gets app's customers and products from isolated storage in local appadata by desearilizing a json file
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                using IsolatedStorageFileStream stream = new(filename, FileMode.Open, storage);
                using StreamReader reader = new(stream);
                string json = reader.ReadToEnd();
                string[] jsons = json.Split("\n\n\n");

                Customers = JsonSerializer.Deserialize<ObservableCollection<Customer>>(jsons[0])!;
                Products = JsonSerializer.Deserialize<ObservableCollection<Product>>(jsons[1])!;

                for (int i = 0; i < Customers.Count; i++)
                {
                    Customers[i].SetId(i);
                }
            }
            catch
            {
                using IsolatedStorageFileStream stream = new(filename, FileMode.Create, storage);
                string json = JsonSerializer.Serialize(Customers, new JsonSerializerOptions { WriteIndented = true }) + "\n\n\n" + JsonSerializer.Serialize(Products, new JsonSerializerOptions { WriteIndented = true });
                using StreamWriter writer = new(stream);
                writer.Write(json);
            }
        }

        // called on exit
        // serializes app's customers and products into a json file in isolated storage in local appdata
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using IsolatedStorageFileStream stream = new(filename, FileMode.Create, storage);
            string json = JsonSerializer.Serialize(Customers, new JsonSerializerOptions { WriteIndented = true }) + "\n\n\n" + JsonSerializer.Serialize(Products, new JsonSerializerOptions { WriteIndented = true });
            using StreamWriter writer = new(stream);
            writer.Write(json);
        }
    }
}
