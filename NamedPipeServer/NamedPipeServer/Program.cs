namespace NamedPipeServer
{
    using FastEndpoints;
    using FastEndpoints.Swagger;
    using Microsoft.AspNetCore.Builder;
    using System.Threading;

    class Program
    {
        static void Main()
        {
            var bld = WebApplication.CreateBuilder();
            bld.Services
               .AddFastEndpoints()
               .SwaggerDocument();

            var app = bld.Build();
            app.UseFastEndpoints()
               .UseSwaggerGen();

            PipeServer pipeServer = new PipeServer();
            Thread namedPipeServerThread = new Thread(pipeServer.StartNamedPipeServer);
            namedPipeServerThread.Start();

            app.Run();
        }
    }
}
