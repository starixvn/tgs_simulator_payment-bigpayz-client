using System.Net;
using System.Text.Json.Nodes;
using Payment.BigPayz.Client;

namespace Payment.BigPayz;

public interface IDepositHandler
{
    (Dictionary<string, string> Body, string Api, string[] KeysHash) MakeDepositRequest(DepositOrder order, BigPayzDepositSettings settings);
    BigPayzClientDepositResponse MakeResponse(DepositOrder order, HttpStatusCode statusCode, JsonNode body);

}
