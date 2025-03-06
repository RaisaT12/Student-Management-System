using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly AppDbContext dbContext;
       
        public AttendancesController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var AttendanceModel=new AttendanceViewModel();
            var studentList = await dbContext.Students.ToListAsync();
            var teacherList= await dbContext.Teachers.ToListAsync();
            AttendanceModel.StudentViewModelList = studentList.Select(a => new SelectListItem()
            {
                Value=a.Id.ToString(),
                Text=a.Registration_Num.ToString(),
                
            }).ToList();
            AttendanceModel.TeacherViewModelList = teacherList.Select(b => new SelectListItem()
            {
                Value = b.Id.ToString(),
                Text = b.FirstName+" "+b.LastName,

            }).ToList();

            return View(AttendanceModel);
        }
        [HttpPost]
        public async Task<JsonResult>Add(AttendanceViewModel viewModel)
        {
            var student= await dbContext.Students.FindAsync(viewModel.StudentId);
            var teacher= await dbContext.Teachers.FindAsync(viewModel.TeacherId);
            Attendance attendance = new Attendance()
            {
                Student = student,
                Teacher = teacher,
                Status = viewModel.Status,
                Date = viewModel.Date,
                CreatedDate = viewModel.CreatedDate,
            };
            await dbContext.Attendance.AddRangeAsync(attendance);
            await dbContext.SaveChangesAsync();
            return new JsonResult(new { Success = true });

        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var attendanceList = await dbContext.Attendance
                .Include(a => a.Student).Include(a => a.Teacher)
                .ToListAsync();
            return View(attendanceList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var attendance = await dbContext.Attendance.FindAsync(id);
            var studentList = await dbContext.Students.ToListAsync();
            var teacherList = await dbContext.Teachers.ToListAsync();
            var attendanceVM = new AttendanceViewModel();
            attendanceVM.Status=attendance.Status;
            attendanceVM.Date = attendance.Date;    
            attendanceVM.StudentId = attendance.Student.Id;
            attendanceVM.StudentViewModelList = (List<SelectListItem>)studentList.Select(student => new SelectListItem()
            {
                Value = student.Id.ToString(),
                Text = student.Registration_Num.ToString(),

            }).ToList();

            attendanceVM.TeacherId = attendance.Teacher.Id;
            attendanceVM.TeacherViewModelList = (List<SelectListItem>)teacherList.Select(teacher => new SelectListItem()
            {
                Value = teacher.Id.ToString(),
                Text = teacher.FirstName + " " + teacher.LastName,

            }).ToList();
            return View(attendanceVM);
        }
        [HttpPost]
        public async Task<JsonResult> Edit(AttendanceViewModel viewmodel)
        {
            var attendance = await dbContext.Attendance.FindAsync(viewmodel.Id);
            var studentItem = await dbContext.Students.FindAsync(viewmodel.StudentId);
            var teacherItem = await dbContext.Teachers.FindAsync(viewmodel.TeacherId);
            if (attendance != null)
            {
                attendance.Student = studentItem;
                attendance.Teacher = teacherItem;
                attendance.Status=viewmodel.Status;
                attendance.Date = viewmodel.Date;
                await dbContext.SaveChangesAsync();
                return new JsonResult(new { Success = true });
            }
            return Json(new { Sucess = true });
        }
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {

            var attendance = await dbContext.Attendance.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (attendance is not null)
            {
                dbContext.Attendance.Remove(attendance);
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
        public async Task<IActionResult> ExportAttendanceToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var attendancelist=await dbContext.Attendance.Include(a=>a.Student).Include(a=>a.Teacher).ToListAsync();

            using(var package=new ExcelPackage()){
                var worksheet = package.Workbook.Worksheets.Add("Attendances");
                worksheet.Cells[1, 1].Value = "Student Registration Number";
                worksheet.Cells[1, 2].Value = "Student Name";
                worksheet.Cells[1, 3].Value = "Teacher Name";
                worksheet.Cells[1, 4].Value = "Date";
                worksheet.Cells[1, 5].Value = "Status";

                int row = 2;
                foreach (var attendance in attendancelist) 
                {
                    worksheet.Cells[row,1].Value = attendance.Student.Registration_Num;
                    worksheet.Cells[row,2].Value= $"{attendance.Student.FirstName} {attendance.Student.LastName}";
                    worksheet.Cells[row,3].Value = $"{attendance.Teacher.FirstName} {attendance.Teacher.LastName}";
                    worksheet.Cells[row, 4].Value = attendance.Date;
                    worksheet.Cells[row,5].Value= attendance.Status;
                    row++;
                }
                var bytes=package.GetAsByteArray();
                var fileName = "Attendance.xlsx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

            }
        }


    }
}
