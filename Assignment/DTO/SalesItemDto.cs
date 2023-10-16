using System.Collections.Generic;

namespace Assignment.DTO
{
    public class SalesItemDto
    {
        public SalesDto salesDto { get; set; }
        public List<SalesDetailsDto> salesDetailsDtos { get; set; }
    }
}
