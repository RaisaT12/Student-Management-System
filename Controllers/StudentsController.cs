using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OfficeOpenXml;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext dbContext;
        public StudentsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Add(int regnumber, string fname, string lname, string address, DateTime date)
        {
            Student student = new Student();
            student.Registration_Num = regnumber;
            student.FirstName = fname;
            student.LastName = lname;
            student.Address = address;
            student.CreatedDate = date;
            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();
            return Json(new { Sucess = true });
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var studentList = await dbContext.Students.ToListAsync();
            return View(studentList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await dbContext.Students.FindAsync(id);
            return View(student);
        }
        [HttpPost]
        public async Task<JsonResult> Edit(Student viewModel)
        {
            var student = await dbContext.Students.FindAsync(viewModel.Id);
            if (student != null)
            {
                student.FirstName = viewModel.FirstName;
                student.LastName = viewModel.LastName;
                student.Address = viewModel.Address;
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {

            var student = await dbContext.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (student is not null)
            {
                dbContext.Students.Remove(student);
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
        [HttpGet]
        public async Task<IActionResult> ExportStudentsToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var studentlist = await dbContext.Students.ToListAsync();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");
                worksheet.Cells[1, 1].Value = "Student Registration Number";
                worksheet.Cells[1, 2].Value = "Student First Name";
                worksheet.Cells[1, 3].Value = "Student Last Name";
                worksheet.Cells[1, 4].Value = "Student Address";

                int row = 2;
                foreach (var student in studentlist)
                {
                    worksheet.Cells[row, 1].Value = student.Registration_Num;
                    worksheet.Cells[row, 2].Value = student.FirstName;
                    worksheet.Cells[row, 3].Value = student.LastName;
                    worksheet.Cells[row, 4].Value = student.Address;
                    row++;
                }
                var bytes = package.GetAsByteArray();
                var fileName = "Students.xlsx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

            }
        }
        [HttpGet]
        public IActionResult UploadStudentsExcel()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> UploadStudentsExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (file == null||file.Length==0)
            {
                return Json(new { success = false, message = "No file selected." });
            }
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(); 
                        var rowCount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var student = new Student()
                            {
                                Registration_Num = int.Parse(worksheet.Cells[row, 1].Text),
                                FirstName = worksheet.Cells[row, 2].Text,
                                LastName = worksheet.Cells[row, 3].Text,
                                Address = worksheet.Cells[row, 4].Text,
                                CreatedDate = DateTime.Now
                            };
                            await dbContext.Students.AddAsync(student);
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
