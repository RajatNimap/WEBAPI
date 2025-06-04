namespace MvcLearning.Model.Entities
{
    public class TodoModel
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.Now;    

    }
}
