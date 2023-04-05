using FluentAssertions;
using HexMaster.UrlShortner.ShortLinks.DomainModels;
using HexMaster.UrlShortner.ShortLinks.ErrorCodes;
using HexMaster.UrlShortner.ShortLinks.Exceptions;

namespace HexMaster.UrlShortner.ShortLinks.Tests.DomainModels;

public class ShortLinkTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void WhenShortCodeIsNullOrEmpty_ItThrowsShortCodeNullOrEmptyException(string shortCode)
    {
        var shortLink = ShortLink.Create("https://link.to.endpoint");

        var action = () => shortLink.SetShortCode(shortCode);
        action.Should().Throw<ShortCodeNullOrEmptyException>()
            .WithMessage(UrlShortnerShortLinksErrorCodes.ShortCodeNullOrEmpty.Code);
    }

    [Theory]
    [InlineData("1shldfail")]
    [InlineData("a")]
    [InlineData("1")]
    [InlineData("shortcodetoolong")]
    public void WhenShortCodeIsInvalid_ItThrowsShortCodeInvalidException(string shortCode)
    {
        var shortLink = ShortLink.Create("https://link.to.endpoint");

        var action = () => shortLink.SetShortCode(shortCode);
        action.Should().Throw<ShortCodeInvalidException>()
            .WithMessage(UrlShortnerShortLinksErrorCodes.ShortLinkInvalid.Code);
    }

    [Theory]
    [InlineData("correct")]
    [InlineData("ok")]
    [InlineData("OK")]
    [InlineData("thisisthemax")]
    [InlineData("th1s1sth3max")]
    [InlineData("t1234567890x")]
    public void WhenShortCodeIsValid_TheNewShortCodeIsAccepted(string shortCode)
    {
        var expected = shortCode.ToLowerInvariant();
        var shortLink = ShortLink.Create("https://link.to.endpoint");
        shortLink.SetShortCode(shortCode);
        shortLink.ShortCode.Should().Be(expected);
    }
}