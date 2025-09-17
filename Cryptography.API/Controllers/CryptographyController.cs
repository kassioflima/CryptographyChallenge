using Cryptography.Domain.DTOs;
using Cryptography.Domain.Entities;
using Cryptography.Domain.Interfaces;
using Cryptography.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cryptography.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class CryptographyController : ControllerBase
    {
        private readonly IRepository<CryptData> _repository;
        private readonly ICryptographyService _cryptographyService;
        private readonly ICryptographyProvider _cryptographyProvider;

        public CryptographyController(
            ICryptographyService cryptographyService,
            IRepository<CryptData> repository,
            ICryptographyProvider cryptographyProvider)
        {
            _cryptographyService = cryptographyService;
            _repository = repository;
            _cryptographyProvider = cryptographyProvider;
        }

        [HttpGet]
        [Route("All/{encrypted?}")]
        public async Task<IActionResult> GetAll(bool encrypted = false)
        {
            if (encrypted)
            {
                var entities = await _repository.GetAllAsync();
                return Ok(entities);
            }

            var entitiesDtos = await _cryptographyService.GetAllAsync();
            return Ok(entitiesDtos);
        }

        [HttpGet]
        [Route("Get/{id}/{encrypted?}")]
        public async Task<IActionResult> GetById(long id, bool encrypted = false)
        {
            if (encrypted)
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }

            var entityDto = await _cryptographyService.GetByIdAsync(id);
            if (entityDto == null)
            {
                return NotFound();
            }

            return Ok(entityDto);
        }

    [HttpPost]
    [Route("Add")]
    public async Task<IActionResult> Create(CryptDataDTO data)
    {
        if (data == null)
            return BadRequest();

        var createdEntity = await _cryptographyService.CreateAsync(data);
        return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id, encrypted = true }, createdEntity);
    }

    [HttpPut]
    [Route("Update/{id}")]
    public async Task<IActionResult> Update(long id, CryptDataDTO data)
    {
        if (data == null)
            return BadRequest();

        var updatedEntity = await _cryptographyService.UpdateAsync(id, data);
        
        if (updatedEntity == null)
        {
            return NotFound();
        }

        return NoContent();
    }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteById(long id)
        {
            var deleted = await _cryptographyService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
