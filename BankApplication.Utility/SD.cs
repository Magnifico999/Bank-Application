namespace BankApplication.Utility
{
    public static class SD
    {
        public static string productAPIBase { get; set; }
        public enum ApiType
        {
            GET,
            POST, 
            PUT,
            DELETE
        }
        public static string SessionToken = "JWTToken";
    }
}