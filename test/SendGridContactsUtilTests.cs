using FluentAssertions;
using Soenneker.SendGrid.Contacts.Abstract;
using Soenneker.SendGrid.Contacts.Requests;
using Soenneker.Tests.FixturedUnit;
using System.Threading.Tasks;
using Soenneker.Facts.Local;
using Soenneker.SendGrid.Contacts.Responses;
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

    [LocalFact]
    public async Task Create_should_create()
    {
        var request = new SendGridContactsRequest
        {
            Contacts =
            [
                new SendGridContactRequest
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = Faker.Random.AlphaNumeric(10) + "@test.com"
                }
            ]
        };

        SendGridContactsJobResponse response = await _util.AddOrUpdate(request);
        response.JobId.Should().NotBeNull();
    }

    [LocalFact]
    public async Task Search_should_find()
    {
        SendGridContactsSearchResponse response = await _util.Search("test@test.com");
        response.Result.Should().NotBeNullOrEmpty();
    }

    [LocalFact]
    public async Task AddAndWait_should_not_throw()
    {
        var request = new SendGridContactsRequest
        {
            Contacts =
            [
                new SendGridContactRequest
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = Faker.Random.AlphaNumeric(10) + "@test.com"
                }
            ]
        };

        SendGridContactGetResponse response = await _util.AddAndWait(request);
        response.Id.Should().NotBeNull();
    }
}
