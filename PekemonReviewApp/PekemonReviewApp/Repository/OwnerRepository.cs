using Microsoft.EntityFrameworkCore;
using PekemonReviewApp.DAL;
using PekemonReviewApp.Models;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly AppDbContext _context;
        public OwnerRepository(AppDbContext context)
        {
            _context = context; 
        }

        public bool CreateOwner(Owner owner)
        {
           _context.Owners.Add(owner);
            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.FirstOrDefault(o => o.Id == ownerId);
        }

        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
          return _context.PokemonOwners.Where(po=>po.PokemonId == pokeId).Select(po=>po.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where(po=>po.OwnerId==ownerId).Select(po=>po.Pokemon).ToList();  
        }

        public bool OwnerExists(int ownerId)
        {
          return _context.Owners.Any(o=>o.Id == ownerId);
        }

        public bool Save()
        {
            var save = _context.SaveChanges();
            return save>0?true:false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Update(owner);
            return Save();
        }
    }
}
