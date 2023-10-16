using System;

namespace Assignment.DTO
{
    public class SalesDto
    {
        public int IntSalesId { get; set; }
        public int? IntCustomerId { get; set; }
        public DateTime? DteSalesDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
