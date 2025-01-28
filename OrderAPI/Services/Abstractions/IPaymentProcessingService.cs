public interface IPaymentProcessingService
{
    Task PublishPaymentUpdate(int orderId);
    Task ProcessPayment(int orderId);
    void ListenForPaymentUpdates();
}
