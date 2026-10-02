namespace _28_VuDucLuc_Assignment01_FrontEnd.Models
{
    public class Category
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDesciption { get; set; } = string.Empty;
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
