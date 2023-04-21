namespace Mango.Web.Models
{
    public class FilterProduct
    {
        public string Sort { get; set; }
        public string SearchTerm { get; set; }
        public int Length { get; set; } = SD.MAX_ITEMS;
        public int Page { get; set; } = 1;
    }
}
