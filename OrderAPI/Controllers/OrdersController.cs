using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Constants;
using OrderAPI.DataAccess;
using OrderAPI.DataTransfer;
using OrderAPI.DataAccess.Models;
using OrderAPI.DataTransfer.Extentions;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly DbAccess _dbContext;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(DbAccess dbContext, ILogger<OrdersController> logger)
        {
            _logger = logger; 
            _dbContext = dbContext;
        }

        // POST: Orders/Create
        [ProducesResponseType(typeof(Order), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("400 Error (BadRequest) : Order fields are not valid");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("On Orders/Create: ");

            try
            {
                var existingOrder = _dbContext.Orders
                    .FirstOrDefault(o => o.OrderNumber == orderRequest.OrderNumber);

                if (existingOrder != null)
                {
                    _logger.LogInformation("409 Error (Conflict): Order with number {OrderNumber} already exists.", orderRequest.OrderNumber);
                    return Conflict($"Order with number {orderRequest.OrderNumber} already exists.");
                }
                
                var order = orderRequest.ToModel();

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();
                return Created(string.Empty, $"Order {order.OrderNumber} successfully created.");

            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error occurred while saving order to the database.");
                return StatusCode(500, "An error occurred while saving the order.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // GET: Orders/GetAllOrders
        [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _dbContext.Orders
                    .Include(o => o.OrderItems)
                    .ToListAsync();

                var orderResponses = orders.Select(order => order.ToResponse()).ToList();                
                return Ok(orderResponses);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error occurred while retrieving orders from the database.");
                return StatusCode(500, "An error occurred while retrieving the orders.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving orders.");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // Helper method: GET: Order/{id}
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("Order {id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("400 Error (BadRequest) : Order fields are not valid");
                return BadRequest(ModelState);
            }
            try
            {
                var order = await _dbContext.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderNumber == id);

                if (order == null)
                {
                    _logger.LogInformation("404 Error: Order with ID {id} not found.", id);
                    return NotFound($"Order with ID {id} not found.");
                }

                var orderResponce = order.ToResponse();

                return Ok(orderResponce);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error occurred while retrieving order from the database.");
                return StatusCode(500, "An error occurred while retrieving the order.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the order.");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        // POST: Payment/{orderNumber}
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpPost("Payment/{orderNumber}")]
        public async Task<IActionResult> UpdatePaymentStatus(int orderNumber, [FromBody] bool isPaid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("400 Error (BadRequest) : Order fields are not valid");
                return BadRequest(ModelState);
            }
            try
            {
                var order = await _dbContext.Orders
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

                if (order == null)
                {
                    _logger.LogInformation("404 Error: Order with number {orderNumber} not found.", orderNumber);
                    return NotFound($"Order with OrderNumber {orderNumber} not found.");
                }

                order.Status = isPaid ? "Paid" : "Cancalled"; //Add constants

                _dbContext.Orders.Update(order);
                await _dbContext.SaveChangesAsync();

                return Ok($"Order {orderNumber} succesfully {order.Status.ToLower()}");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error occurred while updating payment status.");
                return StatusCode(500, "An error occurred while updating the payment status.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the payment status.");
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
