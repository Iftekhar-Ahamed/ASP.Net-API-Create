using System;

namespace Assignment.DTO
{
    public class DailyPurchaseSalesReportDto
    {
        public int IntItemId { get; set; }
        public string StrItemName { get; set; }
        public DateTime? PurchaseDteDate { get; set; }
        public DateTime? SalesDteDate { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public decimal? SalesQuantity { get; set; }
        public decimal? PurchaseCost { get; set; }
        public decimal? SalesCost { get; set; }

    }
}
