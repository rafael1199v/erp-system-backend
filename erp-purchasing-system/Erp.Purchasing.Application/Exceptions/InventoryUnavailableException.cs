namespace Erp.Purchasing.Application.Exceptions;
public sealed class InventoryUnavailableException(string message, Exception? innerException = null)
    : Exception(message, innerException);
