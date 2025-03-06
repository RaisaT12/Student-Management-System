
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementSystem.Models.Entities;

namespace StudentManagementSystem.Models
{
    public class AttendanceViewModel:Attendance
    {
        public List<SelectListItem> StudentViewModelList { get; set; }
        public List<SelectListItem> TeacherViewModelList { get; set; }
        public int StudentId {  get; set; }
        public int TeacherId { get; set; }
    }
}
