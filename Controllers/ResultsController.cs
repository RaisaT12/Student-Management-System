using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Entities;
using System.IO;

using System.Text;

namespace StudentManagementSystem.Controllers
{

    public class ResultsController : Controller
    {
        private readonly AppDbContext dbContext;

        public ResultsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var ResultModel = new ResultViewModel();
            var studentcoursedetailsList = await dbContext.StudentCourseDetails.Include(a => a.Student).Include(a => a.Course).ToListAsync();
            

            ResultModel.StudentCourseDetailsViewModelList= (List<SelectListItem>)studentcoursedetailsList.Select(a=>new SelectListItem()
            {
                Value=a.Id.ToString(),
                Text="Student Registration Number: " + a.Student.Registration_Num + " Student Name: " + a.Student.FirstName + " " + a.Student.LastName + " Course: " + a.Course.Name,

            }).ToList();

            return View(ResultModel);
        }
        [HttpPost]
        public async Task<JsonResult>Add(ResultViewModel viewModel)
        {
            var studentcoursedetails=await dbContext.StudentCourseDetails.FindAsync(viewModel.StudentCourseDetailsId);

            var result = new Result()
            {
                StudentCourseDetails = studentcoursedetails,
                Marks = viewModel.Marks,
                CreatedDate = viewModel.CreatedDate,
            };
            await dbContext.Results.AddAsync(result);
            await dbContext.SaveChangesAsync();
            return new JsonResult(new {Success=true});
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var resultList=await dbContext.Results
                                .Include(a=>a.StudentCourseDetails)
                                .Include(a => a.StudentCourseDetails.Student)
                                .Include(a => a.StudentCourseDetails.Course).ToListAsync();
            return View(resultList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await dbContext.Results.FindAsync(id);
            var studentcoursedetailsList = await dbContext.StudentCourseDetails.Include(a => a.Student).Include(a => a.Course).ToListAsync();
            var resultVM = new ResultViewModel();
            resultVM.Id = result.Id;
            resultVM.Marks = result.Marks;
            resultVM.StudentCourseDetailsId = result.StudentCourseDetails.Id;
            resultVM.StudentCourseDetailsViewModelList = (List<SelectListItem>)studentcoursedetailsList.Select(a => new SelectListItem()
            {
                Value=a.Id.ToString(),
                Text= "Student Registration Number: " + a.Student.Registration_Num + " Student Name: " + a.Student.FirstName + " " + a.Student.LastName + " Course: " + a.Course.Name,
            }).ToList();
            return View(resultVM);
        }
        [HttpPost]
        public async Task<JsonResult>Edit(ResultViewModel viewmodel)
        {
            var result=await dbContext.Results.FindAsync(viewmodel.Id);
            var studentcoursedetailsitem=await dbContext.StudentCourseDetails.FindAsync(viewmodel.StudentCourseDetailsId);
            if (result != null)
            {
                result.StudentCourseDetails = studentcoursedetailsitem;
                result.Marks = viewmodel.Marks;
                await dbContext.SaveChangesAsync();
                return new JsonResult(new { Success = true });
            }
            return Json(new { Sucess = true });
        }
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            var result=await dbContext.Results.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                dbContext.Results.Remove(result);
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Success = true });
        }
        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var resultList = await dbContext.Results
                .Include(a => a.StudentCourseDetails)
                .Include(a => a.StudentCourseDetails.Student)
                .Include(a => a.StudentCourseDetails.Course)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Results");

                worksheet.Cells[1, 1].Value = "Student Registration Number";
                worksheet.Cells[1, 2].Value = "Student First Name";
                worksheet.Cells[1, 3].Value = "Student Last Name";
                worksheet.Cells[1, 4].Value = "Course";
                worksheet.Cells[1, 5].Value = "Marks";


                int row = 2;
                foreach (var result in resultList)
                {
                    worksheet.Cells[row, 1].Value = result.StudentCourseDetails.Student.Registration_Num;
                    worksheet.Cells[row, 2].Value = result.StudentCourseDetails.Student.FirstName;
                    worksheet.Cells[row, 3].Value = result.StudentCourseDetails.Student.LastName;
                    worksheet.Cells[row, 4].Value = result.StudentCourseDetails.Course.Name;
                    worksheet.Cells[row, 5].Value = result.Marks;
                    row++;
                }

                var bytes = package.GetAsByteArray();

                var fileName = "Results.xlsx";

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
       


    }


}

