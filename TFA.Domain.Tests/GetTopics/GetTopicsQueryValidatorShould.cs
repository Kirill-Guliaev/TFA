using FluentAssertions;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsQueryvalidatorShould
{
    private readonly GetTopicsValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenQueryIsvalid()
    {
        var query = new GetTopicsQuery(Guid.Parse("b63b4a0f-10c1-4610-bf31-23f7dae23850"), 10, 5);
        sut.Validate(query).IsValid.Should().BeTrue();  
    }

    public static IEnumerable<object[]> GetInvalidQuery()
    {
        var query = new GetTopicsQuery(Guid.Parse("b63b4a0f-10c1-4610-bf31-23f7dae23850"), 10, 5);
        yield return new object[] { query with { ForumId = Guid.Empty } };
        yield return new object[] { query with { Skip = -5} };
        yield return new object[] { query with { Take = -42} };
    }

    [Theory]
    [MemberData(nameof(GetInvalidQuery))]
    public void ReturFailure_WhenQueryIsInvalid(GetTopicsQuery query)
    {
        sut.Validate(query).IsValid.Should().BeFalse();
    }
}
