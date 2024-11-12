using System.Data;
using Application.Core;
using Application.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClosedXML.Excel;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Statistics
{
    public class ExportExcel
    {
        public class Query : IRequest<Result<ActionResult>>
        {
            public DateTime Date { get; set; }
            public int CourtClusterId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ActionResult>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(DataContext context, IMapper mapper, IMediator mediator)
            {
                _context = context;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Result<ActionResult>> Handle(Query request, CancellationToken cancellationToken)
            {
                var statisticQuery = new StatisticCluster.Query
                {
                    Date = request.Date,
                    CourtClusterId = request.CourtClusterId
                };

                var courtCluster = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == request.CourtClusterId);

                var statisticResult = await _mediator.Send(statisticQuery, cancellationToken);

                if (statisticResult.IsSuccess)
                {
                    var statistics = statisticResult.Value;

                    var workbook = new XLWorkbook();
                    var worksheet = workbook.AddWorksheet("Statistics");

                    worksheet.Column(1).Width = 10;  // STT
                    worksheet.Column(2).Width = 25;  // Chi tiêu
                    worksheet.Column(3).Width = 25;  // Số lượng
                    worksheet.Column(4).Width = 20;  // Thành tiền

                    // Headers
                    worksheet.Cell(1, 1).Value = $"BÁO CÁO LỢI NHUẬN {request.Date.Month}/{request.Date.Year}";
                    worksheet.Cell(2, 1).Value = $"Cụm sân: {courtCluster.CourtClusterName}";
                    worksheet.Range(1, 1, 1, 4).Merge().Style.Font.SetBold(true).Font.FontSize = 14;
                    worksheet.Range(1, 1, 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(2, 1, 2, 4).Merge().Style.Font.SetBold(true);

                    worksheet.Cell(4, 1).Value = "STT";
                    worksheet.Cell(4, 2).Value = "Chi tiêu";
                    worksheet.Cell(4, 3).Value = "Số lượng";
                    worksheet.Cell(4, 4).Value = "Thành tiền";
                    worksheet.Range(4, 1, 4, 4).Style.Font.SetBold(true);

                    // Section A: Doanh thu sân
                    worksheet.Cell(5, 1).Value = "A.";
                    worksheet.Cell(5, 2).Value = "Doanh thu sân";
                    worksheet.Range(5, 2, 5, 4).Merge().Style.Font.SetBold(true);

                    int row = 6;

                    foreach (var detail in statistics.BookingDetails)
                    {
                        worksheet.Cell(row, 2).Value = detail.CourtName;
                        worksheet.Cell(row, 3).Value = detail.HoursBooked;
                        worksheet.Cell(row, 4).Value = detail.TotalPrice;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        row++;
                    }

                    if (statistics.BookingDetails.Count > 0)
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).FormulaA1 = $"SUM(D6:D{row - 1})";
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }
                    else
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).Value = 0;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }
                    row++;

                    // Section B: Doanh thu bán hàng
                    worksheet.Cell(row, 1).Value = "B.";
                    worksheet.Cell(row, 2).Value = "Doanh thu bán hàng";
                    worksheet.Range(row, 2, row, 4).Merge().Style.Font.SetBold(true);
                    row++;

                    foreach (var productDetail in statistics.OrderProductDetails)
                    {
                        worksheet.Cell(row, 2).Value = productDetail.ProductName;
                        worksheet.Cell(row, 3).Value = productDetail.Quantity;
                        worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell(row, 4).Value = productDetail.TotalPrice;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        row++;
                    }

                    if (statistics.OrderProductDetails.Count > 0)
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).FormulaA1 = $"SUM(D{statistics.BookingDetails.Count + 8}:D{row - 1})";
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }
                    else
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).Value = 0;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    row++;

                    // Section C: Doanh thu dịch vụ
                    worksheet.Cell(row, 1).Value = "C.";
                    worksheet.Cell(row, 2).Value = "Doanh thu dịch vụ";
                    worksheet.Range(row, 2, row, 4).Merge().Style.Font.SetBold(true);
                    row++;

                    foreach (var serviceDetail in statistics.OrderServiceDetails)
                    {
                        worksheet.Cell(row, 2).Value = serviceDetail.ServiceName;
                        worksheet.Cell(row, 3).Value = serviceDetail.Quantity;
                        worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell(row, 4).Value = serviceDetail.TotalPrice;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        row++;
                    }

                    if (statistics.OrderServiceDetails.Count > 0)
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).FormulaA1 = $"SUM(D{statistics.BookingDetails.Count + statistics.OrderProductDetails.Count + 10}:D{row - 1})";
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }
                    else
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).Value = 0;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    row++;

                    // Section D: Chi phí & Quản lý
                    worksheet.Cell(row, 1).Value = "D.";
                    worksheet.Cell(row, 2).Value = "Chi phí & Quản lý";
                    worksheet.Range(row, 2, row, 4).Merge().Style.Font.SetBold(true);
                    row++;

                    foreach (var expense in statistics.ExpenseDetails)
                    {
                        worksheet.Cell(row, 2).Value = expense.ExpenseName;
                        worksheet.Cell(row, 4).Value = expense.TotalPrice;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        row++;
                    }

                    if (statistics.ExpenseDetails.Count > 0)
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).FormulaA1 = $"SUM(D{statistics.BookingDetails.Count + statistics.OrderProductDetails.Count + statistics.OrderServiceDetails.Count + 12}:D{row - 1})";
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }
                    else
                    {
                        worksheet.Cell(row, 2).Value = "Tổng";
                        worksheet.Cell(row, 4).Value = 0;
                        worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    row++;
                    row++;
                    // Section E: Lợi nhuận
                    worksheet.Cell(row, 1).Value = "E.";
                    worksheet.Cell(row, 2).Value = "Lợi nhuận";
                    worksheet.Range(row, 2, row, 3).Merge().Style.Font.SetBold(true);
                    worksheet.Cell(row, 4).FormulaA1 = $"(D{statistics.BookingDetails.Count + 6} + D{statistics.BookingDetails.Count + statistics.OrderProductDetails.Count + 8} + D{statistics.BookingDetails.Count + statistics.OrderProductDetails.Count + statistics.OrderServiceDetails.Count + 10} - D{row - 2})";
                    worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Cell(row, 4).Style.Font.SetBold(true);
                    row++;


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        var fileContent = stream.ToArray();

                        return Result<ActionResult>.Success(new FileContentResult(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            FileDownloadName = "ClusterStatistics.xlsx"
                        });
                    }
                }

                return Result<ActionResult>.Failure("Không thể lấy dữ liệu thống kê.");
            }
        }
    }
}
