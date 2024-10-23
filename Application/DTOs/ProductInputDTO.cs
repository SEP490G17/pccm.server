﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductInputDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int CourtClusterId { get; set; }
        [Required]
        public string ProductName { get; set; }  // Tên sản phẩm
        [Required]
        public string Description { get; set; }  // Mô tả chi tiết về sản phẩm
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }  // Số lượng sản phẩm có sẵn
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }  // Giá sản phẩm
        [Required]
        [StringLength(255)]
        public string Thumbnail { get; set; } // URL ảnh đại diện cho Product
    }
}
