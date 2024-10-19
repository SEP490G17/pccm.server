using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.SpecParams
{
    public class BaseSpecWithFilterParam : BaseSpecParam
    {
        public int? Filter { get; set; }
    }
}