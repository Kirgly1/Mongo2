using CatShopApi.Models;
using CatShopService;
using Microsoft.AspNetCore.Mvc;

namespace CatShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly ICatService _catsService;

        public CatsController(ICatService catsService)
        {
            _catsService = catsService;
        }

        [HttpGet]
        public async Task<List<Cat>> Get() =>
            await _catsService.GetAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Cat newCat)
        {
            await _catsService.CreateAsync(newCat);
            return CreatedAtAction(nameof(Get), new { id = newCat.Id }, newCat);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Cat updatedCat)
        {
            var cat = await _catsService.GetAsync(id);
            if (cat is null)
            {
                return NotFound();
            }

            updatedCat.Id = id;
            await _catsService.UpdateAsync(id, updatedCat);
            return NoContent();
        }



        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleteResult = await _catsService.DeleteCatAndAssociatedComments(id);

            if (deleteResult)
            {
                return Ok("Cat and associated comments deleted successfully.");
            }
            else
            {
                return NotFound("Cat not found or error deleting associated comments.");
            }
        }
    }
}
