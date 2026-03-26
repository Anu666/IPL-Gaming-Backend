using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        [Route("GetAllTransactions")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactions();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetTransactionById/{transactionId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> GetTransactionById(Guid transactionId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionById(transactionId);
                if (transaction == null)
                {
                    return NotFound(new { message = $"Transaction with ID {transactionId} not found" });
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetTransactionsByUserId/{userId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> GetTransactionsByUserId(Guid userId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByUserId(userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetTransactionByMatchAndUser/{matchId}/{userId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> GetTransactionByMatchAndUser(Guid matchId, Guid userId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByMatchAndUser(matchId, userId);
                if (transaction == null)
                {
                    return NotFound(new { message = $"No transaction found for user {userId} in match {matchId}" });
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CreateTransaction")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            try
            {
                if (transaction == null)
                {
                    return BadRequest(new { message = "Transaction data is required" });
                }

                if (transaction.UserId == Guid.Empty)
                {
                    return BadRequest(new { message = "A valid User ID is required" });
                }

                if (transaction.MatchId == Guid.Empty)
                {
                    return BadRequest(new { message = "A valid Match ID is required" });
                }

                if (transaction.Changes == null || transaction.Changes.Count == 0)
                {
                    return BadRequest(new { message = "At least one change entry is required" });
                }

                var created = await _transactionService.CreateTransaction(transaction);
                return CreatedAtAction(nameof(GetTransactionById), new { transactionId = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateTransaction")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateTransaction([FromBody] Transaction transaction)
        {
            try
            {
                if (transaction == null || transaction.Id == Guid.Empty)
                {
                    return BadRequest(new { message = "Transaction data with valid ID is required" });
                }

                if (transaction.UserId == Guid.Empty)
                {
                    return BadRequest(new { message = "A valid User ID is required" });
                }

                var updated = await _transactionService.UpdateTransaction(transaction);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteTransaction/{transactionId}/{userId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> DeleteTransaction(Guid transactionId, Guid userId)
        {
            try
            {
                var result = await _transactionService.DeleteTransaction(transactionId, userId);
                if (!result)
                {
                    return NotFound(new { message = $"Transaction with ID {transactionId} not found or could not be deleted" });
                }
                return Ok(new { message = "Transaction deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
