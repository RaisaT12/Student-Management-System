using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Controllers
{
    public class GradesController : Controller
    {
        private readonly AppDbContext dbContext;
        public GradesController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Add(string name, DateTime date)
        {
                Grade grade = new Grade();
                grade.Name = name;
                grade.CreatedDate = date;
                await dbContext.Grades.AddAsync(grade);
                await dbContext.SaveChangesAsync();
                return Json(new { Sucess = true });
            
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var gradeList = await dbContext.Grades.ToListAsync();
            return View(gradeList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var grade = await dbContext.Grades.FindAsync(id);
            return View(grade);
        }
        [HttpPost]
        public async Task<JsonResult> Edit(Grade viewmodel)
        {
            var grade = await dbContext.Grades.FindAsync(viewmodel.Id);
            if (grade is not null)
            {
                grade.Name = viewmodel.Name;
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });
        }
        [HttpDelete]
        public async Task<JsonResult>Delete(int id)
        {

            var grade=await dbContext.Grades.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (grade is not null)
            {
                dbContext.Grades.Remove(grade);
                await dbContext.SaveChangesAsync();
            }
            return Json(new { Sucess = true });

        }
       
    }
}
