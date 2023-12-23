using PekemonReviewApp.DAL;
using PekemonReviewApp.Models;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository:IPokemonRepository
    {
        private readonly AppDbContext _context;
        public PokemonRepository(AppDbContext context)
        {
            _context = context; 
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemons.FirstOrDefault(p => p.Id == id);
        }

        public Pokemon NamePokemon(string name)
        {
            return _context.Pokemons.FirstOrDefault(p => p.Name == name);
        }

        public decimal GetPokemonRating(int pokeId)
        {
           var rewiew= _context.Reviews.Where(r=>r.Pokemon.Id==pokeId);
            if(rewiew.Count() <= 0)
            {
                return 0;
            }
            return ((decimal) rewiew.Sum(r=>r.Rating)/rewiew.Count());
        }

        public ICollection<Pokemon> GetPokemons()
        {
           return _context.Pokemons.OrderBy(p=>p.Id).ToList();  
        }

        public bool PokemonExist(int id)
        {
           return _context.Pokemons.Any(p=>p.Id == id);
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity=_context.Owners.Where(o=>o.Id== ownerId).FirstOrDefault();
            var category=_context.Categories.Where(c=>c.Id== categoryId).FirstOrDefault();
            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon
            };
            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon
            };

            _context.Add(pokemonCategory);
            _context.Add(pokemon);
            return Save();
            
        }

        public bool Save()
        {
            var saved=_context.SaveChanges();
            return saved>0 ? true : false;
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
          _context.Update(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }
    }
}
