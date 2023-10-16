using System;

namespace Assignment.DTO
{
    public class DailyPurchaseReportDto
    {
        public int IntDetailsId { get; set; }
        public int? IntPurchaseId { get; set; }
        public int? IntSupplierId { get; set; }
        public DateTime? DtePurchaseDate { get; set; }
        public int? IntItemId { get; set; }
        public decimal? NumItemQuantity { get; set; }
        public decimal? NumUnitPrice { get; set; }
    }
}
