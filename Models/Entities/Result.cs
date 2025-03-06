namespace StudentManagementSystem.Models.Entities
{
    public class Result
    {
        public int Id { get; set; }
        public decimal Marks {  get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual StudentCourseDetails StudentCourseDetails { get; set; }
    }
}
