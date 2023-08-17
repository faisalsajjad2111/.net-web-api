using CodeCafe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http.Headers;
using System.Web;

namespace CodeCafe.Controllers
{
    public class BillController : ApiController
    {
        mysqliEntities db = new mysqliEntities();
        Response response = new Response();
        private string pdfPath = "E:\\";
        [HttpPost Route("generateReport")]
        //  [CustomAuthenticationFilter]
        public HttpResponseMessage GenerateReport([FromBody] Bill bill)
        {
            try
            {
                //var token = Request.Headers.GetValues("authorization").First();
                //TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueId = ticks.ToString() + '-' + guid;
                //  bill.createdBy = tokenClaim.Email;
                bill.uuid = uniqueId;
                db.Bills.Add(bill);
                db.SaveChanges();
                Get(bill);
                return Request.CreateResponse(HttpStatusCode.OK, new { uuid = bill.uuid });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        private void Get(Bill bill)
        {
            try
            {
                dynamic productDetails = JsonConvert.DeserializeObject(bill.productDetails);
                var todayDate = "Date:" + Convert.ToDateTime(DateTime.Today).ToString("MM/dd/yyyy");
                PdfWriter writer = new PdfWriter(pdfPath + bill.uuid + ".pdf");
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                Paragraph header = new Paragraph("Cafe management system")
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(25);
                document.Add(header);
                Paragraph newline = new Paragraph(new Text("\n"));
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);
                Paragraph customerDetails = new Paragraph("Name" + bill.name + "\nEmail" + bill.email + "nContact Number" + bill.contactNumber + "\nPayment Method" + bill.paymentMethod);
                document.Add(customerDetails);
                Table table = new Table(5, false);
                table.SetWidth(new UnitValue(UnitValue.PERCENT, 100));
                Cell headerName = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Name"));
                Cell headerCategory = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetBold()
                   .Add(new Paragraph("Category"));
                Cell headerQuantity = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetBold()
                   .Add(new Paragraph("Quantity"));
                Cell headerPrice = new Cell(1, 1)
                  .SetTextAlignment(TextAlignment.CENTER)
                  .SetBold()
                  .Add(new Paragraph("Price"));
                Cell headerSubtotal = new Cell(1, 1)
                 .SetTextAlignment(TextAlignment.CENTER)
                 .SetBold()
                 .Add(new Paragraph("Sub total"));
                table.AddCell(headerName);
                table.AddCell(headerCategory);
                table.AddCell(headerQuantity);
                table.AddCell(headerPrice);
                table.AddCell(headerSubtotal);
                foreach (JObject product in productDetails)
                {
                    Cell nameCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["name"].ToString()));
                    Cell categoryCell = new Cell(1, 1)
                      .SetTextAlignment(TextAlignment.CENTER)
                      .Add(new Paragraph(product["category"].ToString()));
                    Cell quantityCell = new Cell(1, 1)
                      .SetTextAlignment(TextAlignment.CENTER)
                      .Add(new Paragraph(product["quantity"].ToString()));
                    Cell priceCell = new Cell(1, 1)
                     .SetTextAlignment(TextAlignment.CENTER)
                     .Add(new Paragraph(product["price"].ToString()));
                    Cell totalCell = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph(product["total"].ToString()));
                    table.AddCell(nameCell);
                    table.AddCell(categoryCell);
                    table.AddCell(quantityCell);
                    table.AddCell(priceCell);
                    table.AddCell(totalCell);

                }
                document.Add(table);
                Paragraph last = new Paragraph("Total" + bill.totalAmount + "\nThank you for visiting");
                document.Add(last);
                document.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }
        [HttpPost Route("getPdf")]
        public HttpResponseMessage GetPdf([FromBody] Bill bill)
        {
            try
            {
                if (bill.name != null)
                {
                    Get(bill);
                }
                HttpResponseMessage respons = Request.CreateResponse(HttpStatusCode.OK);
                string filePath = pdfPath + bill.uuid.ToString() + ".pdf";
                byte[] bytes = File.ReadAllBytes(filePath);
                respons.Content = new ByteArrayContent(bytes);
                respons.Content.Headers.ContentLength = bytes.LongLength;
                respons.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                respons.Content.Headers.ContentDisposition.FileName = bill.uuid.ToString() + ".pdf";
                respons.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(bill.uuid.ToString() + ".pdf"));
                return respons;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);





            }


        }
        [HttpPost Route("deleteBill/{id}")]
        //[CustomAuthenticationFilter]
        public HttpResponseMessage DeleteBill(int id)
        {
            try
            {
                Bill billObj = db.Bills.Find(id);
                if (billObj == null)
                {
                    response.message = "Bill id not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                db.Bills.Remove(billObj);
                db.SaveChanges();
                response.message = "Bill deleted succesfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
                
            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }










    }
}