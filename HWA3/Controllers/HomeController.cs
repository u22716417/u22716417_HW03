using HWA3.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Linq;

namespace HWA3.Controllers
{
    public class HomeController : Controller
    {
        static List<PopularBooksVM> _books = new List<PopularBooksVM>();
        private LibraryEntities db = new LibraryEntities();
        int numberElementsPerPage = 10;
        static StudentBookVM  studentBookVM = new StudentBookVM();
        static MaintainVm maintainVm = new MaintainVm();
        static List<PopularBooksVM> top10Books = new List<PopularBooksVM>();
        static List<ArchiveDetails> archiveDetails = new List<ArchiveDetails>();

        public ActionResult Index(int studentpageIndex =1, int bookIndex = 1)
        {

            studentBookVM.students = (PagedList<student>)db.students.ToList().ToPagedList(studentpageIndex, numberElementsPerPage);
            studentBookVM.books = (PagedList<book>)db.books.ToList().ToPagedList(bookIndex, numberElementsPerPage);

            return View(studentBookVM);
        }

        public ActionResult Maintain(int authorsIndex = 1, int typeIndex = 1, int borrowsIndex=1)
        {

            if(authorsIndex == 0)
                authorsIndex = 1;
            if(typeIndex == 0)
                typeIndex = 1;
            if(borrowsIndex == 0)
                borrowsIndex = 1;

            maintainVm.authorList = (PagedList<author>)db.authors.ToList().ToPagedList(authorsIndex, numberElementsPerPage);
            maintainVm.typeList = (PagedList<type>)db.types.ToList().ToPagedList(typeIndex, numberElementsPerPage);
            maintainVm.borrowsList = (PagedList<borrow>)db.borrows.ToList().ToPagedList(borrowsIndex, numberElementsPerPage);

            return View(maintainVm);
        }

        public ActionResult ReportView()
        {
            top10Books = new List<PopularBooksVM>();
            _books = new List<PopularBooksVM>();
            var books = db.books.ToList();
            foreach(var book in books)
            {
                PopularBooksVM bookVM = new PopularBooksVM();
                bookVM.Title = book.name;
                bookVM.BookCount = db.borrows.Count(x => x.bookId == book.bookId);
                _books.Add(bookVM);
            }
            top10Books.AddRange(_books.OrderByDescending(b => b.BookCount)
                             .Take(10)
                             .ToList());
            ReportingVM reportingVM = new ReportingVM();
            reportingVM.popularBooks = top10Books;
            reportingVM.ArchiveDetails = archiveDetails;
            return View(reportingVM);
        }


        public ActionResult Export(string fileName, string filetype, string desciption)
        {
            var chart = GeneratePieChart();

            if (filetype.Equals("pdf"))
            {
                // Export as a PDF
                ExportChartAsPDF(chart, Server.MapPath("~/GeneratedFiles/"+fileName+ ".pdf"));

                archiveDetails.Add(new ArchiveDetails
                {
                   
                    Description = desciption,
                    FileName = fileName,
                    FileType = filetype,
                    Location = Server.MapPath("~/GeneratedFiles/" + fileName + ".pdf")

                });
            }
            if (filetype.Equals("png"))
            {
                // Export as an image
                ExportChartAsImage(chart, Server.MapPath("~/GeneratedFiles/" + fileName + ".png"));

                archiveDetails.Add(new ArchiveDetails
                {
                   
                    Description = desciption,
                    FileName = fileName,
                    FileType = filetype,
                    Location = Server.MapPath("~/GeneratedFiles/" + fileName + ".png")

                });
            }
            if (filetype.Equals("csv"))
            {
                //Export as an Excel file
                FileInfo file = new FileInfo(Server.MapPath("~/GeneratedFiles/" + fileName + ".csv"));


                archiveDetails.Add(new ArchiveDetails
                {
                   
                    Description = desciption,
                    FileName = fileName,
                    FileType = filetype,
                    Location = Server.MapPath("~/GeneratedFiles/" + fileName + ".csv")

                });

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Books");

                    // Set headers
                    worksheet.Cells[1, 1].Value = "Title";
                    worksheet.Cells[1, 2].Value = "Borrow Count";

                    // Format headers
                    using (var range = worksheet.Cells[1, 1, 1, 3])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Populate data
                    int row = 2;
                    foreach (var book in top10Books)
                    {
                        worksheet.Cells[row, 1].Value = book.Title;
                        worksheet.Cells[row, 2].Value = book.BookCount;
                        row++;
                    }

                    package.Save();
                }
            }
            return RedirectToAction("ReportView");
        }

        public FileResult DownloadFile(string id, string type)
        {
          
            string filePath = Server.MapPath("~/GeneratedFiles/" + id + "."+ type);
            string contentType = "application/octet-stream"; 

            return File(filePath, contentType, id+"."+type);
        }
        public ActionResult DeleteFile(string id)
        {
            archiveDetails.RemoveAll(item => item.FileName == id);
            return RedirectToAction("ReportView");
        }
        public Bitmap GeneratePieChart()
        {
            int dpi = 1200; 

            var chartBitmap = new Bitmap(400, 400);
            var chartGraphics = Graphics.FromImage(chartBitmap);
            chartBitmap.SetResolution(dpi, dpi); 

            chartGraphics.Clear(Color.White);

            // Data defined for the pie chart
            var values = top10Books.Select(s=>s.BookCount).ToArray();
            var colors = new[] { Color.Red, Color.Blue, Color.Green,Color.Orange, Color.Purple, Color.Pink,Color.Gray,Color.Brown,
            Color.Cyan,
            Color.Magenta };
            var labels = top10Books.Select(s => s.Title).ToArray();

            var total = values.Sum();
            var startAngle = 0.0f;

            for (int i = 0; i < values.Length; i++)
            {
                var sweepAngle = (float)(360.0 * values[i] / total);
                using (var brush = new SolidBrush(colors[i]))
                {
                    chartGraphics.FillPie(brush, 0, 0, 400, 400, startAngle, sweepAngle);
                }

                // Calculate the position for the label
                var labelX = 200 + 100 * Math.Cos((startAngle + sweepAngle / 2) * Math.PI / 180);
                var labelY = 200 + 100 * Math.Sin((startAngle + sweepAngle / 2) * Math.PI / 180);

                using (var labelFont = new System.Drawing.Font("Arial", 8))
                {
                    var labelSize = chartGraphics.MeasureString(labels[i], labelFont);
                    var rotationAngle = startAngle + sweepAngle / 2;

                    // Rotate the label inline with the slice
                    chartGraphics.TranslateTransform((float)labelX, (float)labelY);
                    chartGraphics.RotateTransform(rotationAngle);
                    chartGraphics.DrawString(labels[i], labelFont, Brushes.Black, -labelSize.Width / 2, -labelSize.Height / 2, StringFormat.GenericDefault);
                    chartGraphics.ResetTransform();
                }

                startAngle += sweepAngle;
            }

            return chartBitmap;
        }


        public void ExportChartAsImage(Bitmap chart, string imagePath)
        {
            if (chart != null && !string.IsNullOrEmpty(imagePath))
            {
                chart.Save(imagePath, ImageFormat.Png);

                // Dispose of the Bitmap object to release resources
                chart.Dispose();
            }
        }

        public void ExportChartAsPDF(Bitmap chart, string pdfPath)
        {
            using (var document = new Document(PageSize.A4, 50, 50, 50, 50))
            {
                PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
                document.Open();
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(chart, ImageFormat.Png);
                document.Add(image);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}