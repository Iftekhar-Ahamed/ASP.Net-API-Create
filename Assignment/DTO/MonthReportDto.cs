namespace Assignment.DTO
{
    public class MonthReportDto
    {
        public string monthname { get; set; }
        public int year { get; set; }
        public decimal?  totalPurchaseAmount { get; set; }
        public decimal?  totalSalesAmount { get; set; }
        public string status { get; set; }
    }
}
