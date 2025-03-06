namespace StudentManagementSystem.Models.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public int Registration_Num {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address {  get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
