namespace Application.DTOs
{
    public class AvailableSlotDto
    {
        public class CourtAvailableSlot
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<string> AvailableSlots { get; set; }
        }

        public class AvailableSlotsResponse
        {
            public List<CourtAvailableSlot> AvailableSlots { get; set; } = new List<CourtAvailableSlot>();
        }

    }
}