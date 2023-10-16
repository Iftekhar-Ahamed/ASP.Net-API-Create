
using Assignment.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Assignment.DTO;
using System.Collections.Generic;
using Assignment.Helper;
using System;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignment _IRepository;
        public AssignmentController(IAssignment IRepository)
        {
            _IRepository = IRepository;
        }
        [HttpPost]
        [Route("CreatePartnerType")]
        public async Task<IActionResult> CreatePartnerType(PartnerTypeDto ob)
        {
            return Ok(await _IRepository.AddNewPartnerType(ob));
        }
        [HttpPost]
        [Route("CreatePartner")]
        public async Task<IActionResult> CreatePartner(PartnerDto ob)
        {
            return Ok(await _IRepository.CreatePartner(ob));
        }
        [HttpPost]
        [Route("CreateSomeItems")]
        public async Task<IActionResult> CreateSomeItems(List<ItemDto> ob)
        {
            return Ok(await _IRepository.CreateListOfItem(ob));
        }
        [HttpPut]
        [Route("EditSomeItems")]
        public async Task<IActionResult> EditSomeItems(List<ItemDto> ob)
        {
            return Ok(await _IRepository.EditListOfItem(ob));
        }
        [HttpPost]
        [Route("PurchaseSomeItem")]
        public async Task<IActionResult> PurchaseSomeItems(PurchaseItemDto purchaseItemDto)
        {
            return Ok(await _IRepository.PurchaseListItem(purchaseItemDto.PurchaseDto,purchaseItemDto.purchaseDetailsDtos));
        }
        [HttpPost]
        [Route("SaleSomeItem")]
        public async Task<IActionResult> SaleSomeItems(SalesItemDto salesItemDto)
        {
            return Ok(await _IRepository.SaleListItem(salesItemDto.salesDto,salesItemDto.salesDetailsDtos));
        }
        [HttpGet]
        [Route("DailyPurchaseReport")]
        public async Task<IActionResult> DailyPurchaseReport(DateTime day)
        {
            return Ok(await _IRepository.GetDailyPurchaseReport(day));
        }
        [HttpGet]
        [Route("MonthlySalesReport")]
        public async Task<IActionResult> MonthlySalesReport(string month)
        {
            return Ok(await _IRepository.GetMonthlySalesReport(month));
        }
        [HttpGet]
        [Route("ItemWiseDailyPurchaseVsSalesReport")]
        public async Task<IActionResult> ItemWiseDailyPurchase_VS_SalesReport(string day)
        {
            return Ok(await _IRepository.GetItemWiseDailyPurchase_VS_SalesReport(day));
        }
        [HttpGet]
        [Route("MonthlyPurchaseVsSalesReport")]
        public async Task<IActionResult> MonthlyPurchaseVSSalesReport()
        {
            return Ok(await _IRepository.GETMonthlyPurchaseVSSalesReport());
        }
        [HttpGet]
        [Route("PracticeGroupBy")]
        public async Task<IActionResult> PracticeGroupBy()
        {
            return Ok(await _IRepository.PracticeGroupBy());
        }
    }
}
