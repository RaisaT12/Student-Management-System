using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Controllers
{
    public class CoursesController : Controller
    {
        private readonly AppDbContext dbContext;
        public CoursesController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Add(string name, string desc, DateTime date)
        {
            Course course = new Course();
            course.Name = name;
            course.Description = desc;
            course.CreatedDate = date;
            await dbContext.Courses.AddAsync(course);
            await dbContext.SaveChangesAsync();
            return Json(new { Sucess = true });
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var courseList = await dbContext.Courses.ToListAsync();
            return View(courseList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await dbContext.Courses.FindAsync(id);

            return View(course);
        }
        [HttpPost]
        public async Task<JsonResult>Edit(Course viewModel)
        {
            var course=await dbContext.Courses.FindAsync(viewModel.Id);
            if (course != null) {
                course.Name = viewModel.Name;
                course.Description=viewModel.Description;
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
    
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {

            var course = await dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (course is not null)
            {
                dbContext.Courses.Remove(course);
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
        [HttpGet]
        public IActionResult UploadCoursesExcel()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> UploadCoursesExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (file == null || file.Length == 0)
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
                            var course = new Course()
                            {
                                Name = worksheet.Cells[row, 1].Text,
                                Description = worksheet.Cells[row, 2].Text,
                                CreatedDate = DateTime.Now
                            };
                            await dbContext.Courses.AddAsync(course);
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

