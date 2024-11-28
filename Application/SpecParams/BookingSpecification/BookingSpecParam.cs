namespace Application.SpecParams.BookingSpecification
{
    public class BookingSpecParam : BaseSpecParam
    {
        public int? CourtClusterId { get; set; }
        public int? CourtId { get; set; }
        public int? Status { get; set; }
        public string? fromDate { get; set; }
        public string? toDate { get; set; }
    }
}