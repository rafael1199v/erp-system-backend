namespace Erp.Purchasing.Application.Exceptions;

public class PurchasingBusinessException(string message) : InvalidOperationException(message);
