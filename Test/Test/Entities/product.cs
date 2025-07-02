namespace Test.Entities
{
    public class product
    {
        public int Id { get; set; } 
        public string Name { get; set; }    
        public string Description { get; set; } 
        public double price { get; set; }


        category category { get; set; } 
        public List<category> categories { get; set; }  



    }
}
