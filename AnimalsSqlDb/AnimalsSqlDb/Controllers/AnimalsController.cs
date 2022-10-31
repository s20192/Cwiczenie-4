using Microsoft.AspNetCore.Mvc;

namespace AnimalsSqlDb.Controllers
{
    [Route("api/animals")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private IDatabaseService _databaseService;

        public AnimalsController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public IActionResult GetAnimals(string orderBy = "Name")
        {
            return Ok(_databaseService.GetAnimals(orderBy));
        }

        [HttpPost]
        public IActionResult AddAnimal(Animal animal)
        {
            try
            {
                _databaseService.AddAnimal(animal);
                
                return Ok(animal);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("{idNumber}")]
        public IActionResult GetAnimal(int idNumber)
        {
            try
            {
                return Ok(_databaseService.GetAnimal(idNumber));
            }
            catch (AnimalNotFoundException e)
            {
                return NotFound(e.Message);
            } catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{idNumber}")]
        public IActionResult UpdateAnimal(int idNumber, Animal animal)
        {
            try
            {
                _databaseService.UpdateAnimal(idNumber, animal);
            }
            catch (AnimalNotFoundException e)
            {
                return NotFound(e.Message);
            } catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(animal);
        }

        [HttpDelete("{idNumber}")]
        public IActionResult DeleteAnimal(int idNumber)
        {
            try
            {
                _databaseService.DeleteAnimal(idNumber);
            }
            catch (AnimalNotFoundException e)
            {
                return NotFound(e.Message);
            } catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }

}
