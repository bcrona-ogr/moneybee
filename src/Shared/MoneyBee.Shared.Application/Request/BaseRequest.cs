namespace MoneyBee.Shared.Application.Request;

public abstract class BaseRequest
{
    public string CorrelationId { get; set; } = null;
}