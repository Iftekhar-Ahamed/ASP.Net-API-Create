using System;

namespace Assignment.DTO
{
    public class MonthlySalesReportDto
    {
        
        public int IntDetailsId { get; set; }
        public int? IntSalesId { get; set; }
        public int? IntCustomerId { get; set; }
        public DateTime? DteSalesDate { get; set; }
        public int? IntItemId { get; set; }
        public decimal? NumItemQuantity { get; set; }
        public decimal? NumUnitPrice { get; set; }
    }
}
