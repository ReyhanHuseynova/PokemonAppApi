using PekemonReviewApp.DAL;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonReviewApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly AppDbContext _context;
        public ReviewerRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _context.Reviewers.Add(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            _context.Remove(reviewer);
            return Save();
        }

        public Reviewer GetReviewer(int id)
        {
            return _context.Reviewers.FirstOrDefault(r => r.Id == id);
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _context.Reviews.Where(r=>r.Reviewer.Id==reviewerId).ToList();
        }

        public bool ReviewerExist(int id)
        {
           return _context.Reviewers.Any(r=> r.Id == id);   
        }

        public bool Save()
        {
            var saved= _context.SaveChanges();  
            return saved>0?true:false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _context.Update(reviewer);
           return Save();
        }
    }
}
