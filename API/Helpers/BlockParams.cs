namespace API.Helpers
{
    public class BlockParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Predicate { get; set; }
    }
}