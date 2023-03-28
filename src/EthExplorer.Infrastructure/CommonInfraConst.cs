namespace EthExplorer.Infrastructure;

public class CommonInfraConst
{
    public const string DAPR_SECRETSTORE_NAME = "secretstore";
    public const string DAPR_STATESTORE_NAME = "statestore";
    public const string DAPR_PUBSUP_NAME = "pubsub";
    public const string API_APP_ID = "api";

    public static readonly decimal DIAGNOSTIC_METHOD_ELAPSED_SECONDS_WARN_THRESHOLD = 10.0M;
}