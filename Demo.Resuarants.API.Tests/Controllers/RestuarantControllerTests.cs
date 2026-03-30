using Demo.Restuarants.API.Controllers;
using Demo.Restuarants.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Demo.Resuarants.API.Tests.Controllers;

public class RestuarantControllerTests
{
    private readonly Mock<ILogger<RestuarantController>> _mockLogger;
    private readonly Mock<IRestuarantOrchestration> _mockOrchestration;
    private readonly RestuarantController _controller;

    public RestuarantControllerTests()
    {
        _mockLogger = new Mock<ILogger<RestuarantController>>();
        _mockOrchestration = new Mock<IRestuarantOrchestration>();
        _controller = new(_mockLogger.Object, _mockOrchestration.Object)
        {
            // There may be instances where http context might need to be mocked or setup
            // This covers most of the basics that might be needed
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        _controller.HttpContext.Request.Headers["device-id"] = "20317";
        _controller.HttpContext.Request.Scheme = "http";
        _controller.HttpContext.Request.Host = new HostString("localhost");
    }


    [Test]
    public async Task ListRestuarants_UnhandledException_ThrowsException()
    {
        // arrange
        FilterQueryParameters mockQueryParameters = new();
        _mockOrchestration.Setup(r => r.ListRestuarants(It.IsAny<FilterQueryParametersBO>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.ListRestuarants(mockQueryParameters));
    }

    [Test]
    public async Task ListRestuarants_QueryCompleted_ReturnsOk()
    {
        // arrange
        FilterQueryParameters mockQueryParameters = new();
        List<RestuarantBO> mockPaginatedData = [
            new("", "", "", null, "", new LocationBO("", "", "", "", "")),
            new("", "", "", null, "", new LocationBO("", "", "", "", ""))
        ];
        PaginationMetaData mockPaginatedMetaData = new(1, mockPaginatedData.Count, 1, mockPaginatedData.Count);
        PaginationResponse<RestuarantBO> mockResponse = new(mockPaginatedData, mockPaginatedMetaData);
        _mockOrchestration.Setup(r => r.ListRestuarants(It.IsAny<FilterQueryParametersBO>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _controller.ListRestuarants(mockQueryParameters);

        // assert
        Assert.That(testResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testResult, Is.TypeOf<Ok<PaginationResponse<RestuarantBO>>>());
            Assert.That(((Ok<PaginationResponse<RestuarantBO>>)testResult).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        string mockId = "12345";
        _mockOrchestration.Setup(r => r.GetRestuarant(It.IsAny<string>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.GetRestuarant(mockId));
    }

    [Test]
    public async Task GetRestuarant_RestuarantIsNull_ReturnsNotFound()
    {
        // arrange
        string mockId = "12345";
        RestuarantBO? mockResponse = null;
        _mockOrchestration.Setup(r => r.GetRestuarant(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _controller.GetRestuarant(mockId);

        // assert
        Assert.That(testResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testResult, Is.TypeOf<NotFound>());
            Assert.That(((NotFound)testResult).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task GetRestuarant_RestuarantFound_ReturnsOk()
    {
        // arrange
        string mockId = "12345";
        RestuarantBO mockResponse = new(mockId, "test", "test", null, "1112223333", new LocationBO("test", "test", "KY", "test", "12345"));
        _mockOrchestration.Setup(r => r.GetRestuarant(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _controller.GetRestuarant(mockId);

        // assert
        Assert.That(testResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testResult, Is.TypeOf<Ok<RestuarantBO>>());
            Assert.That(((Ok<RestuarantBO>)testResult).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task CreateRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        CreateRestuarantRequest mockRequest = new();
        _mockOrchestration.Setup(r => r.CreateRestuarant(It.IsAny<CreateRestuarantRequestBO>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.CreateRestuarant(mockRequest));
    }

    [Test]
    public async Task CreateRestuarant_CreateSuccessful_ReturnsCreated()
    {
        // arrange
        CreateRestuarantRequest mockRequest = new();
        RestuarantBO mockResponse = new("12345", "test", "test", null, "1112223333", new LocationBO("test", "test", "KY", "test", "12345"));
        _mockOrchestration.Setup(r => r.CreateRestuarant(It.IsAny<CreateRestuarantRequestBO>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _controller.CreateRestuarant(mockRequest);

        // assert
        Assert.That(testResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testResult, Is.TypeOf<Created<RestuarantBO>>());
            Assert.That(((Created<RestuarantBO>)testResult).StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        });
    }

    [Test]
    public async Task CreateManyRestuarants_UnhandledException_ThrowsException()
    {
        // arrange
        CreateRestuarantRequest[] mockRequest = [
            new CreateRestuarantRequest(),
            new CreateRestuarantRequest()
        ];
        _mockOrchestration.Setup(r => r.CreateManyRestuarants(It.IsAny<CreateRestuarantRequestBO[]>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.CreateManyRestuarants(mockRequest));
    }

    [Test]
    public async Task CreateManyRestuarants_OperationComplete_ReturnsOk()
    {
        // arrange
        CreateRestuarantRequest[] mockRequest = [
            new CreateRestuarantRequest(),
            new CreateRestuarantRequest()
        ];
        TransactionResult mockResponse = new(true, true, mockRequest.Length, mockRequest.Length);
        _mockOrchestration.Setup(r => r.CreateManyRestuarants(It.IsAny<CreateRestuarantRequestBO[]>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _controller.CreateManyRestuarants(mockRequest);

        // assert
        Assert.That(testResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testResult, Is.TypeOf<Ok<TransactionResult>>());
            Assert.That(((Ok<TransactionResult>)testResult).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task UpdateRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        string mockId = "12345";
        UpdateRestuarantRequest mockRequest = new();
        _mockOrchestration.Setup(r => r.UpdateRestuarant(It.IsAny<string>(), It.IsAny<UpdateRestuarantRequestBO>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.UpdateRestuarant(mockId, mockRequest));
    }

    [Test]
    public async Task UpdateRestuarant_UpdateCompletes_ReturnsOk()
    {
        // arrange
        string mockId = "12345";
        UpdateRestuarantRequest mockRequest = new();
        bool mockResponse = true;
        _mockOrchestration.Setup(r => r.UpdateRestuarant(It.IsAny<string>(), It.IsAny<UpdateRestuarantRequestBO>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _controller.UpdateRestuarant(mockId, mockRequest);

        // assert
        Assert.That(testResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testResult, Is.TypeOf<Ok<bool>>());
            Assert.That(((Ok<bool>)testResult).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task RemoveRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        string mockId = "12345";
        _mockOrchestration.Setup(r => r.RemoveRestuarant(It.IsAny<string>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.RemoveRestuarant(mockId));
    }

    [Test]
    public async Task RemoveRestuarant_RemoveCompletes_ReturnsOk()
    {
        // arrange
        string mockId = "12345";
        bool mockResponse = true;
        _mockOrchestration.Setup(r => r.RemoveRestuarant(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _controller.RemoveRestuarant(mockId);

        // assert
        Assert.That(testResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testResult, Is.TypeOf<Ok<bool>>());
            Assert.That(((Ok<bool>)testResult).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        });
    }
}
