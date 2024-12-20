﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ReviewInputDto
    {
        [Key]
        public int Id { get; set; }
        public string phoneNumber { get; set; }
        public int? CourtClusterId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }  // Điểm đánh giá từ 1 đến 5
        public string Comment { get; set; }  // Bình luận chi tiết của người dùng
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Thời gian đánh giá được tạo
    }
}
