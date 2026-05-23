namespace Erp.Purchasing.Application.Exceptions;

public class PurchasingNotFoundException(string message) : KeyNotFoundException(message);
