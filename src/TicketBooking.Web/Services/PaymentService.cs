namespace TicketBooking.Web.Services;

public interface IPaymentService
{
    Task<(bool Success, string TransactionId, string Message)> ExecutePaymentSimulationAsync(
        string bookingReference, decimal amount, string outcome, CancellationToken cancellationToken = default);
}

public class SimulatedPaymentService : IPaymentService
{
    private readonly ILogger<SimulatedPaymentService> _logger;

    public SimulatedPaymentService(ILogger<SimulatedPaymentService> logger)
    {
        _logger = logger;
    }

    public async Task<(bool Success, string TransactionId, string Message)> ExecutePaymentSimulationAsync(
        string bookingReference, decimal amount, string outcome, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Simulating payment of INR {Amount} for Booking {Ref} with outcome {Outcome}.",
            amount, bookingReference, outcome);

        // Small simulation delay
        await Task.Delay(250, cancellationToken);

        switch (outcome?.Trim().ToLowerInvariant())
        {
            case "failureinsufficientfunds":
                return (false, string.Empty, "Simulated Payment Failed: Insufficient funds in the selected account.");

            case "failuretimeout":
                return (false, string.Empty, "Simulated Payment Failed: Bank gateway connection timed out.");

            case "failurecarddeclined":
                return (false, string.Empty, "Simulated Payment Failed: Card declined by the issuing authority.");

            case "success":
            default:
                var txnId = "SIM-TXN-" + Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
                return (true, txnId, "Simulated Payment Completed Successfully.");
        }
    }
}
