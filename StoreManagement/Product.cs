namespace StoreManagement
{
    public class Product
    {
        public string Producer { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public uint Amount { get; set; } = 0;

        public override string ToString()
        {
            return $"{Producer} {Name}: {Price:F}€ ({Amount})";
        }
    }
}
