namespace Application.SpecParams.CourtClusterSpecification
{
    public class CourtClusterSpecParam : BaseSpecParam
    {
        public string? Province { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Rating { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
    }
}
