using PekemonReviewApp.DAL;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        public ReviewRepository(AppDbContext context)
        {
            _context=context;
        }

        public bool CreateReview(Review review)
        {
           _context.Reviews.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _context.Remove(review);
            return Save();
        }

        public Review GetReview(int id)
        {
            return _context.Reviews.FirstOrDefault(r => r.Id == id);
        }

        public ICollection<Review> GetReviewOfAPokemon(int pokeId)
        {
            return _context.Reviews.Where(r=>r.Pokemon.Id==pokeId).ToList();
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        public bool ReviewExist(int id)
        {
            return _context.Reviews.Any(review => review.Id == id);
        }

        public bool Save()
        {
           var saved=_context.SaveChanges();    
            return saved>0?true:false;  
        }

        public bool UpdateReview(Review review)
        {
            _context.Update(review);
            return Save();
        }
    }
}
