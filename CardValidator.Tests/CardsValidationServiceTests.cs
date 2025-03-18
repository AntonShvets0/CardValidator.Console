using CardValidator.Console.Models.Entities;
using CardValidator.Console.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace CardValidator.Tests;

public class CardsValidationServiceTests
{
    private readonly CardsValidationService _cardValidationService;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IOptions<PaymentServiceOptions>> _optionsMock;

    public CardsValidationServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _optionsMock = new Mock<IOptions<PaymentServiceOptions>>();
        
        var options = new PaymentServiceOptions
        {
            Host = "https://acstopay.online",
            KeyId = "47e8fde35b164e888a57b6ff27ec020f",
            SharedKey = "ac/1LUdrbivclAeP67iDKX2gPTTNmP0DQdF+0LBcPE/3NWwUqm62u5g6u+GE8uev5w/VMowYXN8ZM+gWPdOuzg==",
            Path = "/api/testassignments/pan"
        };
        
        _optionsMock.Setup(x => x.Value).Returns(options);
        
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(options.Host);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        
        _cardValidationService = new CardsValidationService(_httpClientFactoryMock.Object.CreateClient(), _optionsMock.Object);
    }

    [Theory]
    [InlineData("4111111111111111", true)]
    [InlineData("4627100101654724", true)]
    [InlineData("4486441729154030", false)]
    [InlineData("4024007123874108", false)]
    public async Task ValidateCard_ShouldReturnExpectedResult(string cardNumber, bool expectedResult)
    {
        var result = await _cardValidationService.ValidateCardAsync(cardNumber);
        Assert.Equal(expectedResult, result);
    }
}