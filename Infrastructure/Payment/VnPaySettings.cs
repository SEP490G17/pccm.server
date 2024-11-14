namespace API.DTOs
{
    public class VnPaySettings
    {
        public string HashSecret { get; set; }
        public string Url { get; set; }
        public string TmnCode { get; set; }
        public string ReturnUrl { get; set; }
    }
}