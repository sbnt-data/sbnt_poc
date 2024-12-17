using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerDetails(string customerId)
        {
            try
            {
                var customers = await _customerRepository.GetCustomerDetailsAsync(customerId);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer details.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{customerId}/name")]
        public async Task<IActionResult> UpdateCustomerName(string customerId, [FromBody] string newName)
        {
            try
            {
                var rowsAffected = await _customerRepository.UpdateCustomerNameAsync(customerId, newName);
                return Ok(new { message = "Customer updated successfully.", rowsAffected });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer name.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("count/{country}")]
        public async Task<IActionResult> GetCustomerCount(string country)
        {
            try
            {
                var count = await _customerRepository.GetCustomerCountByCountryAsync(country);
                return Ok(new { country, count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer count.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{customerId}/with-orders")]
        public async Task<IActionResult> GetCustomerWithOrders(string customerId)
        {
            try
            {
                var (customers, orders) = await _customerRepository.GetCustomerWithOrdersAsync(customerId);
                return Ok(new { customers, orders });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer with orders.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{customerId}/orders-and-employees")]
        public async Task<IActionResult> GetCustomerOrdersAndEmployees(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                _logger.LogWarning("Invalid customer ID provided.");
                return BadRequest(new { status = "error", message = "Customer ID cannot be null or empty." });
            }

            try
            {
                var (customers, orders, employees) = await _customerRepository.GetCustomerOrdersAndEmployeesAsync(customerId);

                return Ok(new
                {
                    status = "success",
                    message = "Customer, orders, and employees retrieved successfully.",
                    data = new
                    {
                        Customers = customers,
                        Orders = orders,
                        Employees = employees
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customer, orders, and employees for ID: {CustomerId}", customerId);
                return StatusCode(500, new { status = "error", message = "An unexpected error occurred. Please try again later." });
            }
        }

    }
}
