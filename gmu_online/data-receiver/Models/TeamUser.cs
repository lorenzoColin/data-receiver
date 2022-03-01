namespace data_receiver.Models
{
    public class TeamUser
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
