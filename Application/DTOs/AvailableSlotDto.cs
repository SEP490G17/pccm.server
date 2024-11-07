using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AvailableSlotDto
    {
        public class AvailableSlotsResponse
        {
            public Dictionary<string, List<string>> AvailableSlots { get; set; } = new Dictionary<string, List<string>>();
        }

    }
}