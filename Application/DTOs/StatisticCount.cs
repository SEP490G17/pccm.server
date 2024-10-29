﻿using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StatisticCount
    {
        public int TotalCourtClusters { get; set; }
        public int TotalCourts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalStaff { get; set; }
        public string[] TopStaffs { get; set; }
        public string[] TopProducts { get; set; }

    }
}
