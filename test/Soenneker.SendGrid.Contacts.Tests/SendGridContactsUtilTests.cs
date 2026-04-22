using Soenneker.SendGrid.Contacts.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.SendGrid.Contacts.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class SendGridContactsUtilTests : HostedUnitTest
{
    private readonly ISendGridContactsUtil _util;

    public SendGridContactsUtilTests(Host host) : base(host)
    {
        _util = Resolve<ISendGridContactsUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
