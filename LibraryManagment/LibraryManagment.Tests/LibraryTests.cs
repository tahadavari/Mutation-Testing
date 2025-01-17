namespace LibraryManagment.Tests;

public class LibraryTests
{
    [Fact]
    public void AddBook_ShouldAddBookToLibrary()
    {
        // Arrange
        var libraryBooks = new List<Book>();
        var manager = new LibraryManager("Alice", "M123");
        var book = new Book("C# Programming", "John Doe");

        // Act
        manager.AddBook(libraryBooks, book);

        // Assert
        Assert.Contains(book, libraryBooks);
    }

    [Fact]
    public void RemoveBook_ShouldRemoveBookFromLibrary()
    {
        // Arrange
        var libraryBooks = new List<Book>();
        var manager = new LibraryManager("Alice", "M123");
        var book = new Book("C# Programming", "John Doe");
        libraryBooks.Add(book);

        // Act
        manager.RemoveBook(libraryBooks, book);

        // Assert
        Assert.DoesNotContain(book, libraryBooks);
    }

    [Fact]
    public void BorrowBook_ShouldAddBookToBorrowedBooks()
    {
        // Arrange
        var member = new LibraryMember("Bob");
        var book = new Book("C# Programming", "John Doe");

        // Act
        member.BorrowBook(book);

        // Assert
        Assert.Contains(book, member.BorrowedBooks);
    }

    [Fact]
    public void ReturnBook_ShouldRemoveBookFromBorrowedBooks()
    {
        // Arrange
        var member = new LibraryMember("Bob");
        var book = new Book("C# Programming", "John Doe");
        member.BorrowBook(book);

        // Act
        member.ReturnBook(book);

        // Assert
        Assert.DoesNotContain(book, member.BorrowedBooks);
    }

    [Fact]
    public void LibraryPerson_DontHaveIdOne_WhenInitialized()
    {
        // Arrange
        var member = new LibraryMember("Bob");

        // Act
        var actual = member.ID;

        // Assert
        Assert.NotEqual("1", actual);
    }

    [Fact]
    public void LibraryPersonMemberLanguage_MustFaIr_WhenInitialized()
    {
        // Arrange
        var member = new LibraryMember("Bob");

        // Act
        var actual = member.Language;

        // Assert
        Assert.Equal("Fa-ir", actual);
    }

    [Fact]
    public void LibraryPersonManagerLanguage_MustFaIr_WhenInitialized()
    {
        // Arrange
        var member = new LibraryManager("Bob", "M123");

        // Act
        var actual = member.Language;

        // Assert
        Assert.Equal("Fa-ir", actual);
    }

    [Fact]
    public void MemberGetInfo_ShouldReturnMemberInfo()
    {
        // Arrange
        var member = new LibraryMember("Bob");

        // Act
        var actual = member.GetInfo();

        // Assert
        Assert.Equal("Member Info: Name: Bob, ID: 1", actual);
    }

    [Fact]
    public void LibraryMemberGetId_ShouldReturnMemberId()
    {
        // Arrange
        var member = new LibraryMember("Bob");

        // Act
        var actual = member.GetId();

        // Assert
        Assert.NotEqual("1", actual);
    }

    [Fact]
    public void GetBaseRole_ShouldReturnPersonRole()
    {
        // Arrange
        var member = new LibraryMember("Bob");

        // Act
        var actual = member.GetBaseRole();

        // Assert
        Assert.Equal("Person", actual);
    }

    [Fact]
    public void FullName_ShouldBeNameAndFamily()
    {
        // Arrange
        var member = new LibraryMember("Bob", "Doe");

        // Act
        var actual = member.FullName;

        // Assert
        Assert.Equal("Bob Doe", actual);
    }

    [Fact]
    public void AddRandomEducationalBook_ShouldAddEducationalBookToLibrary()
    {
        // Arrange
        var libraryBooks = new List<Book>();
        var manager = new LibraryManager("Alice", "M123");

        // Act
        var actual = manager.AddRandomEducationalBook(libraryBooks);

        // Assert
        Assert.Equal(actual.GetBookType(), "Educational Book");
    }

    [Fact]
    public void PersonSetNationalCode_SetCorrectNationaCode()
    {
        // Arrange
        const string nationalCode = "11111";
        var person = new Person("Bob");

        // Act
        person.SetNationalCode(nationalCode);

        // Assert
        Assert.Equal(nationalCode, person.nationalCode);
    }

    [Fact]
    public void LibraryCartId_ShouldValidGuid()
    {
        // Arrange

        // Act
        var cart = new LibraryCart();

        // Assert
        Assert.NotEqual(cart.ID, "-1");
    }

    [Fact]
    public void AddRandomBook_ShouldAddBookToLibrary()
    {
        // Arrange
        var libraryBooks = new List<Book>();
        var manager = new LibraryManager("Alice", "M123");

        // Act
        var actual = manager.AddRandomBook(libraryBooks);

        // Assert
        Assert.Equal(actual.GetBookType(), "Book");
    }

    [Fact]
    public void CompareBook_ShouldCompareBooksWithProperty()
    {
        // Arrange
        var book1 = new Book("C# Programming", "John Doe");
        var book2 = new Book("C# Programming", "John Doe");

        // Act
        var actual = book1.ComparerBook(book2);

        // Assert
        Assert.False(actual);
    }
}