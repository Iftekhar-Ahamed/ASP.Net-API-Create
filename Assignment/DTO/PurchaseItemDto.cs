using System;
using System.Collections.Generic;

namespace Assignment.DTO
{
    public class PurchaseItemDto
    {
        public PurchaseDto PurchaseDto { get; set; }
        public List<PurchaseDetailsDto> purchaseDetailsDtos { get; set; }
}
}
