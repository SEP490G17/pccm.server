using Application.Core;
using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
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

                // Lấy thông tin đặt sân
                var booking = await _context.Bookings
                    .Include(b => b.Court)
                    .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
                if (booking == null)
                {
                    return Result<ActionResult>.Failure("Booking not found.");
                }

                // Lấy thông tin đơn hàng
                var orders = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                    .Include(o => o.Payment)
                    .Where(o => o.BookingId == bookingId && o.Payment.Status == Domain.Enum.PaymentStatus.Success)
                    .ToListAsync(cancellationToken);

                // Lấy thông tin sân
                var court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == booking.Court.Id, cancellationToken);
                if (court == null)
                {
                    return Result<ActionResult>.Failure("Court not found.");
                }

                // Lấy thông tin cụm sân
                var courtCluster = await _context.CourtClusters.FirstOrDefaultAsync(cc => cc.Id == court.CourtClusterId, cancellationToken);
                if (courtCluster == null)
                {
                    return Result<ActionResult>.Failure("Court cluster not found.");
                }

                // Xử lý đơn hàng thành sản phẩm và dịch vụ
                foreach (var order in orders)
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        if (orderDetail.ProductId != null)
                        {
                            var product = productBillDtos.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                            if (product != null)
                            {
                                product.Quantity += (decimal)orderDetail.Quantity;
                            }
                            else
                            {
                                productBillDtos.Add(_mapper.Map<ProductBillDto>(orderDetail));
                            }
                        }
                        else if (orderDetail.ServiceId != null)
                        {
                            var service = serviceBillDtos.FirstOrDefault(s => s.ServiceId == orderDetail.ServiceId);
                            if (service != null)
                            {
                                service.Quantity += (decimal)orderDetail.Quantity;
                            }
                            else
                            {
                                serviceBillDtos.Add(_mapper.Map<ServiceBillDto>(orderDetail));
                            }
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
                try
                {
                    using (var document = new PdfDocument())
                    {
                        var page = document.AddPage();
                        page.Size = PdfSharpCore.PageSize.A6;
                        var gfx = XGraphics.FromPdfPage(page);

                        XFont font = new XFont("Calibri", 8, XFontStyle.Regular);
                        XFont fontBold = new XFont("Calibri", 8, XFontStyle.Bold);

                        const int margin = 10;
                        int yPos = margin;
                        int pageHeight = (int)page.Height;
                        int maxHeight = pageHeight - 2 * margin;

                        void EnsureSpace(int height)
                        {
                            if (yPos + height > maxHeight)
                            {
                                page = document.AddPage();
                                page.Size = PdfSharpCore.PageSize.A6;
                                gfx = XGraphics.FromPdfPage(page);
                                yPos = margin;
                            }
                            else yPos += 20;
                        }
                        string imageLogobase64 = "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";

                        byte[] imageBytes = Convert.FromBase64String(imageLogobase64);

                        using (var ms = new MemoryStream(imageBytes))
                        {
                            XImage imagelogo = XImage.FromStream(() => new MemoryStream(imageBytes));
                            double xPos = (gfx.PageSize.Width - 120) / 2;
                            gfx.DrawImage(imagelogo, xPos, yPos, 120, 50);
                        }
                        yPos += 60;

                        gfx.DrawString(data.courtCluster.CourtClusterName, fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopCenter);
                        EnsureSpace(15);

                        gfx.DrawString($"{data.courtCluster.Address}, {data.courtCluster.WardName}, {data.courtCluster.ProvinceName}", font, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopCenter);
                        EnsureSpace(15);

                        gfx.DrawString($"HÓA ĐƠN {data.booking.CourtName.ToUpper()}", fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopCenter);
                        EnsureSpace(15);


                        gfx.DrawString($"Giờ bắt đầu:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{data.booking.StartTime:HH:mm dd/MM/yyyy}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                        EnsureSpace(15);

                        gfx.DrawString($"Giờ kết thúc:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{data.booking.EndTime:HH:mm dd/MM/yyyy}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                        EnsureSpace(15);


                        gfx.DrawString("Hàng hóa", fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                        EnsureSpace(15);


                        DrawCell(gfx, "Tên sản phẩm", fontBold, margin, yPos, (page.Width - 2 * margin) * 0.4, 20);
                        DrawCell(gfx, "SL", fontBold, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, "Giá", fontBold, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, "Tổng", fontBold, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        EnsureSpace(15);


                        foreach (var item in data.products)
                        {
                            if (yPos + 20 > maxHeight)
                            {
                                page = document.AddPage();
                                page.Size = PdfSharpCore.PageSize.A6;
                                gfx = XGraphics.FromPdfPage(page);
                                yPos = margin;
                            }

                            DrawCell(gfx, item.ProductName, font, margin, yPos, (page.Width - 2 * margin) * 0.4, 20);
                            DrawCell(gfx, item.Quantity.ToString(), font, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                            DrawCell(gfx, item.Price.ToString("N0"), font, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                            DrawCell(gfx, (item.Price * item.Quantity).ToString("N0"), font, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                            if (yPos + 20 > maxHeight)
                            {
                                page = document.AddPage();
                                page.Size = PdfSharpCore.PageSize.A6;
                                gfx = XGraphics.FromPdfPage(page);
                                yPos = margin;
                            }
                            else yPos += 20;
                        }

                        EnsureSpace(15);

                        gfx.DrawString("Dịch vụ", fontBold, XBrushes.Black, new XRect(margin, yPos, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                        EnsureSpace(15);


                        DrawCell(gfx, "Tên dịch vụ", fontBold, margin, yPos, (page.Width - 2 * margin) * 0.4, 20);
                        DrawCell(gfx, "SL", fontBold, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, "Giá", fontBold, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        DrawCell(gfx, "Tổng", fontBold, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                        EnsureSpace(15);


                        foreach (var item in data.services)
                        {
                            if (yPos + 20 > maxHeight)
                            {
                                page = document.AddPage();
                                page.Size = PdfSharpCore.PageSize.A6;
                                gfx = XGraphics.FromPdfPage(page);
                                yPos = margin;
                            }

                            DrawCell(gfx, item.ServiceName, font, margin, yPos, (page.Width - 2 * margin) * 0.4, 20);
                            DrawCell(gfx, item.Quantity.ToString(), font, (page.Width - 2 * margin) * 0.4 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                            DrawCell(gfx, item.Price.ToString("N0"), font, (page.Width - 2 * margin) * 0.6 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                            DrawCell(gfx, (item.Price * item.Quantity).ToString("N0"), font, (page.Width - 2 * margin) * 0.8 + margin, yPos, (page.Width - 2 * margin) * 0.2, 20);
                            if (yPos + 20 > maxHeight)
                            {
                                page = document.AddPage();
                                page.Size = PdfSharpCore.PageSize.A6;
                                gfx = XGraphics.FromPdfPage(page);
                                yPos = margin;
                            }
                            else yPos += 20;
                        }

                        EnsureSpace(15);

                        gfx.DrawString($"Tổng hàng hóa:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{data.products.Sum(s => s.Price * s.Quantity):N0}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                        EnsureSpace(15);

                        gfx.DrawString($"Tổng dịch vụ:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{data.services.Sum(s => s.Price * s.Quantity):N0}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                        EnsureSpace(15);


                        gfx.DrawString($"Tổng tiền giờ:", font, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{data.booking.TotalPrice:N0}", font, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);
                        EnsureSpace(15);


                        gfx.DrawString($"Tổng thanh toán:", fontBold, XBrushes.Black, new XRect(margin, yPos, (page.Width - 2 * margin) * 0.8, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{(data.products.Sum(p => p.Price * p.Quantity) + data.services.Sum(s => s.Price * s.Quantity) + data.booking.TotalPrice):N0}", fontBold, XBrushes.Black, new XRect(page.Width - 60 - margin, yPos, 60, 20), XStringFormats.TopRight);

                        using (var ms = new MemoryStream())
                        {
                            document.Save(ms, false);
                            return ms.ToArray();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error generating PDF: " + ex.Message);
                    throw;
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
