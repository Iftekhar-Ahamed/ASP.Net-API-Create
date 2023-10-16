using Assignment.DbContexts;
using Assignment.DTO;
using Assignment.Helper;
using Assignment.IRepository;
using Assignment.Models.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable
namespace Assignment.Repository
{
    public class AssignmentRepository : IAssignment
    {
        private readonly WriteDbContext _contextW;
        private readonly ReadDbContext _contextR;
        private readonly IJwtToken _JwtToken;
        public AssignmentRepository(WriteDbContext contextW, ReadDbContext contextR, IJwtToken jwtToken)
        {
            _contextW = contextW;
            _contextR = contextR;
            _JwtToken = jwtToken;
        }

        public async Task<MessageHelper> AddNewPartnerType(PartnerTypeDto ob)
        {
            var msg = new MessageHelper();
            msg.Message = "Duplicate Found";
            msg.statuscode = 200;


            var isExists = await (from t in _contextW.TblPartnerType
                                  where t.IsActive == true
                                        && t.StrPartnerTypeName == ob.StrPartnerTypeName
                                  select t).FirstOrDefaultAsync();

            if (isExists == null)
            {
                TblPartnerType partnerType = new TblPartnerType
                {
                    StrPartnerTypeName = ob.StrPartnerTypeName,
                    IsActive = true,
                };

                await _contextW.TblPartnerType.AddAsync(partnerType);
                await _contextW.SaveChangesAsync();

                msg.Message = "Saved Successfully";
                msg.statuscode = 200;
            }
            return msg;
        }

        public async Task<MessageHelper> CreatePartner(PartnerDto ob)
        {
            var msg = new MessageHelper();
            msg.Message = "Faild to create";
            msg.statuscode = 200;


            TblPartner partner = new TblPartner
            {
                StrPartnerName = ob.StrPartnerName,
                IsActive = true,
                IntPartnerTypeId = ob.IntPartnerTypeId,
            };

            await _contextW.TblPartner.AddAsync(partner);
            if (await _contextW.SaveChangesAsync() != null)
            {
                msg.Message = "Successfully Created";
                msg.statuscode = 200;
            }
            return msg;
        }
        public async Task<TMessageHelper<List<ItemDto>>> CreateListOfItem(List<ItemDto> ob)
        { 
            var msg = new TMessageHelper<List<ItemDto>>();
            msg.Message = "Duplicate data in value";
            msg.statuscode = 200;

            List<TblItem> uniqueItemsList = new List<TblItem>();
            List<ItemDto> duplicateList = ob.GroupBy(x => x.StrItemName).Where(g => g.Count() > 1).SelectMany(g => g.Skip(1)).ToList();
            ob = ob.GroupBy(x => x.StrItemName).Select(x => x.First()).ToList();


            foreach (var item in ob)
            {
                //var res =await (from i in _contextR.TblItem where i.StrItemName == item.StrItemName).FirstOrDefaultAsync();
                if (await _contextR.TblItem.Where(x => x.StrItemName == item.StrItemName).FirstOrDefaultAsync() == null)
                {
                    var ModelItem = new TblItem
                    {
                        StrItemName = item.StrItemName,
                        NumStockQuantity = item.NumStockQuantity,
                        IsActive = true,
                    };
                    uniqueItemsList.Add(ModelItem);
                }
                else
                {
                    duplicateList.Add(item);
                }
            }

            await _contextW.AddRangeAsync(uniqueItemsList);
            if (await _contextW.SaveChangesAsync() > 0)
            {
                msg.Message = "Successfully Created." + msg.Message;
                msg.statuscode = 200;
            }
            else
            {
                msg.Message = "Faild to Created." + msg.Message;
            }
            msg.Value = duplicateList;
            return msg;
        }
        public async Task<TMessageHelper<List<ItemDto>>> EditListOfItem(List<ItemDto> ob)
        {
            var msg = new TMessageHelper<List<ItemDto>>();
            msg.Message = "Duplicate data in value";
            msg.statuscode = 200;

            List<TblItem> updateItemsList = new List<TblItem>();

            List<ItemDto> duplicateList = ob.GroupBy(x => x.StrItemName).Where(g => g.Count() > 1).SelectMany(g => g.Skip(1)).ToList();
            

            ob = ob.GroupBy(x => x.StrItemName).Select(x => x.First()).ToList();

            foreach (var item in ob)
            {
                //var res = await (from i in _contextR.TblItem where i.StrItemName == item.StrItemName select new { ItemId = i.IntItemId}).FirstOrDefaultAsync()==null && await (from i in _contextR.TblItem where i.IntItemId ==item.IntItemId select new { itemid = i.IntItemId}).FirstOrDefaultAsync()!=null;
                if (await _contextR.TblItem.Where(x => x.StrItemName == item.StrItemName).FirstOrDefaultAsync() == null && await _contextR.TblItem.Where(x => x.IntItemId == item.IntItemId).FirstOrDefaultAsync() != null)
                {
                    var ModelItem = new TblItem
                    {
                        IntItemId = item.IntItemId,
                        StrItemName = item.StrItemName,
                        IsActive = item.IsActive,
                        NumStockQuantity = item.NumStockQuantity,
                    };
                    updateItemsList.Add(ModelItem);
                }
                else
                {
                    duplicateList.Add(item);
                }
            }
            _contextW.UpdateRange(updateItemsList);
            if (await _contextW.SaveChangesAsync() > 0)
            {
                msg.Message = "Successfully Edit." + msg.Message;
                msg.statuscode = 200;
            }
            else
            {
                msg.Message = "Faild to Edit." + msg.Message;
            }
            msg.Value = duplicateList;
            return msg;
        }

        public async Task<TMessageHelper<PurchaseDto>> PurchaseListItem(PurchaseDto p, List<PurchaseDetailsDto> pd)
        {
            var msg = new TMessageHelper<PurchaseDto>();

            msg.Message = "Purchase List Item Faild";
            msg.statuscode = 200;

            var purchaseTbl = new TblPurchase
            {
                IntSupplierId = p.IntSupplierId,
                IsActive = true,
                DtePurchaseDate = p.DtePurchaseDate,
            };

            await _contextW.TblPurchase.AddAsync(purchaseTbl);
            if (await _contextW.SaveChangesAsync() > 0)
            {
                List<TblPurchaseDetails> pdTbl = new List<TblPurchaseDetails>();
                foreach (var item in pd)
                {
                    var _pdTbl = new TblPurchaseDetails
                    {
                        IntPurchaseId = purchaseTbl.IntPurchaseId,
                        IntItemId = item.IntItemId,
                        NumItemQuantity = item.NumItemQuantity,
                        NumUnitPrice = item.NumUnitPrice,
                        IsActive = true,
                    };

                    var _updateStock = await _contextW.TblItem.Where(x => x.IntItemId == item.IntItemId).FirstOrDefaultAsync();
                    if (_updateStock == null)
                    {
                        continue;
                    }
                    _updateStock.NumStockQuantity += item.NumItemQuantity;
                    _contextW.TblItem.Update(_updateStock);
                    var res = await _contextW.SaveChangesAsync();
                    pdTbl.Add(_pdTbl);
                }
                await _contextW.TblPurchaseDetails.AddRangeAsync(pdTbl);
                if (await _contextW.SaveChangesAsync() > 0)
                {
                    msg.Message = "Purchase List Item Sucessfully Added";
                    msg.statuscode = 200;
                }
            }
            return msg;
        }

        public async Task<TMessageHelper<List<SalesDetailsDto>>> SaleListItem(SalesDto s, List<SalesDetailsDto> sd)
        {
            var msg = new TMessageHelper<List<SalesDetailsDto>>();

            msg.Message = "Sale List Item Faild";
            msg.statuscode = 200;
            msg.Value = new List<SalesDetailsDto>();

            var _sale = new TblSales
            {
                IntCustomerId = s.IntCustomerId,
                DteSalesDate = s.DteSalesDate,
                IsActive = true,
            };

            await _contextW.TblSales.AddAsync(_sale);
            if (await _contextW.SaveChangesAsync() > 0)
            {
                List<TblSalesDetails> sdTbl = new List<TblSalesDetails>();
                foreach (var item in sd)
                {
                    var tblItem = await _contextW.TblItem.Where(x => x.IntItemId == item.IntItemId).FirstOrDefaultAsync();
                    //var tblItem = await (from i in _contextW.TblItem where i.IntItemId == item.IntItemId select i).FirstOrDefaultAsync();

                    if (tblItem.NumStockQuantity >= item.NumItemQuantity)
                    {
                        var _sdTbl = new TblSalesDetails
                        {
                            IntSalesId = _sale.IntSalesId,
                            IntItemId = item.IntItemId,
                            NumItemQuantity = item.NumItemQuantity,
                            NumUnitPrice = item.NumUnitPrice,
                            IsActive = true,
                        };

                        tblItem.NumStockQuantity = tblItem.NumStockQuantity - item.NumItemQuantity;
                        _contextW.Update(tblItem);
                        var r = await _contextW.SaveChangesAsync();
                        sdTbl.Add(_sdTbl);
                    }
                    else
                    {
                        msg.Value.Add(item);
                    }
                }
                await _contextW.TblSalesDetails.AddRangeAsync(sdTbl);
                var res = await _contextW.SaveChangesAsync();
                if (res > 0)
                {
                    msg.Message = "Sale List Item Sucessfully Added";
                    msg.statuscode = 200;
                }
            }
            return msg;
        }

        public async Task<TMessageHelper<List<DailyPurchaseReportDto>>> GetDailyPurchaseReport(DateTime day)
        {
            TMessageHelper<List<DailyPurchaseReportDto>> messageHelper = new TMessageHelper<List<DailyPurchaseReportDto>>();
            messageHelper.Message = "Report of " + day.ToString();

            var _report = (from p in _contextR.TblPurchase
                           join pd in _contextR.TblPurchaseDetails
                           on p.IntPurchaseId equals pd.IntPurchaseId
                           where p.DtePurchaseDate == day
                           select new DailyPurchaseReportDto
                           {
                               IntDetailsId = pd.IntDetailsId,
                               IntPurchaseId = p.IntPurchaseId,
                               IntSupplierId = p.IntSupplierId,
                               DtePurchaseDate = p.DtePurchaseDate,
                               IntItemId = pd.IntItemId,
                               NumItemQuantity = pd.NumItemQuantity,
                               NumUnitPrice = pd.NumUnitPrice
                           }).ToList();
            messageHelper.Value = _report;
            return messageHelper;
        }
        public async Task<TMessageHelper<List<MonthlySalesReportDto>>> GetMonthlySalesReport(string _month)
        {
            TMessageHelper<List<MonthlySalesReportDto>> msg = new TMessageHelper<List<MonthlySalesReportDto>>();
            var _monthlySalesReport = (from s in _contextR.TblSales
                                       join sd in _contextR.TblSalesDetails
                                       on s.IntSalesId equals sd.IntSalesId
                                       where s.DteSalesDate.ToString().StartsWith(_month)
                                       select new MonthlySalesReportDto
                                       {
                                           IntSalesId = s.IntSalesId,
                                           IntDetailsId = sd.IntDetailsId,
                                           IntCustomerId = s.IntCustomerId,
                                           DteSalesDate = s.DteSalesDate,
                                           IntItemId = sd.IntItemId,
                                           NumItemQuantity = sd.NumItemQuantity,
                                           NumUnitPrice = sd.NumUnitPrice
                                       }).ToList();
            msg.Value = _monthlySalesReport;
            return msg;
        }

        public async Task<TMessageHelper<List<DailyPurchaseSalesReportDto>>> GetItemWiseDailyPurchase_VS_SalesReport(string day)
        {
            TMessageHelper<List<DailyPurchaseSalesReportDto>> messageHelper = new TMessageHelper<List<DailyPurchaseSalesReportDto>>();
            messageHelper.statuscode = 200;

            
            
            
            
            var itemList = await _contextR.TblItem.Select(x => new JoinDto
            {
                ItemId = x.IntItemId,
                ItemName = x.StrItemName
            }).ToListAsync();

            // Join with  purchase and purchaseDetails where date = day
            var _purchasesList =await (from p in _contextR.TblPurchase
                        join d2 in _contextR.TblPurchaseDetails on p.IntPurchaseId equals d2.IntPurchaseId into d1
                        from d in d1.DefaultIfEmpty()
                        where p.DtePurchaseDate.ToString() == day
                        group new { d, p }  by new { d.IntItemId } into gd
                        select new JoinDto
                        {
                            ItemId = gd.Key.IntItemId??0,
                            PDteDate = gd.Max(x => x.p.DtePurchaseDate),
                            PQuantity = gd.Sum(x => x.d.NumItemQuantity),
                            PCost = gd.Sum(x => x.d.NumItemQuantity * x.d.NumUnitPrice)
                        }).ToListAsync();

            var _salesList = await (from s in _contextR.TblSales
                        join d2 in _contextR.TblSalesDetails on s.IntSalesId equals d2.IntSalesId into d1
                        from d in d1.DefaultIfEmpty()
                        where s.DteSalesDate.ToString() == day
                        group new { s, d } by new { s.IntSalesId } into gd
                        select new JoinDto
                        {
                            ItemId = gd.Key.IntSalesId,
                            SDteDate = gd.Max(x => x.s.DteSalesDate),
                            SQuantity = gd.Sum(x => x.d.NumItemQuantity),
                            SCost = gd.Sum(x=>x.d.NumItemQuantity*x.d.NumUnitPrice)
                        }).ToListAsync();

            var _itemPurchaseJoin = (from item in itemList
                        join p in _purchasesList on item.ItemId equals p.ItemId into d1
                        from d in d1.DefaultIfEmpty()
                        select new JoinDto
                        {
                            ItemId = item.ItemId,
                            ItemName = item.ItemName,
                            PDteDate = d?.PDteDate ?? DateTime.Parse(day),
                            PQuantity = d?.PQuantity ?? 0,
                            PCost = d?.PCost ?? 0
                        }).ToList();

            var _itemSalesJoin = (from item in _itemPurchaseJoin
                        join s in _salesList on item.ItemId equals s.ItemId into d1
                        from d in d1.DefaultIfEmpty()
                        select new DailyPurchaseSalesReportDto
                        {
                            IntItemId = item.ItemId,
                            StrItemName = item.ItemName,
                            PurchaseDteDate = item.PDteDate,
                            PurchaseQuantity = item.PQuantity,
                            PurchaseCost = item.PCost,
                            SalesDteDate = d?.SDteDate?? DateTime.Parse(day),
                            SalesQuantity = d?.SQuantity ?? 0,
                            SalesCost = d?.SCost ?? 0
                        }).ToList();

            messageHelper.Value = _itemSalesJoin;






            /*var itemList = await _contextR.TblItem.Select(x => new
            {
                itemId = x.IntItemId,
                itemName = x.StrItemName
            }).ToListAsync();

            // Join with  purchase and purchaseDetails where date = day
            var _purchasesList = await _contextR.TblPurchase.Where(x => x.DtePurchaseDate.ToString() == day).Join(
                                 _contextR.TblPurchaseDetails,
                                 x => x.IntPurchaseId,
                                 y => y.IntPurchaseId,
                                (x, y) => new
                                {
                                    PId = x.IntPurchaseId,
                                    Pdte = x.DtePurchaseDate,
                                    PItemid = y.IntItemId,
                                    PQ = y.NumItemQuantity,
                                    PUCost = y.NumUnitPrice
                                }
                                ).GroupBy(P => P.PItemid).Select(
                                g => new
                                {
                                    ItemId = g.Key,
                                    PurchaseDate = g.Max(x => x.Pdte),
                                    PurchaseQuantity = g.Sum(x => x.PQ),
                                    TotalPurchasedCost = g.Sum(x => x.PQ * x.PUCost)
                                }).ToListAsync();

            // Join with  sale and salesDetails where date = day
            var _salesList = await _contextR.TblSales.Where(x => x.DteSalesDate.ToString() == day).Join(
                              _contextR.TblSalesDetails,
                              x => x.IntSalesId,
                              y => y.IntSalesId,
                              (x, y) => new
                              {
                                  SId = x.IntSalesId,
                                  Sdte = x.DteSalesDate,
                                  SItemid = y.IntItemId,
                                  SQ = y.NumItemQuantity,
                                  SUCost = y.NumUnitPrice
                              }
                              ).GroupBy(S => S.SItemid).Select(
                              g => new
                              {
                                  ItemId = g.Key,
                                  SaleDate = g.Max(x => x.Sdte),
                                  SaleQuantity = g.Sum(x => x.SQ),
                                  TotalSaleCost = g.Sum(x => x.SQ * x.SUCost)
                              }).ToListAsync();

            List<DailyPurchaseSalesReportDto> purchaseVsSale = new List<DailyPurchaseSalesReportDto>();

            foreach (var item in itemList)
            {
                var purchase = _purchasesList.Find(x => x.ItemId == item.itemId);
                var sales = _salesList.Find(x => x.ItemId == item.itemId);

                var ob = new DailyPurchaseSalesReportDto
                {
                    IntItemId = item.itemId,
                    StrItemName = item.itemName,
                    PurchaseDteDate = purchase == null ? null : purchase.PurchaseDate,
                    SalesDteDate = purchase == null ? null : sales.SaleDate,
                    PurchaseQuantity = purchase == null ? null : purchase.PurchaseQuantity,
                    SalesQuantity = purchase == null ? null : sales.SaleQuantity,
                    PurchaseCost = purchase == null ? null : purchase.TotalPurchasedCost,
                    SalesCost = purchase == null ? null : sales.TotalSaleCost
                };
                purchaseVsSale.Add(ob);
            }
            messageHelper.Value = purchaseVsSale;*/
            
            
            
            return messageHelper;
        }
        public async Task<TMessageHelper<List<MonthReportDto>>> GETMonthlyPurchaseVSSalesReport()
        {
            TMessageHelper<List<MonthReportDto>> msg = new TMessageHelper<List<MonthReportDto>>();
            msg.Message = "Monthly report";
            msg.statuscode = 200;
            msg.Value = new List<MonthReportDto>();

            var DateMonths = await _contextR.TblSales.Select(x => x.DteSalesDate).ToListAsync();
            DateMonths.AddRange(await _contextR.TblPurchase.Select(x => x.DtePurchaseDate).ToListAsync());

            List<YearMonthDto> yearMonth = new List<YearMonthDto>();
            foreach (var dateMonth in DateMonths)
            {
                if (yearMonth.Where(x => x.Year == dateMonth.Value.Year && x.Month == dateMonth.Value.Month).FirstOrDefault() == null)
                {
                    var ob = new YearMonthDto
                    {
                        Year = dateMonth.Value.Year,
                        Month = dateMonth.Value.Month,
                    };
                    yearMonth.Add(ob);
                }
            }
            foreach (var _yearmonth in yearMonth)
            {
                var _purchasesList = await _contextR.TblPurchase.Where(x => x.DtePurchaseDate.Value.Year == _yearmonth.Year && x.DtePurchaseDate.Value.Month == _yearmonth.Month).Join(
                                 _contextR.TblPurchaseDetails,
                                 x => x.IntPurchaseId,
                                 y => y.IntPurchaseId,
                                (x, y) => new
                                {
                                    PQ = y.NumItemQuantity,
                                    PUCost = y.NumUnitPrice
                                }
                                ).ToListAsync();
                var _totalPurchaseAmount = _purchasesList.Sum(x => x.PQ * x.PUCost);
                var _salesList = await _contextR.TblSales.Where(x => x.DteSalesDate.Value.Year == _yearmonth.Year && x.DteSalesDate.Value.Month == _yearmonth.Month).Join(
                                 _contextR.TblSalesDetails,
                                 x => x.IntSalesId,
                                 y => y.IntSalesId,
                                (x, y) => new
                                {
                                    SQ = y.NumItemQuantity,
                                    SUCost = y.NumUnitPrice
                                }
                                ).ToListAsync();
                var _totalSalesAmount = _salesList.Sum(x => x.SQ * x.SUCost);
                var ob = new MonthReportDto
                {
                    monthname = _yearmonth.GetMonthName(),
                    year = _yearmonth.Year,
                    totalPurchaseAmount = _totalPurchaseAmount,
                    totalSalesAmount = _totalSalesAmount,
                    status = (_totalPurchaseAmount > _totalSalesAmount ? "Loss" : "Profit")
                };
                msg.Value.Add(ob);
            }



            /*var DateMonths = await (from dt in _contextR.TblPurchase
                                    group dt by dt.DtePurchaseDate into gdt
                                    select new YearMonthDto
                                    {
                                        Year =  gdt.Key.Value.Year,
                                        Month = gdt.Key.Value.Month,
                                    }).ToListAsync();

                DateMonths.AddRange( await (from dt in _contextR.TblSales
                                    group dt by dt.DteSalesDate into gdt
                                    where DateMonths.Where(x => x.Year == gdt.Key.Value.Year && x.Month == gdt.Key.Value.Month).FirstOrDefault() == null
                                    select new YearMonthDto
                                    {
                                        Year = gdt.Key.Value.Year,
                                        Month = gdt.Key.Value.Month,
                                    }).ToListAsync());


            foreach (var _yearmonth in DateMonths)
            {
                var _purchasesList = await _contextR.TblPurchase.Where(x => x.DtePurchaseDate.Value.Year==_yearmonth.Year && x.DtePurchaseDate.Value.Month==_yearmonth.Month).Join(
                                 _contextR.TblPurchaseDetails,
                                 x => x.IntPurchaseId,
                                 y => y.IntPurchaseId,
                                (x, y) => new
                                {
                                    PQ = y.NumItemQuantity,
                                    PUCost = y.NumUnitPrice
                                }
                                ).ToListAsync();
                var _totalPurchaseAmount = _purchasesList.Sum(x=>x.PQ*x.PUCost);
                var _salesList = await _contextR.TblSales.Where(x => x.DteSalesDate.Value.Year == _yearmonth.Year && x.DteSalesDate.Value.Month == _yearmonth.Month).Join(
                                 _contextR.TblSalesDetails,
                                 x => x.IntSalesId,
                                 y => y.IntSalesId,
                                (x, y) => new
                                {
                                    SQ = y.NumItemQuantity,
                                    SUCost = y.NumUnitPrice
                                }
                                ).ToListAsync();
                var _totalSalesAmount = _salesList.Sum(x => x.SQ * x.SUCost);
                var ob = new MonthReportDto
                {
                    monthname = _yearmonth.GetMonthName(),
                    year = _yearmonth.Year,
                    totalPurchaseAmount = _totalPurchaseAmount,
                    totalSalesAmount = _totalSalesAmount,
                    status = (_totalPurchaseAmount > _totalSalesAmount ? "Loss" : "Profit")
                };
                msg.Value.Add(ob);
            }*/

            return msg;
        }
        public async Task<TMessageHelper<List<PurchaseDetailsDto>>> PracticeGroupBy()
        {
            TMessageHelper<List<PurchaseDetailsDto>> msg = new TMessageHelper<List<PurchaseDetailsDto>>();
            msg.Message = "Monthly report";
            msg.statuscode = 200;
            msg.Value = new List<PurchaseDetailsDto>();

            var res = await (from pd in _contextR.TblPurchaseDetails
                       group pd by pd.IntItemId into gpd
                       select new PurchaseDetailsDto
                       {
                           IntPurchaseId = gpd.Max(x=>x.IntPurchaseId),
                           IntItemId = gpd.Key??0,
                           NumUnitPrice = gpd.Sum(x=>x.NumItemQuantity*x.NumUnitPrice)
                       }
                       ).ToListAsync();
            msg.Value.AddRange(res);
            return msg;
        }

        public async Task<TMessageHelper<string>> AuthenticateUser(UserDto user)
        {
            TMessageHelper<string> messageHelper = new TMessageHelper<string>();
            messageHelper.statuscode = 200;
            messageHelper.Message = "Faild";

            var res = await (from u in _contextR.TblUser where user.UserName == u.UserName && user.PassWord == u.PassWord select u).FirstOrDefaultAsync();
            if (res == null)
            {
                messageHelper.Message = "WRONG USER NAME OR PASSWORD";
            }
            else
            {
                messageHelper.Message = "Sucessfully LogIn";
                user.UserId = res.UserId;
                messageHelper.Value = _JwtToken.GenerateToken(user);
            }
            return messageHelper;
        }
    }
}
