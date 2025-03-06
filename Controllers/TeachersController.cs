using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Controllers
{
    public class TeachersController : Controller
    {
        private readonly AppDbContext dbContext;
        public TeachersController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Add(string fname, string lname, string address, string contact, DateOnly joindate, DateTime createddate)
        {
            Teacher teacher = new Teacher();
            teacher.FirstName = fname;
            teacher.LastName = lname;
            teacher.Address = address;
            teacher.Contact = contact;
            teacher.JoinDate = joindate;
            teacher.CreatedDate = createddate;
            await dbContext.Teachers.AddAsync(teacher);
            await dbContext.SaveChangesAsync();
            return Json(new { Sucess = true });
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var teacherList = await dbContext.Teachers.ToListAsync();
            return View(teacherList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await dbContext.Teachers.FindAsync(id);
            var joindate = teacher.JoinDate;
            return View(teacher);
        }
        [HttpPost]
        public async Task<JsonResult> Edit(Teacher viewModel)
        {
            var teacher = await dbContext.Teachers.FindAsync(viewModel.Id);
            if (teacher != null)
            {
                teacher.FirstName = viewModel.FirstName;
                teacher.LastName = viewModel.LastName;
                teacher.Address = viewModel.Address;
                teacher.Contact = viewModel.Contact;
                teacher.JoinDate = viewModel.JoinDate;
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {

            var teacher = await dbContext.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (teacher is not null)
            {
                dbContext.Teachers.Remove(teacher);
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
        [HttpGet]
        public async Task<IActionResult> ExportTeachersToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var teacherlist = await dbContext.Teachers.ToListAsync();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Teachers");
                worksheet.Cells[1, 1].Value = "Teacher First Name";
                worksheet.Cells[1, 2].Value = "Teacher Last Name";
                worksheet.Cells[1, 3].Value = "Teacher Address";
                worksheet.Cells[1, 4].Value = "Teacher Contact";
                worksheet.Cells[1, 5].Value = "Teacher Join Date";

                int row = 2;
                foreach (var teacher in teacherlist)
                {
                    worksheet.Cells[row, 1].Value = teacher.FirstName;
                    worksheet.Cells[row, 2].Value = teacher.LastName;
                    worksheet.Cells[row, 3].Value = teacher.Address;
                    worksheet.Cells[row, 4].Value = teacher.Contact;
                    worksheet.Cells[row, 5].Value = teacher.JoinDate;
                    row++;
                }
                var bytes = package.GetAsByteArray();
                var fileName = "Teachers.xlsx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

            }
        }
        [HttpGet]
        public IActionResult UploadTeachersExcel()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> UploadTeachersExcel(IFormFile file)
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
                            var teacher = new Teacher()
                            {
                                FirstName = worksheet.Cells[row,1].Text,
                                LastName = worksheet.Cells[row, 2].Text,
                                Address = worksheet.Cells[row,3].Text,
                                Contact = worksheet.Cells[row, 4].Text,
                                CreatedDate = DateTime.Now
                            };
                            await dbContext.Teachers.AddAsync(teacher);
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
