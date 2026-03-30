using Demo.Restuarants.Core.Orchestrations;
using Demo.Restuarants.Infrastructure.MongoDb.Interfaces;

namespace Demo.Restuarants.Core.Tests.Orchestrators;

public class RestuarantOrchestrationTests
{
    private readonly Mock<ILogger<RestuarantOrchestration>> _mockLogger;
    private readonly Mock<IRestuarantRepo> _mockRepo;
    private readonly RestuarantOrchestration _orchestrator;

    public RestuarantOrchestrationTests()
    {
        _mockLogger = new Mock<ILogger<RestuarantOrchestration>>();
        _mockRepo = new Mock<IRestuarantRepo>();
        _orchestrator = new(_mockLogger.Object, _mockRepo.Object);
    }

    [Test]
    public async Task ListRestuarants_UnhandledException_ThrowsException()
    {
        // arrange
        FilterQueryParametersBO mockQueryParameters = new(null, null);
        _mockRepo.Setup(r => r.QueryRestuarants(It.IsAny<FilterQueryParametersBO>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _orchestrator.ListRestuarants(mockQueryParameters));
    }

    [Test]
    public async Task ListRestuarants_QueryCompleted_ReturnsResults()
    {
        // arrange
        FilterQueryParametersBO mockQueryParameters = new(null, null);
        List<RestuarantBO> mockPaginatedData = [
            new("", "", "", null, "", new LocationBO("", "", "", "", "")),
            new("", "", "", null, "", new LocationBO("", "", "", "", ""))
        ];
        PaginationMetaData mockPaginatedMetaData = new(1, mockPaginatedData.Count, 1, mockPaginatedData.Count);
        PaginationResponse<RestuarantBO> mockResponse = new(mockPaginatedData, mockPaginatedMetaData);
        _mockRepo.Setup(r => r.QueryRestuarants(It.IsAny<FilterQueryParametersBO>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _orchestrator.ListRestuarants(mockQueryParameters);

        // assert
        Assert.NotNull(testResult);
        Assert.That(testResult, Is.TypeOf<PaginationResponse<RestuarantBO>>());
    }

    [Test]
    public async Task GetRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        string mockId = "12345";
        _mockRepo.Setup(r => r.GetRestuarant(It.IsAny<string>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _orchestrator.GetRestuarant(mockId));
    }

    [Test]
    public async Task GetRestuarant_RestuarantNotFound_ReturnsNull()
    {
        // arrange
        string mockId = "12345";
        RestuarantBO? mockResponse = null;
        _mockRepo.Setup(r => r.GetRestuarant(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _orchestrator.GetRestuarant(mockId);

        // assert
        Assert.Null(testResult);
    }

    [Test]
    public async Task GetRestuarant_RestuarantFound_ReturnsRestuarant()
    {
        // arrange
        string mockId = "12345";
        RestuarantBO mockResponse = new(mockId, "test", "test", null, "1112223333", new LocationBO("test", "test", "KY", "test", "12345"));
        _mockRepo.Setup(r => r.GetRestuarant(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _orchestrator.GetRestuarant(mockId);

        // assert
        Assert.NotNull(testResult);
        Assert.That(testResult, Is.TypeOf<RestuarantBO>());
    }

    [Test]
    public async Task CreateRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        CreateRestuarantRequestBO mockRequest = new("test", "test", null, "1112223333", new CreateLocationRequestBO("test", "test", "KY", "test", "12345"));
        _mockRepo.Setup(r => r.CreateRestuarant(It.IsAny<RestuarantBO>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _orchestrator.CreateRestuarant(mockRequest));
    }

    [Test]
    public async Task CreateRestuarant_RestuarantFound_ReturnsRestuarant()
    {
        // arrange
        CreateRestuarantRequestBO mockRequest = new("test", "test", null, "1112223333", new CreateLocationRequestBO("test", "test", "KY", "test", "12345"));
        RestuarantBO mockResponse = new("12345", "test", "test", null, "1112223333", new LocationBO("test", "test", "KY", "test", "12345"));
        _mockRepo.Setup(r => r.CreateRestuarant(It.IsAny<RestuarantBO>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _orchestrator.CreateRestuarant(mockRequest);

        // assert
        Assert.NotNull(testResult);
        Assert.That(testResult, Is.TypeOf<RestuarantBO>());
    }

    [Test]
    public async Task CreateManyRestuarants_UnhandledException_ThrowsException()
    {
        // arrange
        CreateRestuarantRequestBO[] mockRequests =
        [
            new("test", "test", null, "1112223333", new CreateLocationRequestBO("test", "test", "KY", "test", "12345")),
            new("test", "test", null, "1112223333", new CreateLocationRequestBO("test", "test", "KY", "test", "12345"))
        ];
        _mockRepo.Setup(r => r.CreateManyRestuarants(It.IsAny<RestuarantBO[]>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _orchestrator.CreateManyRestuarants(mockRequests));
    }

    [Test]
    public async Task CreateManyRestuarants_QueryCompleted_ReturnsResults()
    {
        // arrange
        CreateRestuarantRequestBO[] mockRequests =
        [
            new("test", "test", null, "1112223333", new CreateLocationRequestBO("test", "test", "KY", "test", "12345")),
            new("test", "test", null, "1112223333", new CreateLocationRequestBO("test", "test", "KY", "test", "12345"))
        ];
        TransactionResult mockResponse = new(true, true, mockRequests.Length, mockRequests.Length);
        _mockRepo.Setup(r => r.CreateManyRestuarants(It.IsAny<RestuarantBO[]>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _orchestrator.CreateManyRestuarants(mockRequests);

        // assert
        Assert.NotNull(testResult);
        Assert.That(testResult, Is.TypeOf<TransactionResult>());
    }

    [Test]
    public async Task UpdateRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        string mockId = "12345";
        UpdateRestuarantRequestBO mockRequest = new("test", "test", null, "1112223333", new UpdateLocationRequestBO("test", "test", "KY", "test", "12345"));
        _mockRepo.Setup(r => r.GetRestuarant(It.IsAny<string>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _orchestrator.UpdateRestuarant(mockId, mockRequest));
    }

    [Test]
    public async Task UpdateRestuarant_RestuarantNotFound_ThrowsException()
    {
        // arrange
        string mockId = "12345";
        UpdateRestuarantRequestBO mockRequest = new("test", "test", null, "1112223333", new UpdateLocationRequestBO("test", "test", "KY", "test", "12345"));
        RestuarantBO? mockResponse = null;
        _mockRepo.Setup(r => r.GetRestuarant(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _orchestrator.UpdateRestuarant(mockId, mockRequest));
    }

    [Test]
    public async Task UpdateRestuarant_UpdateCompleted_ReturnsResults()
    {
        // arrange
        string mockId = "12345";
        UpdateRestuarantRequestBO mockRequest = new("test", "test", null, "1112223333", new UpdateLocationRequestBO("test", "test", "KY", "test", "12345"));
        RestuarantBO mockRestuarant = new(mockId, "test", "test", null, "1112223333", new LocationBO("test", "test", "KY", "test", "12345"));
        TransactionResult mockResponse = new(true, true, 1, 1);
        _mockRepo.Setup(r => r.GetRestuarant(It.IsAny<string>())).ReturnsAsync(mockRestuarant);
        _mockRepo.Setup(r => r.UpdateRestuarant(It.IsAny<string>(), It.IsAny<UpdateRestuarantRequestBO>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _orchestrator.UpdateRestuarant(mockId, mockRequest);

        // assert
        Assert.That(testResult, Is.TypeOf<bool>());
        Assert.True(testResult);
    }

    [Test]
    public async Task RemoveRestuarant_UnhandledException_ThrowsException()
    {
        // arrange
        string mockId = "12345";
        _mockRepo.Setup(r => r.RemoveRestuarant(It.IsAny<string>())).ThrowsAsync(new Exception());

        // act and assert
        Assert.ThrowsAsync<Exception>(async () => await _orchestrator.RemoveRestuarant(mockId));
    }

    [Test]
    public async Task RemoveRestuarant_RemoveCompleted_ReturnsResults()
    {
        // arrange
        string mockId = "12345";
        TransactionResult mockResponse = new(true, true, 1, 1);
        _mockRepo.Setup(r => r.RemoveRestuarant(It.IsAny<string>())).ReturnsAsync(mockResponse);

        // act
        var testResult = await _orchestrator.RemoveRestuarant(mockId);

        // assert
        Assert.That(testResult, Is.TypeOf<bool>());
        Assert.True(testResult);
    }
}
