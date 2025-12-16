using NUnit.Framework;
using UnityEngine;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Presentation.Presenters;

/// <summary>
/// Unit tests for CardDisplayPresenter.
/// Part of Phase 39+ - Test coverage improvement sprint.
/// Phase 43: Updated for migration to JDG.Presentation - uses ICardVisualService.
/// </summary>
[TestFixture]
public class CardDisplayPresenterTests
{
    private CardDisplayPresenter _presenter;
    private TestBigImageCard _testBigImageCard;
    private MockCardVisualService _mockCardVisualService;

    [SetUp]
    public void SetUp()
    {
        _testBigImageCard = new TestBigImageCard();
        _mockCardVisualService = new MockCardVisualService();
        _presenter = new CardDisplayPresenter(_testBigImageCard.GameObject, _mockCardVisualService);
    }

    [TearDown]
    public void TearDown()
    {
        _testBigImageCard?.Dispose();
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WithValidParameters_CreatesPresenter()
    {
        // Assert
        Assert.IsNotNull(_presenter);
    }

    [Test]
    public void Constructor_WithNullGameObject_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardDisplayPresenter(null, _mockCardVisualService);

        // Assert
        Assert.IsNotNull(presenter);
    }

    [Test]
    public void Constructor_WithNullCardVisualService_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardDisplayPresenter(_testBigImageCard.GameObject, null);

        // Assert
        Assert.IsNotNull(presenter);
    }

    #endregion

    #region ShowCard Tests

    [Test]
    public void ShowCard_WithNullCard_DoesNotThrow()
    {
        // Act & Assert (should not throw)
        Assert.DoesNotThrow(() => _presenter.ShowCard(null));
    }

    [Test]
    public void ShowCard_WithNullBigImageCard_DoesNotThrow()
    {
        // Arrange
        var presenter = new CardDisplayPresenter(null, _mockCardVisualService);

        // Act & Assert (should not throw even with null GameObject)
        Assert.DoesNotThrow(() => presenter.ShowCard(null));
    }

    [Test]
    public void ShowCard_WithValidCard_ActivatesGameObject()
    {
        // Arrange
        var testCard = new MockInGameCard();

        // Act
        _presenter.ShowCard(testCard);

        // Assert
        Assert.IsTrue(_testBigImageCard.IsActive);
    }

    [Test]
    public void ShowCard_WithValidCard_CallsCardVisualService()
    {
        // Arrange
        var testCard = new MockInGameCard();

        // Act
        _presenter.ShowCard(testCard);

        // Assert
        Assert.IsTrue(_mockCardVisualService.GetMaterialCalled);
        Assert.AreEqual(testCard, _mockCardVisualService.LastCard);
    }

    [Test]
    public void ShowCard_WithValidCard_SetsMaterialFromService()
    {
        // Arrange
        var testCard = new MockInGameCard();
        var expectedMaterial = new Material(Shader.Find("UI/Default"));
        _mockCardVisualService.MaterialToReturn = expectedMaterial;

        // Act
        _presenter.ShowCard(testCard);

        // Assert
        Assert.AreEqual(expectedMaterial, _testBigImageCard.ImageMaterial);
    }

    [Test]
    public void ShowCard_WhenServiceReturnsNull_DoesNotSetMaterial()
    {
        // Arrange
        var testCard = new MockInGameCard();
        _mockCardVisualService.MaterialToReturn = null;
        var originalMaterial = _testBigImageCard.ImageMaterial;

        // Act
        _presenter.ShowCard(testCard);

        // Assert - Material should remain unchanged (null)
        Assert.IsNull(_testBigImageCard.ImageMaterial);
    }

    #endregion

    #region HideCard Tests

    [Test]
    public void HideCard_DeactivatesGameObject()
    {
        // Arrange - First show a card
        var testCard = new MockInGameCard();
        _presenter.ShowCard(testCard);

        // Act
        _presenter.HideCard();

        // Assert
        Assert.IsFalse(_testBigImageCard.IsActive);
    }

    [Test]
    public void HideCard_WithNullBigImageCard_DoesNotThrow()
    {
        // Arrange
        var presenter = new CardDisplayPresenter(null, _mockCardVisualService);

        // Act & Assert
        Assert.DoesNotThrow(() => presenter.HideCard());
    }

    [Test]
    public void HideCard_CalledMultipleTimes_DoesNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _presenter.HideCard();
            _presenter.HideCard();
            _presenter.HideCard();
        });
    }

    #endregion

    #region Integration Tests

    [Test]
    public void ShowThenHide_TogglesVisibilityCorrectly()
    {
        // Arrange
        var testCard = new MockInGameCard();

        // Act & Assert - Show
        _presenter.ShowCard(testCard);
        Assert.IsTrue(_testBigImageCard.IsActive);

        // Act & Assert - Hide
        _presenter.HideCard();
        Assert.IsFalse(_testBigImageCard.IsActive);

        // Act & Assert - Show again
        _presenter.ShowCard(testCard);
        Assert.IsTrue(_testBigImageCard.IsActive);
    }

    [Test]
    public void ShowCard_MultipleTimes_UpdatesMaterialEachTime()
    {
        // Arrange
        var card1 = new MockInGameCard();
        var card2 = new MockInGameCard();
        var material1 = new Material(Shader.Find("UI/Default")) { name = "Material1" };
        var material2 = new Material(Shader.Find("UI/Default")) { name = "Material2" };

        // Act & Assert - Show first card
        _mockCardVisualService.MaterialToReturn = material1;
        _presenter.ShowCard(card1);
        Assert.AreEqual(material1, _testBigImageCard.ImageMaterial);

        // Act & Assert - Show second card
        _mockCardVisualService.MaterialToReturn = material2;
        _presenter.ShowCard(card2);
        Assert.AreEqual(material2, _testBigImageCard.ImageMaterial);
    }

    #endregion
}

#region Test Doubles

/// <summary>
/// Test double for the BigImageCard GameObject.
/// Simulates a Unity GameObject with an Image component.
/// </summary>
public class TestBigImageCard : System.IDisposable
{
    public GameObject GameObject { get; private set; }
    public bool IsActive { get; private set; }
    public Material ImageMaterial => _image?.material;
    private UnityEngine.UI.Image _image;

    public TestBigImageCard()
    {
        GameObject = new GameObject("TestBigImageCard");
        _image = GameObject.AddComponent<UnityEngine.UI.Image>();
        IsActive = false;
        GameObject.SetActive(false);

        // Track SetActive calls
        var tracker = GameObject.AddComponent<GameObjectActiveTracker>();
        tracker.OnActiveChanged += (active) =>
        {
            IsActive = active;
        };
    }

    public void Dispose()
    {
        if (GameObject != null)
        {
            Object.DestroyImmediate(GameObject);
        }
    }
}

/// <summary>
/// Helper MonoBehaviour to track GameObject active state changes.
/// </summary>
public class GameObjectActiveTracker : MonoBehaviour
{
    public event System.Action<bool> OnActiveChanged;

    private void OnEnable()
    {
        OnActiveChanged?.Invoke(true);
    }

    private void OnDisable()
    {
        OnActiveChanged?.Invoke(false);
    }
}

/// <summary>
/// Mock implementation of IInGameCard for testing.
/// </summary>
public class MockInGameCard : IInGameCard
{
    public string Title { get; set; } = "Test Card";
    public string Description { get; set; } = "Test Description";
    public string DetailedDescription { get; set; } = "Test Detailed Description";
    public JDG.Domain.CardOwner CardOwner { get; set; } = JDG.Domain.CardOwner.Player1;
    public JDG.Domain.Enums.CardType Type { get; set; } = JDG.Domain.Enums.CardType.Invocation;
    public bool Collector { get; set; } = false;
    public string VisualId { get; set; } = "test_visual_id";
}

/// <summary>
/// Mock implementation of ICardVisualService for testing.
/// Phase 43: Updated to implement all ICardVisualService interface members.
/// </summary>
public class MockCardVisualService : ICardVisualService
{
    public bool GetMaterialCalled { get; private set; }
    public bool GetMaterialByVisualIdCalled { get; private set; }
    public bool HasVisualCalled { get; private set; }
    public IInGameCard LastCard { get; private set; }
    public string LastVisualId { get; private set; }
    public Material MaterialToReturn { get; set; }
    public bool HasVisualResult { get; set; } = true;

    public object GetMaterial(IInGameCard card)
    {
        GetMaterialCalled = true;
        LastCard = card;
        return MaterialToReturn;
    }

    public object GetMaterialByVisualId(string visualId)
    {
        GetMaterialByVisualIdCalled = true;
        LastVisualId = visualId;
        return MaterialToReturn;
    }

    public bool HasVisual(IInGameCard card)
    {
        HasVisualCalled = true;
        LastCard = card;
        return HasVisualResult;
    }

    public void Reset()
    {
        GetMaterialCalled = false;
        GetMaterialByVisualIdCalled = false;
        HasVisualCalled = false;
        LastCard = null;
        LastVisualId = null;
        MaterialToReturn = null;
        HasVisualResult = true;
    }
}

#endregion
