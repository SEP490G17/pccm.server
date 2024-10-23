using Domain.Enum;

namespace Application.DTOs
{
    public class NewsBlogDTO
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public BannerStatus Status { get; set; }
        public string[] Tags { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Thời gian tạo sự kiện

    }
}