namespace LibraryManagment.Tests;

public class UnitTest1
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
        var member = new LibraryMember("Bob", "M456");
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
        var member = new LibraryMember("Bob", "M456");
        var book = new Book("C# Programming", "John Doe");
        member.BorrowBook(book);

        // Act
        member.ReturnBook(book);

        // Assert
        Assert.DoesNotContain(book, member.BorrowedBooks);
    }
}