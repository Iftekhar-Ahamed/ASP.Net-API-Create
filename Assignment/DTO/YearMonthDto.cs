using Microsoft.VisualBasic;
using System;

namespace Assignment.DTO
{
    public class YearMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public string GetMonthName()
        {
            string monthName = string.Empty;
            DateTime dateTime = new DateTime(this.Year,this.Month,1);
            monthName = dateTime.ToString("MMMM");
            return monthName;
        }
    }
}
