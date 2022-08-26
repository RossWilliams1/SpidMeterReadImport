using Microsoft.AspNetCore.Mvc;
using SpidMeterReadImport.Service.Interfaces;

namespace SpidMeterReadImport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpidMeterReadController : ControllerBase
    {
        private readonly ISpidMeterReadService _spidMeterReadService;
        public SpidMeterReadController(ISpidMeterReadService spidMeterReadService)
        {
            _spidMeterReadService = spidMeterReadService;
        }

        /// <summary>
        /// Imports Spid Meter reads into the system
        /// </summary>
        /// <returns>A summary of the import</returns>
        [Route("~/meter-readings-upload")]
        [HttpPost]
        public async Task<IActionResult> MeterReadUpload()
        {
            var file = Request?.Form?.Files?.FirstOrDefault();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded or it is empty.");

            try
            {
                var result = await _spidMeterReadService.ImportSpidMeterReads(file.OpenReadStream());
                return Ok(result);
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
