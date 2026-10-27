using System.Threading;
using System.Threading.Tasks;
using Soenneker.SendGrid.OpenApiClient;
using Soenneker.SendGrid.OpenApiClientUtil.Abstract;

namespace Soenneker.SendGrid.Contacts.Tests;

internal sealed class ContactsOpenApiClientUtil(SendGridOpenApiClient client) : ISendGridOpenApiClientUtil
{
    public ValueTask<SendGridOpenApiClient> Get(CancellationToken cancellationToken = default) => new(client);
    public void Dispose() { }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
