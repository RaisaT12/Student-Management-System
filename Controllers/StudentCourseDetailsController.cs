using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Controllers
{
    public class StudentCourseDetailsController: Controller
    {
        private readonly AppDbContext dbContext;

        public StudentCourseDetailsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var StudentCourseDetailModel = new StudentCourseDetailsViewModel();
            var studentList = await dbContext.Students.ToListAsync();
            var courseList = await dbContext.Courses.ToListAsync();
            var gradeList = await dbContext.Grades.ToListAsync();
            var teacherList = await dbContext.Teachers.ToListAsync();
            StudentCourseDetailModel.StudentViewModelList= (List<SelectListItem>)studentList.Select(student=>new SelectListItem()
            {
                Value=student.Id.ToString(),
                Text=student.Registration_Num.ToString(),

            }).ToList();

            StudentCourseDetailModel.CourseViewModelList = (List<SelectListItem>)courseList.Select(course => new SelectListItem()
            {
                Value = course.Id.ToString(),
                Text = course.Name,

            }).ToList();
            StudentCourseDetailModel.GradeViewModelList = (List<SelectListItem>)gradeList.Select(grade => new SelectListItem()
            {
                Value = grade.Id.ToString(),
                Text = grade.Name,

            }).ToList();
            StudentCourseDetailModel.TeacherViewModelList = (List<SelectListItem>)teacherList.Select(teacher => new SelectListItem()
            {
                Value = teacher.Id.ToString(),
                Text = teacher.FirstName+ " " +teacher.LastName,

            }).ToList();

            return View(StudentCourseDetailModel);

        }


       
        [HttpPost]
        public async Task<JsonResult> Add(StudentCourseDetailsViewModel viewModel)
        {
            var student = await dbContext.Students.FindAsync(viewModel.StudentId);
            var course = await dbContext.Courses.FindAsync(viewModel.CourseId);
            var grade = await dbContext.Grades.FindAsync(viewModel.GradeId);
            var teacher = await dbContext.Teachers.FindAsync(viewModel.TeacherId);
            var studentcoursedetail = new StudentCourseDetails()
            {
                Student = student,
                Course = course,
                Grade = grade,
                Teacher = teacher,
                CreatedDate = viewModel.CreatedDate,
            };
            await dbContext.StudentCourseDetails.AddAsync(studentcoursedetail);
            await dbContext.SaveChangesAsync();
            return new JsonResult(new { Success = true });
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var studentcoursedetailList=await dbContext.StudentCourseDetails
                .Include(a=>a.Student).Include(a=>a.Course).Include(a=>a.Grade).Include(a=>a.Teacher)
                .ToListAsync();
            return View(studentcoursedetailList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var studentcoursedetail = await dbContext.StudentCourseDetails.FindAsync(id);
            var studentList= await dbContext.Students.ToListAsync();
            var courseList = await dbContext.Courses.ToListAsync();
            var gradeList = await dbContext.Grades.ToListAsync();
            var teacherList = await dbContext.Teachers.ToListAsync();
            var studentcoursedetailVM = new StudentCourseDetailsViewModel();
            studentcoursedetailVM.Id = studentcoursedetail.Id;
            studentcoursedetailVM.StudentId = studentcoursedetail.Student.Id;
            studentcoursedetailVM.StudentViewModelList= (List<SelectListItem>)studentList.Select(student=>new SelectListItem()
            {
                Value = student.Id.ToString(),
                Text = student.Registration_Num.ToString(),

            }).ToList();
            studentcoursedetailVM.CourseId = studentcoursedetail.Course.Id;
            studentcoursedetailVM.CourseViewModelList = (List<SelectListItem>)courseList.Select(course => new SelectListItem()
            {
                Value = course.Id.ToString(),
                Text = course.Name,

            }).ToList();
            studentcoursedetailVM.GradeId = studentcoursedetail.Grade.Id;
            studentcoursedetailVM.GradeViewModelList = (List<SelectListItem>)gradeList.Select(grade => new SelectListItem()
            {
                Value = grade.Id.ToString(),
                Text = grade.Name,

            }).ToList();
            studentcoursedetailVM.TeacherId = studentcoursedetail.Teacher.Id;
            studentcoursedetailVM.TeacherViewModelList = (List<SelectListItem>)teacherList.Select(teacher => new SelectListItem()
            {
                Value = teacher.Id.ToString(),
                Text = teacher.FirstName + " " + teacher.LastName,

            }).ToList();
            return View(studentcoursedetailVM);
        }
        [HttpPost]
        public async Task<JsonResult>Edit(StudentCourseDetailsViewModel viewmodel)
        {
            var studentcoursedetail = await dbContext.StudentCourseDetails.FindAsync(viewmodel.Id);
            var studentItem = await dbContext.Students.FindAsync(viewmodel.StudentId);
            var courseItem = await dbContext.Courses.FindAsync(viewmodel.CourseId);
            var gradeItem = await dbContext.Grades.FindAsync(viewmodel.GradeId);
            var teacherItem = await dbContext.Teachers.FindAsync(viewmodel.TeacherId);
            if(studentcoursedetail != null)
            {
                studentcoursedetail.Student = studentItem;
                studentcoursedetail.Course = courseItem;
                studentcoursedetail.Grade = gradeItem;
                studentcoursedetail.Teacher = teacherItem;
                await dbContext.SaveChangesAsync();
                return new JsonResult(new { Success = true });
            }
            return Json(new { Sucess = true });
        }
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {

            var studentcoursedetail = await dbContext.StudentCourseDetails.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (studentcoursedetail is not null)
            {
                dbContext.StudentCourseDetails.Remove(studentcoursedetail);
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
        [HttpGet]
        public async Task<IActionResult>ExportStudentCourseToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var studentcourselist = await dbContext.StudentCourseDetails
                .Include(a => a.Student).Include(a => a.Course)
                .Include(a => a.Grade).Include(a => a.Teacher).ToListAsync();
            using(var package=new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("StudentCourseDetails");
                worksheet.Cells[1, 1].Value = "Student Registration Number";
                worksheet.Cells[1, 2].Value = "Student Name";
                worksheet.Cells[1, 3].Value = "Course";
                worksheet.Cells[1, 4].Value = "Grade";
                worksheet.Cells[1, 5].Value = "Teacher Name";
                
                int row = 2;
                foreach (var item in studentcourselist)
                {
                    worksheet.Cells[row, 1].Value = item.Student.Registration_Num;
                    worksheet.Cells[row, 2].Value =$"{item.Student.FirstName} {item.Student.LastName}";
                    worksheet.Cells[row, 3].Value = item.Course.Name;
                    worksheet.Cells[row, 4].Value = item.Grade.Name;
                    worksheet.Cells[row, 5].Value = $"{item.Teacher.FirstName} {item.Teacher.LastName}";
                    row++;
                }
                var bytes = package.GetAsByteArray();

                var fileName = "StudentCourseDetails.xlsx";

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

            }

        }
        [HttpGet]
        public IActionResult UploadExcel()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> UploadExcel(IFormFile file)
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
                            var studentId = int.Parse(worksheet.Cells[row, 1].Text);
                            var courseId = int.Parse(worksheet.Cells[row, 2].Text);
                            var gradeId = int.Parse(worksheet.Cells[row, 3].Text);
                            var teacherId = int.Parse(worksheet.Cells[row, 4].Text);

                            var student = await dbContext.Students.FindAsync(studentId);
                            var course = await dbContext.Courses.FindAsync(courseId);
                            var grade = await dbContext.Grades.FindAsync(gradeId);
                            var teacher = await dbContext.Teachers.FindAsync(teacherId);

                            if (student != null && course != null && grade != null && teacher != null)
                            {
                                var studentCourseDetail = new StudentCourseDetails()
                                {
                                    Student = student,
                                    Course = course,
                                    Grade = grade,
                                    Teacher = teacher,
                                    CreatedDate = DateTime.Now
                                };
                                await dbContext.StudentCourseDetails.AddAsync(studentCourseDetail);
                            }
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
