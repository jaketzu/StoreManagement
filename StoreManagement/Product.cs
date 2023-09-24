namespace StoreManagement
{
    public class Product
    {
        public string Producer { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
        public int Amount { get; set; } = 0;

        public override string ToString()
        {
            return $"{Producer} {Name}: {Price}€ ({Amount})";
        }
    }
}
