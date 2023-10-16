using Assignment.DTO;
using Assignment.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.IRepository
{
    public interface IAssignment
    {

        Task<MessageHelper> AddNewPartnerType(PartnerTypeDto ob);
        Task<MessageHelper> CreatePartner(PartnerDto ob);
        Task<TMessageHelper<List<ItemDto>>> CreateListOfItem(List<ItemDto> ob);
        Task<TMessageHelper<List<ItemDto>>> EditListOfItem(List<ItemDto> ob);
        Task<TMessageHelper<PurchaseDto>> PurchaseListItem(PurchaseDto p, List<PurchaseDetailsDto> pd);
        Task<TMessageHelper<List<SalesDetailsDto>>> SaleListItem(SalesDto s, List<SalesDetailsDto> sd);
        Task<TMessageHelper<List<DailyPurchaseReportDto>>> GetDailyPurchaseReport(DateTime day);
        Task<TMessageHelper<List<MonthlySalesReportDto>>> GetMonthlySalesReport(string _month);
        Task<TMessageHelper<List<DailyPurchaseSalesReportDto>>> GetItemWiseDailyPurchase_VS_SalesReport(string day);
        Task<TMessageHelper<List<MonthReportDto>>> GETMonthlyPurchaseVSSalesReport();
        Task<TMessageHelper<List<PurchaseDetailsDto>>> PracticeGroupBy();
    }
}
