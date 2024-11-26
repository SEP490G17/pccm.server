using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Persistence;

namespace Application.Handler.Bill
{
    public class CreateBillAll
    {
        public class Query : IRequest<Result<ActionResult>>
        {
            public int bookingId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ActionResult>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }

            public async Task<Result<ActionResult>> Handle(Query request, CancellationToken cancellationToken)
            {
                var bookingId = request.bookingId;
                List<ProductBillDto> productBillDtos = new List<ProductBillDto>();
                List<ServiceBillDto> serviceBillDtos = new List<ServiceBillDto>();
                var booking = await _context.Bookings
                    .Include(b => b.Court)
                    .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.BookingId == bookingId && o.Payment.Status == Domain.Enum.PaymentStatus.Success, cancellationToken);
                var court = await _context.Courts
                    .FirstOrDefaultAsync(b => b.Id == booking.Court.Id, cancellationToken);
                var courtCluster = await _context.CourtClusters
                    .FirstOrDefaultAsync(b => b.Id == court.CourtClusterId, cancellationToken);

                if (order != null)
                {
                    foreach (var item in order.OrderDetails)
                    {
                        if (item.ProductId != null && item.ServiceId == null)
                        {
                            productBillDtos.Add(_mapper.Map<ProductBillDto>(item));
                        }
                        else
                        {
                            serviceBillDtos.Add(_mapper.Map<ServiceBillDto>(item));
                        }
                    }
                }

                BillDto billData = new BillDto()
                {
                    booking = _mapper.Map<BookingBillDto>(booking),
                    courtCluster = _mapper.Map<CourtClusterBillDto>(courtCluster),
                    products = productBillDtos,
                    services = serviceBillDtos,
                };

                var pdfBytes = GeneratePdf(billData);

                return Result<ActionResult>.Success(new FileContentResult(pdfBytes, "application/pdf"));
            }

            private byte[] GeneratePdf(BillDto data)
            {
                using (var document = new PdfDocument())
                {
                    var page = document.AddPage();
                    page.Width = 105 * 2.83465;

                    var gfx = XGraphics.FromPdfPage(page);
                    var font = new XFont("Arial", 8, XFontStyleEx.Regular);
                    var fontBold = new XFont("Arial", 8, XFontStyleEx.Bold);

                    int yPos = 10;
                    const int margin = 10;
                    string filePath = "../Infrastructure/Images/logo.png";
                    XImage image = XImage.FromFile(filePath);
                    double xPos = (gfx.PageSize.Width - 120) / 2;
                    gfx.DrawImage(image, xPos, yPos, 120, 50);
                    yPos += 60;
                    gfx.DrawString(data.courtCluster.CourtClusterName, fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopCenter);
                    yPos += 20;

                    gfx.DrawString($"{data.courtCluster.Address}, {data.courtCluster.WardName}, {data.courtCluster.ProvinceName}", font, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopCenter);
                    yPos += 20;

                    gfx.DrawString($"HÓA ĐƠN {data.booking.CourtName.ToUpper()}", fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopCenter);
                    yPos += 20;

                    gfx.DrawString($"Giờ bắt đầu:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{data.booking.StartTime:HH:mm dd/MM/yyyy}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                    yPos += 20;
                    gfx.DrawString($"Giờ kết thúc:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{data.booking.EndTime:HH:mm dd/MM/yyyy}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                    yPos += 20;
                    gfx.DrawString("Hàng hóa", fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                    yPos += 20;

                    DrawCell(gfx, "Tên sản phẩm", fontBold, margin, yPos, (page.Width - 2 * margin) * 0.4, 20); // 40% width
                    DrawCell(gfx, "SL", fontBold, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20); // 20% width
                    DrawCell(gfx, "Giá", fontBold, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20); // 20% width
                    DrawCell(gfx, "Tổng", fontBold, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20); // 20% width
                    yPos += 20;

                    foreach (var item in data.products)
                    {
                        DrawCell(gfx, item.ProductName, font, margin, yPos, (page.Width - 2 * margin) * 0.4, 20);
                        DrawCell(gfx, item.Quantity.ToString(), font, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, item.Price.ToString("N0"), font, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, (item.Price * item.Quantity).ToString("N0"), font, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        yPos += 20;
                    }
                    yPos += 20;
                    gfx.DrawString("Dịch vụ", fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                    yPos += 20;

                    DrawCell(gfx, "Tên dịch vụ", fontBold, margin, yPos, (page.Width - 2 * margin) * 0.4, 20);
                    DrawCell(gfx, "SL", fontBold, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                    DrawCell(gfx, "Giá", fontBold, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                    DrawCell(gfx, "Tổng", fontBold, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                    yPos += 20;

                    foreach (var item in data.services)
                    {
                        DrawCell(gfx, item.ServiceName, font, margin, yPos, (page.Width - 2 * margin) * 0.4, 20);
                        DrawCell(gfx, item.Quantity.ToString(), font, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, item.Price.ToString("N0"), font, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, (item.Price * item.Quantity).ToString("N0"), font, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        yPos += 20;
                    }
                    yPos += 25;
                    gfx.DrawString($"Tổng hàng hóa:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{data.products.Sum(p => p.Price * p.Quantity):N0}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                    yPos += 20;

                    gfx.DrawString($"Tổng dịch vụ:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{data.services.Sum(s => s.Price * s.Quantity):N0}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                    yPos += 20;
                    gfx.DrawString($"Tổng tiền giờ:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{data.booking.TotalPrice:N0}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                    yPos += 20;
                    gfx.DrawString($"Tổng thanh toán:", fontBold, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{(data.products.Sum(p => p.Price * p.Quantity) + data.services.Sum(s => s.Price * s.Quantity) + data.booking.TotalPrice):N0}", fontBold, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                    yPos += 20;

                    using (var ms = new MemoryStream())
                    {
                        document.Save(ms, false);
                        return ms.ToArray();
                    }
                }
            }

            private void DrawCell(XGraphics gfx, string text, XFont font, double x, double y, double width, double height)
            {
                gfx.DrawRectangle(XPens.Black, x, y, width, height);
                gfx.DrawString(text, font, XBrushes.Black, new XRect(x + 2, y + 2, width - 4, height - 4), XStringFormats.CenterLeft);
            }
        }
    }
}
