using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagementSystem.Models
{
    public class ResultViewModel
    {
        public int Id { get; set; }
        public List<SelectListItem> StudentCourseDetailsViewModelList {  get; set; }
        public int StudentCourseDetailsId {  get; set; }
        public decimal Marks { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
