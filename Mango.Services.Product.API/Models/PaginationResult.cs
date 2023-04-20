namespace Mango.Services.ProductAPI.Models
{
    public class PaginationResult<T>
    {
        public List<T> Data { get; set; }
        public int TotalPages { get; set; }
        public int TotalFilteredItems { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public FilterProduct Filter { get; set; }
    }
}
