using System;

namespace Assignment.DTO
{
    public class JoinDto
    {
        public int ItemId { get; set; }
        public DateTime? PDteDate { get; set; }
        public DateTime? SDteDate { get; set; }

        public decimal? PQuantity { get; set; }
        public decimal? SQuantity { get; set; }
        public decimal? PCost { get; set; }
        public decimal? SCost { get; set; }

        public string ItemName { get; set; } = string.Empty;

    }
}
