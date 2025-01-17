namespace LibraryManagment;

public class Person
{
    public string Name { get; set; }
    public string Family { get; set; }
    public string FullName { get; set; }
    public string ID { get; set; }
    public string Language { get; set; } = "Fa-ir";
    public string Role { get; set; } = "Person";

    public string nationalCode { get; set; } = "Unknown";

    public void SetNationalCode(string nationalCode)
    {
        this.nationalCode = nationalCode;
    }


    public Person(string name = "Unknown", string family = "Unknown")
    {
        Name = name;
        Family = family;
        ID = "1";
        FullName = Name + " " + Family;
    }

    public virtual string GetInfo()
    {
        return $"Name: {Name}, ID: {ID}";
    }

    public virtual void ShowDetails()
    {
        Console.WriteLine($"Name: {Name}, ID: {ID}");
    }
}

public class LibraryMember : Person
{
    public List<Book> BorrowedBooks { get; set; }
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string Role { get; set; } = "Member";

    public LibraryMember(string name, string family = "Unknown") : base(name, family)
    {
        BorrowedBooks = new List<Book>();
    }

    public override string GetInfo()
    {
        return "Member Info: " + base.GetInfo();
    }

    public string GetId()
    {
        return ID;
    }

    public string GetBaseRole()
    {
        return base.Role;
    }


    public void BorrowBook(Book book)
    {
        if (ID == "1")
        {
            return;
        }

        BorrowedBooks.Add(book);
        Console.WriteLine($"{Name} borrowed the book: {book.Title}");
    }

    public void ReturnBook(Book book)
    {
        BorrowedBooks.Remove(book);
        Console.WriteLine($"{Name} returned the book: {book.Title}");
    }

    public override void ShowDetails()
    {
        base.ShowDetails();
        Console.WriteLine("Borrowed Books:");
        foreach (var book in BorrowedBooks)
        {
            Console.WriteLine($"- {book.Title}");
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }

    public Book(string title, string author)
    {
        Title = title;
        Author = author;
    }

    public virtual string GetBookType()
    {
        return "Book";
    }
}

public class EducationalBook : Book
{
    public EducationalBook(string title, string author) : base(title, author)
    {
    }

    public override string GetBookType()
    {
        return "Educational Book";
    }
}

public class LibraryManager : Person
{
    public LibraryManager(string name, string id) : base(name)
    {
    }

    public Book AddBook(List<Book> books, Book book)
    {
        books.Add(book);
        Console.WriteLine($"Book added: {book.Title} by {book.Author}");
        return book;
    }

    public Book AddRandomEducationalBook(List<Book> books)
    {
        var book = new EducationalBook("Random Educational Book", "Random Author");
        books.Add(book);
        Console.WriteLine($"Book added: {book.Title} by {book.Author}");
        return book;
    }

    public void RemoveBook(List<Book> books, Book book)
    {
        books.Remove(book);
        Console.WriteLine($"Book removed: {book.Title} by {book.Author}");
    }

    public override void ShowDetails()
    {
        base.ShowDetails();
        Console.WriteLine("Library Manager details displayed.");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var libraryBooks = new List<Book>();
        LibraryManager manager = new LibraryManager("Alice", "M123");

        Book book1 = new Book("C# Programming", "John Doe");
        Book book2 = new Book("ASP.NET Core", "Jane Doe");

        manager.AddBook(libraryBooks, book1);
        manager.AddBook(libraryBooks, book2);

        LibraryMember member = new LibraryMember("Bob");

        member.BorrowBook(book1);

        Console.WriteLine("\nLibrary Manager Details:");
        manager.ShowDetails();

        Console.WriteLine("\nLibrary Member Details:");
        member.ShowDetails();

        member.ReturnBook(book1);

        manager.RemoveBook(libraryBooks, book1);
    }
}