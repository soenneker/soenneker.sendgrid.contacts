using Soenneker.SendGrid.Contacts.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.SendGrid.Contacts.Tests;

[Collection("Collection")]
public class SendGridContactsUtilTests : FixturedUnitTest
{
    private readonly ISendGridContactsUtil _util;

    public SendGridContactsUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ISendGridContactsUtil>(true);
    }
}
