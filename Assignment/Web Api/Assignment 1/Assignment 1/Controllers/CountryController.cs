using Assignment_1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Assignment_1.Controllers
{
    public class CountryController : ApiController
    {
        private static List<Country> countries = new List<Country>();

        // GET api/country
        public IHttpActionResult Get()
        {
            return Ok(countries);
        }

        // GET api/country/1
        public IHttpActionResult Get(int id)
        {
            var country = countries.FirstOrDefault(c => c.ID == id);
            if (country == null)
                return NotFound();
            return Ok(country);
        }

        // POST api/country
        public IHttpActionResult Post(Country country)
        {
            if (country == null)
                return BadRequest("Country object is null");

            country.ID = countries.Count + 1;
            countries.Add(country);

            return CreatedAtRoute("DefaultApi", new { id = country.ID }, country);
        }

        // PUT api/country/1
        public IHttpActionResult Put(int id, Country country)
        {
            var existingCountry = countries.FirstOrDefault(c => c.ID == id);
            if (existingCountry == null)
                return NotFound();

            existingCountry.CountryName = country.CountryName;
            existingCountry.Capital = country.Capital;

            return Ok(existingCountry);
        }

        // DELETE api/country/1
        public IHttpActionResult Delete(int id)
        {
            var country = countries.FirstOrDefault(c => c.ID == id);
            if (country == null)
                return NotFound();

            countries.Remove(country);

            return Ok(country);
        }
    }
}
