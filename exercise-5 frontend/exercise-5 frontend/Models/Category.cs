namespace exercise_5_frontend.Models
{
    public class Category
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public Category(string x)
        {
            this.Name = x;
            this.Id = Guid.NewGuid(); ;
        }
        public Category() { }

    }
}
