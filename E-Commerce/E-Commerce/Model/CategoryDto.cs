namespace E_Commerce.Model
{
    public class CategoryDto
    {
        public required string Name { get; set; }
        public ICollection<SubCategoryDto> SubCategories { get; set; } = new List<SubCategoryDto>();
    }
}
