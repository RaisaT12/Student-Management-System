using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Models
{
    public class StudentCourseDetailsViewModel
    {
        public int Id { get; set; }
        public List<SelectListItem> StudentViewModelList { get;  set; }
        public List<SelectListItem> CourseViewModelList { get;  set; }
        public List<SelectListItem> GradeViewModelList { get; set; }
        public List<SelectListItem> TeacherViewModelList { get; set; }
        public int StudentId {  get; set; }
        public int CourseId {  get; set; }
        public int GradeId { get;  set; }
        public int TeacherId { get;  set; }   
        public DateTime CreatedDate { get; set; }

    }
}
