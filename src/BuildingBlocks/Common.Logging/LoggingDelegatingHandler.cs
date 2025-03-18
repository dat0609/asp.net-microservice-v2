using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Common.Logging;

public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> _logger;

    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Sending request to {request.RequestUri}" +
                                   $"- Method: {request.Method}" +
                                   $"- Version: {request.Version}");

            var response = await base.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Received a success response from {response.RequestMessage.RequestUri}");
            }
            else
            {
                _logger.LogWarning($"Received non success status code {response.StatusCode}");
            }
        }
        catch (HttpRequestException e)
        when(e.InnerException is SocketException{SocketErrorCode: SocketError.ConnectionRefused})
        {
            var hostWithPort = request.RequestUri.IsDefaultPort
                ? request.RequestUri.DnsSafeHost
                : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";
            
            _logger.LogCritical($"Unable to connect the host {e}", e);
        }

        return new HttpResponseMessage(HttpStatusCode.BadGateway);
    }
}