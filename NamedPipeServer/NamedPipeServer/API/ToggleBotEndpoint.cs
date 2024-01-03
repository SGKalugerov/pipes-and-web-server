namespace NamedPipeServer.API
{
    using FastEndpoints;

    internal class ToggleBotEndpoint : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("/api/getpesho");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            //todo bot work

            await SendAsync(new()
            {
                Status = BotStatus.Running
            });
        }
    }

    internal class Request
    {
        public int ProcessId { get; set; }
    }

    internal class Response
    {
        public BotStatus Status { get; set; }
    }

    internal enum BotStatus
    {
        Running,
        Stopped
    }
}
