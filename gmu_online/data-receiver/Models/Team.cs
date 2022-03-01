namespace data_receiver.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<TeamUser> teamUser { get; set; }


        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
