using NUnit.Framework;
using UnityEngine;
using Cards;

/// <summary>
/// Unit tests for CardDisplayPresenter.
/// Part of Phase 39+ - Test coverage improvement sprint.
/// Tests in default assembly to access presenters which depend on legacy types.
/// </summary>
[TestFixture]
public class CardDisplayPresenterTests
{
    private CardDisplayPresenter _presenter;
    private TestBigImageCard _testBigImageCard;

    [SetUp]
    public void SetUp()
    {
        _testBigImageCard = new TestBigImageCard();
        _presenter = new CardDisplayPresenter(_testBigImageCard.GameObject);
    }

    [TearDown]
    public void TearDown()
    {
        _testBigImageCard?.Dispose();
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WithValidGameObject_CreatesPresenter()
    {
        // Assert
        Assert.IsNotNull(_presenter);
    }

    [Test]
    public void Constructor_WithNullGameObject_DoesNotThrow()
    {
        // Arrange & Act
        var presenter = new CardDisplayPresenter(null);

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
        var presenter = new CardDisplayPresenter(null);

        // Act & Assert (should not throw even with null GameObject)
        Assert.DoesNotThrow(() => presenter.ShowCard(null));
    }

    [Test]
    public void ShowCard_WithValidCard_ActivatesGameObject()
    {
        // Arrange
        var testCard = new TestInGameCard();

        // Act
        _presenter.ShowCard(testCard);

        // Assert
        Assert.IsTrue(_testBigImageCard.IsActive);
    }

    [Test]
    public void ShowCard_WithValidCard_SetsMaterial()
    {
        // Arrange
        var testCard = new TestInGameCard();
        var expectedMaterial = testCard.MaterialCard;

        // Act
        _presenter.ShowCard(testCard);

        // Assert
        Assert.AreEqual(expectedMaterial, _testBigImageCard.ImageMaterial);
    }

    #endregion

    #region HideCard Tests

    [Test]
    public void HideCard_DeactivatesGameObject()
    {
        // Arrange - First show a card
        var testCard = new TestInGameCard();
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
        var presenter = new CardDisplayPresenter(null);

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
        var testCard = new TestInGameCard();

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
        var card1 = new TestInGameCard("Material1");
        var card2 = new TestInGameCard("Material2");

        // Act & Assert - Show first card
        _presenter.ShowCard(card1);
        Assert.AreEqual(card1.MaterialCard, _testBigImageCard.ImageMaterial);

        // Act & Assert - Show second card
        _presenter.ShowCard(card2);
        Assert.AreEqual(card2.MaterialCard, _testBigImageCard.ImageMaterial);
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
    public Material ImageMaterial { get; private set; }
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

    /// <summary>
    /// Helper to track material changes on the Image component.
    /// Called from test to check material after ShowCard.
    /// </summary>
    public void UpdateTracking()
    {
        if (_image != null)
        {
            ImageMaterial = _image.material;
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
/// Test double for InGameCard.
/// Provides minimal implementation for testing CardDisplayPresenter.
/// Since InGameCard fields are protected, we use a subclass to set them.
/// </summary>
public class TestInGameCard : InGameCard
{
    public TestInGameCard(string materialName = "TestMaterial")
    {
        // Set protected field via subclass constructor
        materialCard = new Material(Shader.Find("UI/Default"));
        if (materialCard != null)
        {
            materialCard.name = materialName;
        }
        title = "TestCard";
    }
}

#endregion
