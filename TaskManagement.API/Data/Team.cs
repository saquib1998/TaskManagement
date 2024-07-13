namespace TaskManagement.API.Data
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public AppUser Manager { get; set; }
        public List<AppUser> Members { get; set; }
    }
}
