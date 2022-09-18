using System.Threading.Tasks;
using Xunit;

namespace Tests;

public class HarvesterTests : IClassFixture<HarvesterFixture>
{
    [Fact]
    public async Task Given_TooLongJokesReturnedByExternalApi_When_HarvestingNewData_Then_TooLongJokesAreFilteredOut()
    {
        Assert.True(false);
    }
    
    [Fact]
    public async Task Given_DuplicatedJokesAmongNewData_When_HarvestingNewData_Then_DuplicatedJokesAreNotSavedButNewOneAre()
    {
        Assert.True(false);
    }
    
    [Fact]
    public async Task Given_InvalidExternalApiResponse_When_HarvestingNewData_Then_HarvesterKeepRunning()
    {
        Assert.True(false);
    }

    [Fact]
    public async Task Given_ValidDataReturnedByApi_when_HarvestingNewData_Then_ValidDataIsSavedInDatabase()
    {
        Assert.True(false);
    }
}