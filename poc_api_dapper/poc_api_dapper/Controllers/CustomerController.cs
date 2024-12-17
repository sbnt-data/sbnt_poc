using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using poc_api_dapper.Models;
using poc_api_dapper.Repositories;

namespace poc_api_dapper.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerRepository customerRepository, ILogger<CustomerController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves customer details for the given customer ID.
        /// </summary>
        /// <param name="customerId">The unique identifier for the customer.</param>
        /// <returns>Returns customer details if found; otherwise, returns an appropriate error response.</returns>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerDetails(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                _logger.LogWarning("Invalid customer ID provided.");
                return BadRequest(new { status = "error", message = "Customer ID cannot be null or empty." });
            }

            try
            {
                // Fetch customer details
                var customerDetails = await _customerRepository.GetCustomerDetails(customerId);

                if (customerDetails == null || !customerDetails.Any())
                {
                    _logger.LogInformation("Customer not found for ID: {CustomerId}", customerId);
                    return NotFound(new { status = "error", message = $"Customer with ID '{customerId}' not found." });
                }

                // Return success response
                return Ok(new
                {
                    status = "success",
                    message = "Customer details retrieved successfully.",
                    data = customerDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customer details for ID: {CustomerId}", customerId);
                return StatusCode(500, new { status = "error", message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
