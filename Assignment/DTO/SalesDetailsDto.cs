﻿namespace Assignment.DTO
{
    public class SalesDetailsDto
    {
        public int IntDetailsId { get; set; }
        public int? IntSalesId { get; set; }
        public int? IntItemId { get; set; }
        public decimal? NumItemQuantity { get; set; }
        public decimal? NumUnitPrice { get; set; }
        public bool? IsActive { get; set; }
    }
}
