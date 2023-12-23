namespace PokemonReviewApp.Models
{
    public class Reviewer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
