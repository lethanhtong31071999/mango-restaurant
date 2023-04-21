namespace Mango.Web
{
    public static class SD
    {
        public static string ProductAPIBase { get; set; }
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
        }
        public static int MAX_ITEMS = 10;
    }
}
