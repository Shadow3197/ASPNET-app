namespace API.Entities
{
    public class BlockUser
    {
        public AppUser SourceUser {get;set;}
        public int SourceUserId { get; set; }
        public AppUser BlockedUser { get; set; }
        public int BlockedUserId { get; set; }
    }
}