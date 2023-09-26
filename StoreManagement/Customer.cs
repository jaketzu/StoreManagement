using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;

namespace StoreManagement
{
    public class Customer
    {
        [JsonIgnore]
        public int Id { get; private set; } = App.Customers.Count;
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Product> Cart { get; private set; } = new();
        public ObservableCollection<Product> Products { get; set; } = new();

        // called when reloading customers
        public void SetId(int index)
        {
            Id = index;
        }

        public void AddToCart(Product product)
        {
            Cart.Add(product);
        }

        public void Purchase()
        {
            foreach (Product product in Cart)
            {
                Products.Add(product);
            }
        }

        // called when updating the text in cartTextBlock
        // returns a string containing all the items in the cart in order from newest to oldest
        public string GetCart()
        {
            if (Cart.Count == 0)
            {
                return "No products in cart.";
            }
            else
            {
                List<Product> products = Cart;
                products.Reverse();

                StringBuilder sb = new();

                foreach (Product product in products)
                {
                    sb.AppendLine(product.ToString());
                }

                return sb.ToString();
            }
        }

        // called when updating customerStatisticsTextBlock
        // returns a string containing all the items bought in order from newest to oldest
        public string GetProducts()
        {
            if (Products.Count == 0)
            {
                return "No products bought.";
            }
            else
            {
                List<Product> productsReversed = new();
                productsReversed.AddRange(Products);
                productsReversed.Reverse();

                StringBuilder sb = new();

                foreach (Product product in productsReversed)
                {
                    sb.AppendLine(product.ToString());
                }

                return sb.ToString();
            }
        }

        public decimal GetMoneySpent()
        {
            decimal money = 0;
            foreach (Product product in Products)
            {
                money += product.Price * product.Amount;
            }

            return money;
        }

        public string GetStatistics()
        {
            return $"{Id}: {Name}\nMoney spent: {GetMoneySpent()}€\nProducts bought:\n\n{GetProducts()}";
        }

        public override string ToString()
        {
            return $"{Id}: {Name}";
        }
    }
}
