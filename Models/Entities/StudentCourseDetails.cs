namespace StudentManagementSystem.Models.Entities
{
    public class StudentCourseDetails
    {
        public int Id { get; set; }
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
        public virtual Grade Grade { get; set; }
        public virtual Teacher Teacher { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
