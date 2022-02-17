using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context
                    .CelestialObjects
                    .Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObject);
        }


        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Name == name);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context
                    .CelestialObjects
                    .Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context
                    .CelestialObjects
                    .Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }



        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { celestialObject.Id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = celestialObject.Name;
            celestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            celestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;

            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();

        }

        [HttpPatch("{id}/name")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;
            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            _context.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
