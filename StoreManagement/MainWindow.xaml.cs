using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace StoreManagement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            customerComboBox1.ItemsSource = App.Customers;
            customerComboBox2.ItemsSource = App.Customers;
            productComboBox1.ItemsSource = App.Products;
            productComboBox2.ItemsSource = App.Products;
        }

        // adds a product with the chosen parameters
        // if the product already exists in the store, it just adds the amount to the existing product
        // if not, creates a new product and adds it to the store
        private void AddProducts(object sender, RoutedEventArgs e)
        {
            try
            {
                string productProducer = productProducerTextBox.Text;
                string productName = productNameTextBox.Text;
                string productPriceString = productPriceTextBox.Text;
                string productAmountString = productAmountTextBox1.Text;

                if (productProducer != string.Empty &&
                    productName != string.Empty &&
                    productPriceString != string.Empty &&
                    productAmountString != string.Empty)
                {
                    if (double.TryParse(productPriceString, out double productPrice) &&
                        int.TryParse(productAmountString, out int productAmount) &&
                        productPrice > 0 &&
                        productAmount > 0)
                    {
                        Product? product = new();
                        if (!App.Products.Any(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice))
                        {
                            product = new()
                            {
                                Producer = productProducer,
                                Name = productName,
                                Price = productPrice,
                                Amount = productAmount,
                            };
                            App.Products.Add(product);
                        }
                        else
                        {
                            product = App.Products.ToList()
                                .Find(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice);
                            if (product != null)
                                product.Amount += productAmount;
                        }

                        productProducerTextBox.Clear();
                        productNameTextBox.Clear();
                        productPriceTextBox.Clear();
                        productAmountTextBox1.Text = "1";

                        ReloadProducts(App.Products.Count);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Wrong type of input for either the price or amount of the product.",
                            "Check your inputs.",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "One or more of the input text boxes are empty.",
                        "Check your inputs.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // removes the selected product
        private void RemoveProduct(object sender, RoutedEventArgs e)
        {
            try
            {
                if (productComboBox1.SelectedItem is Product selectedProduct)
                {
                    int selectedIndex = productComboBox1.SelectedIndex;
                    App.Products.Remove(selectedProduct);
                    ReloadProducts(selectedIndex);
                }
                else
                {
                    MessageBox.Show(
                        "You have not selected a product to remove.",
                        "No product selected.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // adds the chosen amount to the selected product
        private void RefillProduct(object sender, RoutedEventArgs e)
        {
            try
            {
                if (productComboBox2.SelectedItem is Product selectedProduct)
                {
                    if (int.TryParse(productAmountTextBox2.Text, out int refillAmount) && refillAmount > 0)
                    {
                        selectedProduct.Amount += refillAmount;
                        productAmountTextBox2.Text = "1";
                        ReloadProducts(productComboBox2.SelectedIndex);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Wrong type of input for the amount of the product.",
                            "Check your inputs.",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "You have not selected a product to refill.",
                        "No product selected.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // adds a new customer with the chosen name
        private void AddCustomer(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = nameTextBox.Text;
                if (name != string.Empty)
                {
                    Customer newCustomer = new()
                    {
                        Name = name,
                    };

                    App.Customers.Add(newCustomer);
                    customerComboBox1.SelectedItem = newCustomer;
                    customerComboBox2.SelectedItem = newCustomer;

                    nameTextBox.Clear();
                }
                else
                {
                    MessageBox.Show(
                        "Text box for the name of the customer is empty. Please insert a name.",
                        "Customer must be given a name.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // removes the selected customer
        private void RemoveCustomer(object sender, RoutedEventArgs e)
        {
            try
            {
                if (customerComboBox1.SelectedItem is Customer selectedCustomer)
                {
                    int selectedIndex = customerComboBox1.SelectedIndex;

                    App.Customers.Remove(selectedCustomer);

                    ReloadCustomers(selectedIndex);
                }
                else
                {
                    MessageBox.Show(
                        "You have not selected a customer to remove.",
                        "No customer selected.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // serializes the selected customer into a json file
        private void ExportCustomer(object sender, RoutedEventArgs e)
        {
            try
            {
                if (customerComboBox1.SelectedItem is Customer customer)
                {
                    Microsoft.Win32.SaveFileDialog dialog = new()
                    {
                        FileName = customer.Name,
                        DefaultExt = ".json",
                        Filter = "JSON files| *.json",
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        string jsonString = JsonSerializer.Serialize(customer, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(dialog.FileName, jsonString);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "No customer selected. Select a customer.",
                        "No customer selected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        // deserealizes selected json files into customers and adds them
        private void ImportCustomers(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new()
                {
                    DefaultExt = ".json",
                    Filter = "JSON files| *.json",
                    Multiselect = true,
                };

                if (dialog.ShowDialog() == true)
                {
                    foreach (string file in dialog.FileNames)
                    {
                        string json = File.ReadAllText(file);
                        Customer? importedCustomer = JsonSerializer.Deserialize<Customer>(json);

                        if (importedCustomer != null)
                            App.Customers.Add(importedCustomer);
                    }

                    ReloadCustomers(App.Customers.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        // adds a chosen amount of the selected product to the selected customers cart
        private void AddToCart(object sender, RoutedEventArgs e)
        {
            try
            {
                string amountString = productAmountTextBox3.Text;
                if (customerComboBox2.SelectedItem is Customer selectedCustomer &&
                    productComboBox2.SelectedItem is Product selectedProduct)
                {
                    if (int.TryParse(amountString, out int productAmount) &&
                        productAmount > 0)
                    {
                        string productProducer = selectedProduct.Producer;
                        string productName = selectedProduct.Name;
                        double productPrice = selectedProduct.Price;

                        Product product = new();

                        if (App.Products.ToList().Find(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice) is Product storeProduct && storeProduct.Amount > productAmount)
                        {
                            if (!selectedCustomer.Cart.Exists(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice))
                            {
                                product = new()
                                {
                                    Producer = productProducer,
                                    Name = productName,
                                    Price = productPrice,
                                    Amount = productAmount,
                                };

                                selectedCustomer.Cart.Add(product);
                            }
                            else
                            {
                                if (selectedCustomer.Cart.Find(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice) is Product cartProduct && storeProduct.Amount > cartProduct.Amount + productAmount)
                                {
                                    cartProduct.Amount += productAmount;
                                }
                                else
                                {
                                    MessageBox.Show(
                                        "Trying to add too many products to cart.",
                                        "You are trying to add more products than available to the cart. Please check the amount you're trying to add.",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                "Trying to add too many products to cart.",
                                "You are trying to add more products than available to the cart. Please check the amount you're trying to add.",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Wrong type of input for the amount of the product.",
                            "Check your inputs.",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }

                    productAmountTextBox3.Text = "1";
                    cartTextBlock.Text = selectedCustomer.GetCart();
                }
                else
                {
                    MessageBox.Show(
                        "No customer or product selected.",
                        "Select a customer and product.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // adds the products in the selected customers cart to the customers purchased products
        // removes the correct amount of the product in the store
        private void Purchase(object sender, RoutedEventArgs e)
        {
            try
            {
                if (customerComboBox2.SelectedItem is Customer selectedCustomer)
                {
                    if (selectedCustomer.Cart.Count > 0)
                    {
                        foreach (Product cartProduct in selectedCustomer.Cart)
                        {
                            string productProducer = cartProduct.Producer;
                            string productName = cartProduct.Name;
                            double productPrice = cartProduct.Price;
                            int productAmount = cartProduct.Amount;

                            Product newProduct = new();

                            if (!selectedCustomer.Products.Any(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice))
                            {
                                newProduct = new()
                                {
                                    Producer = productProducer,
                                    Name = productName,
                                    Price = productPrice,
                                    Amount = productAmount,
                                };

                                selectedCustomer.Products.Add(newProduct);
                            }
                            else
                            {
                                if (selectedCustomer.Products.ToList()
                                    .Find(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice) is Product existingProduct)
                                    existingProduct.Amount += productAmount;
                            }

                            Product? storeProduct = App.Products.ToList()
                                .Find(product => product.Producer == productProducer && product.Name == productName && product.Price == productPrice);

                            storeProduct!.Amount -= productAmount;
                        }

                        OnCustomerComboBox1SelectionChanged(sender, e);

                        selectedCustomer.Cart.Clear();
                        cartTextBlock.Text = selectedCustomer.GetCart();

                        ReloadProducts(productComboBox2.SelectedIndex);
                    }
                    else
                    {
                        MessageBox.Show(
                        "Please add products to the cart to finish purchase.",
                        "No products in cart.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Please select a customer from the list.",
                        "No customer selected.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // clears the selected customers cart
        private void ClearCart(object sender, RoutedEventArgs e)
        {
            try
            {
                if (customerComboBox2.SelectedItem is Customer selectedCustomer)
                {
                    selectedCustomer.Cart.Clear();
                    cartTextBlock.Text = selectedCustomer.GetCart();
                }
                else
                {
                    MessageBox.Show(
                        "Please select a customer.",
                        "No customer selected.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // called when the selected customer changed
        // sets the customerStatisticsTextBlock's text to the selected customers statistics
        private void OnCustomerComboBox1SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (customerComboBox1.SelectedItem is Customer selectedCustomer)
                {
                    customerStatisticsTextBlock.Text = selectedCustomer.GetStatistics();
                }
                else
                {
                    customerStatisticsTextBlock.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        // called when changing tabs
        // if the selected tab is the 'Handle Purchases' tab, sets the cartTextBlock's text to the selected customers cart
        // if the selected tab is the 'Statistics' tab, reloads the statisticsTextBlock
        private void OnTabChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tabControl.SelectedIndex == 2 && customerComboBox2.SelectedItem is Customer selectedCustomer)
                    cartTextBlock.Text = selectedCustomer.GetCart();

                if (tabControl.SelectedIndex == 3)
                {
                    double income = 0;
                    int productsSold = 0;
                    foreach (Customer customer in App.Customers)
                    {
                        income += customer.GetMoneySpent();
                        foreach (Product product in customer.Products)
                        {
                            productsSold += product.Amount;
                        }
                    }

                    statisticsTextBlock.Text = $"Products sold: {productsSold}\nIncome: {income}€";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        // needed so the combo boxes' text's are accurate after changing a products count
        // clears the products and readds them, but sorted
        // sets the selectedindex of the combo boxes to the correct place
        public void ReloadProducts(int selectedIndex)
        {
            try
            {
                List<Product> sortedProducts = App.Products.OrderBy(product => product.Producer).ToList();

                App.Products.Clear();

                foreach (Product product in sortedProducts)
                    App.Products.Add(product);

                int productAmount = App.Products.Count;
                if (productAmount > 0)
                {
                    if (selectedIndex < productAmount)
                    {
                        productComboBox1.SelectedIndex = selectedIndex;
                        productComboBox2.SelectedIndex = selectedIndex;
                    }
                    else
                    {
                        productComboBox1.SelectedIndex = selectedIndex - 1;
                        productComboBox2.SelectedIndex = selectedIndex - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        // needed so the combo boxes' text's are accurate after adding or removing customers
        // clears the customers and readds them
        // sets the correct ids
        // sets the selectedindex of the combo boxes to the correct place
        public void ReloadCustomers(int selectedIndex)
        {
            try
            {
                List<Customer> customers = App.Customers.ToList();

                App.Customers.Clear();

                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].SetId(i);
                }

                foreach (Customer customer in customers)
                {
                    App.Customers.Add(customer);
                }

                int customerAmount = App.Customers.Count;
                if (customerAmount > 0)
                {
                    if (selectedIndex < customerAmount)
                    {
                        customerComboBox1.SelectedIndex = selectedIndex;
                        customerComboBox2.SelectedIndex = selectedIndex;
                    }
                    else
                    {
                        customerComboBox1.SelectedIndex = selectedIndex - 1;
                        customerComboBox2.SelectedIndex = selectedIndex - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}
