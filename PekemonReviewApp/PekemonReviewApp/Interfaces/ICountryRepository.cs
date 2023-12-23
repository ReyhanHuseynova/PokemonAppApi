using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICountryRepository
    {
         ICollection<Country> GetCountryList();
         Country GetCountry(int id);
        Country GetCountryByOwner(int ownerId); 
        ICollection<Owner> GetOwnersFromACountry(int countryId);
        bool CountryExist(int countryId);   
        bool CreateCountry(Country country);
        bool UpdateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();

    }
}
