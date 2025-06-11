using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Configurations;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudBaseController<TEntity, TKey> : ControllerBase where TEntity : BaseKeyEntity<TKey>
    {
        private readonly ICrudBaseService<TEntity, TKey> _service;

        protected CrudBaseController(ICrudBaseService<TEntity, TKey> service)
        {
            _service = service;
        }

        [HttpGet]
        protected virtual async Task<IActionResult> GetAll()
        {
            var entities = await _service.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        protected virtual async Task<IActionResult> GetById(TKey id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        protected virtual async Task<IActionResult> Create([FromBody] TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        protected virtual async Task<IActionResult> Update(TKey id, [FromBody] TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existingEntity = await _service.GetByIdAsync(id);
            if (existingEntity == null) return NotFound();

            await _service.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        protected virtual async Task<IActionResult> Delete(TKey id)
        {
            var existingEntity = await _service.GetByIdAsync(id);
            if (existingEntity == null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
