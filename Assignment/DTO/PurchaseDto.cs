using System;

namespace Assignment.DTO
{
    public class PurchaseDto
    {
        public int IntPurchaseId { get; set; }
        public int? IntSupplierId { get; set; }
        public DateTime? DtePurchaseDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
