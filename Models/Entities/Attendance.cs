using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagementSystem.Models.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public virtual Student Student { get; set; }
        public virtual Teacher Teacher { get; set; }
        public DateOnly Date {  get; set; }
        public bool Status {  get; set; }
        public DateTime CreatedDate {  get; set; }

    }
}
