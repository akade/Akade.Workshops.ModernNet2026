using System;

namespace ModernizeMe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Library Management");

            var library = new Library();
            AddSampleData(library);
            Console.WriteLine();

            library.Now = new DateTime(2026, 1, 1);
            Console.WriteLine();

            var charlie = new Member("Charlie");
            var book = new Book("Patterns of Enterprise Application Architecture", "Martin Fowler");

            library.RegisterMember(charlie);
            library.AddItem(book);
            Console.WriteLine();

            var loan = library.TryLoanItem(charlie, book);
            Console.WriteLine(loan != null
                ? string.Format("Loan successful: {0} (due {1:d})", book, loan.ReturnDate)
                : string.Format("Loan failed: {0}", book));

            Console.WriteLine();
            library.Now = library.Now.AddDays(35);
            Console.WriteLine();

            decimal? requiredFees;
            var returnWithoutFees = library.TryReturnItem(charlie, book, false, out requiredFees);
            Console.WriteLine(returnWithoutFees
                ? string.Format("Return successful. Fees paid: {0:C}", requiredFees ?? 0m)
                : string.Format("Return failed. Required fees: {0:C}", requiredFees ?? 0m));

            var returned = library.TryReturnItem(charlie, book, true, out requiredFees);
            Console.WriteLine(returned
                ? string.Format("Return successful. Fees paid: {0:C}", requiredFees ?? 0m)
                : string.Format("Return failed. Required fees: {0:C}", requiredFees ?? 0m));

            var secondLoan = library.TryLoanItem(charlie, book);
            Console.WriteLine(secondLoan != null
                ? string.Format("Second loan successful: {0}", book)
                : string.Format("Second loan failed: {0}", book));

            Console.WriteLine();
            Console.WriteLine(library.GetLoanSummary(charlie));
            Console.WriteLine(library.GetLoanSummary(new Member("Alice")));
        }

        private static void AddSampleData(Library library)
        {
            if (library == null)
            {
                throw new ArgumentNullException("library");
            }


            library.RegisterMember(new Member("Alice"));
            library.RegisterMember(new Member("Bob"));
            library.RegisterMember(new Member("Eve"));

            library.AddItem(new Book("Clean Code", "Robert C. Martin"));
            library.AddItem(new Book("Domain-Driven Design", "Eric Evans"));
            library.AddItem(new Book("Refactoring", "Martin Fowler"));
            library.AddItem(new Book("The Pragmatic Programmer", "Andrew Hunt and David Thomas"));
            library.AddItem(new Book("Working Effectively with Legacy Code", "Michael Feathers"));
            library.AddItem(new Magazine(".NET Weekly", 42));
        }
    }
}
