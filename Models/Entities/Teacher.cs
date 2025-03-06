namespace StudentManagementSystem.Models.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Contact {  get; set; }
        public DateOnly JoinDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
