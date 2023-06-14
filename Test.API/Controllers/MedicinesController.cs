using Microsoft.AspNetCore.Mvc;
using Test.API.Models;
using Newtonsoft.Json;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("medicines")]
    public class MedicinesController : ControllerBase
    {
        private readonly ILogger<MedicinesController> _logger;
        private readonly string _dbFilePath;

        public MedicinesController(ILogger<MedicinesController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _dbFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "db.json");
            if (!System.IO.File.Exists(_dbFilePath))
            {
                System.IO.File.Create(_dbFilePath);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetMedicines([FromQuery]string name = "")
        {
            try
            {
                var medicines = ReadDataFromFile();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    medicines = medicines?.Where(x => x.FullName!.Contains(name)).ToList();
                }
                return medicines?.Any() == true ? Ok(medicines) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving medicines: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving medicines.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddMedicine(Medicines medicine)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var data = ReadDataFromFile();
                var lastRecordsId = data!.OrderByDescending(x => x.MedicineId).Select(y => y.MedicineId).FirstOrDefault();
                medicine.MedicineId = lastRecordsId + 1;
                data!.Add(medicine);
                WriteDataIntoFile(data);
                return Created("medicines", medicine);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding a medicine: {ex}");
                return StatusCode(500, "An error occurred while adding a medicine.");
            }
        }

        private List<Medicines>? ReadDataFromFile()
        {
            try
            {
                var jsonData = System.IO.File.ReadAllText(_dbFilePath);
                return string.IsNullOrWhiteSpace(jsonData) ? new List<Medicines>() : JsonConvert.DeserializeObject<List<Medicines>>(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while reading data from the file: {ex}");
                throw;
            }
        }
        
        private void WriteDataIntoFile(List<Medicines> medicines)
        {
            try
            {
                var jData = JsonConvert.SerializeObject(medicines, Formatting.Indented);
                System.IO.File.WriteAllText(_dbFilePath, jData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while writing data to the file: {ex}");
                throw;
            }
        }
    }
}
