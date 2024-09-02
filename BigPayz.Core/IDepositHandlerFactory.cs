namespace Payment.BigPayz;

public interface IDepositHandlerFactory
{
    IDepositHandler GetDepositHandler(string channelCode);
}