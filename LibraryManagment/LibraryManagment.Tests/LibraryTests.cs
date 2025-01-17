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
}