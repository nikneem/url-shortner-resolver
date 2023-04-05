﻿using FluentAssertions;
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void WhenTargetUrlIsNullOrEmpty_ItThrowsShortCodeNullOrEmptyException(string shortCode)
    {
        var shortLink = ShortLink.Create("https://link.to.endpoint");

        var action = () => shortLink.SetTargetUrl(shortCode);
        action.Should().Throw<TargetUrlNullOrEmptyException>()
            .WithMessage(UrlShortnerShortLinksErrorCodes.TargetUrlNullOrEmpty.Code);
    }

    [Theory]
    [InlineData("1shldfail")]
    [InlineData("a")]
    [InlineData("1")]
    [InlineData("shortcodetoolong")]
    public void WhenTargetUrlIsInvalid_ItThrowsShortCodeInvalidException(string shortCode)
    {
        var shortLink = ShortLink.Create("https://link.to.endpoint");

        var action = () => shortLink.SetTargetUrl(shortCode);
        action.Should().Throw<TargetUrlInvalidException>()
            .WithMessage(UrlShortnerShortLinksErrorCodes.TargetUrlInvalid.Code);
    }

    [Theory]
    [InlineData("http://google.com")]
    [InlineData("https://google.com")]
    [InlineData("https://www.website.com")]
    [InlineData("https://www.website.com/bananas")]
    [InlineData("https://www.website.com/bananas/23")]
    [InlineData("https://www.website.com/bananas/23?id=45")]
    [InlineData("https://www.website.com/bananas/23?id=45&param=bike")]
    public void WhenTargetUrlIsValid_TheNewShortCodeIsAccepted(string shortCode)
    {
        var expected = shortCode.ToLowerInvariant();
        var shortLink = ShortLink.Create("https://link.to.endpoint");
        shortLink.SetTargetUrl(shortCode);
        shortLink.TargetUrl.Should().Be(expected);
    }

    [Fact]
    public void WhenExpiryDateSet_TheExpiryDateChanged()
    {
        var expected = DateTimeOffset.UtcNow;
        var shortLink = ShortLink.Create("https://link.to.endpoint");
        shortLink.SetExpiryDate(expected);
        shortLink.ExpiresOn.Should().Be(expected);
    }

    [Fact]
    public void WhenExpiryDateChanged_TheExpiryDateChanged()
    {
        var start = DateTimeOffset.UtcNow.AddDays(-2);
        var expected = DateTimeOffset.UtcNow;
        var shortLink = new ShortLink(Guid.NewGuid(), "abcdef", "https://link.to.endpoint", start, start);
        shortLink.SetExpiryDate(expected);
        shortLink.ExpiresOn.Should().Be(expected);
    }

    [Fact]
    public void WhenExpiryDateEmptied_TheExpiryDateChanged()
    {
        var start = DateTimeOffset.UtcNow.AddDays(-2);
        var shortLink = new ShortLink(Guid.NewGuid(), "abcdef", "https://link.to.endpoint", start, start);
        shortLink.SetExpiryDate(null);
        shortLink.ExpiresOn.Should().Be(null);
    }
}