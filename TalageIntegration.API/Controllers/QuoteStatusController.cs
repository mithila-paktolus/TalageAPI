using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Talage.SDK.EntityFramework.TalageIntegration.Model;
using TalageIntegration.API.Services;

namespace TalageIntegration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteStatusController : ControllerBase
    {
        private readonly IQuoteStatusService _service;

        public QuoteStatusController(IQuoteStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<QuoteStatus>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuoteStatus>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<QuoteStatus>> Create(QuoteStatus quoteStatus)
        {
            var result = await _service.Create(quoteStatus);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuoteStatus quoteStatus)
        {
            if (id != quoteStatus.Id) return BadRequest();
            await _service.Update(quoteStatus);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return NoContent();
        }
    }
}
