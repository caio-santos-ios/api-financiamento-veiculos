using api_financiamento.src.Interfaces;
using api_financiamento.src.Models;
using api_financiamento.src.Models.Base;
using api_financiamento.src.Responses.Simulation;
using api_financiamento.src.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api_financiamento.src.Controllers
{
    [Route("api/simulations")]
    [ApiController]
    public class SimulationController(ISimulationService simulationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await simulationService.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await simulationService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSimulationDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");

            ResponseApi<Simulation?> response = await simulationService.CreateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSimulationDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");

            ResponseApi<Simulation?> response = await simulationService.UpdateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Simulation> response = await simulationService.DeleteAsync(id);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] CalculateSimulationDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");

            ResponseApi<SimulationResponse> response = simulationService.Calculate(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [HttpGet("compare")]
        public IActionResult CompareInstallments(
            [FromQuery] decimal vehicleValue,
            [FromQuery] decimal downPayment,
            [FromQuery] decimal monthlyInterestRate)
        {
            ResponseApi<CompareInstallmentsResponse> response = simulationService.CompareInstallments(vehicleValue, downPayment, monthlyInterestRate);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
    }
}
