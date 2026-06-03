
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_OOP
{
    internal class Program
    {
        #region Question1
        /*
         Differences between class and struct (short summary):

         - class => reference type. Variables hold a reference to an object on the heap.
           Assigning one variable to another copies the reference (both point to the same object).
           Classes can be null, support inheritance and polymorphism.

         - struct => value type. Variables hold the actual data (stack or inline inside other objects).
           Assigning one variable to another copies the value (creates an independent copy).
           Structs cannot be null (unless you use nullable: MyStruct?), and they do not support
           inheritance (they can implement interfaces).

         The example below demonstrates these behaviors.
        */

        class MyClass
        {
            public int Value;
            public override string ToString() => Value.ToString();
        }

        struct MyStruct
        {
            public int Value;
            public override string ToString() => Value.ToString();
        }

        static void SetClass(MyClass c)
        {
            // This mutates the same object referenced by the caller.
            c.Value = 100;
            // Reassigning the local parameter only affects the local variable.
            c = new MyClass { Value = 200 };
        }

        static void SetStruct(MyStruct s)
        {
            // This changes only the local copy because structs are passed by value.
            s.Value = 100;
            // Reassigning the local copy does not affect the caller's instance.
            s = new MyStruct { Value = 200 };
        }

        #region Question2
        /*
         Differences between public and private access modifiers:

         - public: the member is accessible from any other code that can see the containing type.
         - private: the member is accessible only within the containing type.

         Use public to expose a type or member to callers. Use private to hide implementation
         details and enforce encapsulation.
        */

        class AccessExample
        {
            // Public field: accessible from Main or other classes
            public int PublicValue = 10;

            // Private field: only accessible inside AccessExample
            private int PrivateValue = 42;

            // Public method to expose controlled access to the private field
            public int GetPrivateValue() => PrivateValue;

            // Private method: implementation detail
            private void SecretMessage() => Console.WriteLine("Secret (private) method called");

            // Public wrapper to call the private method
            public void RevealSecret() => SecretMessage();
        }

        #endregion

        #region Question3
        static void Main(string[] args)
        {
            // Class example (reference type)
            var a = new MyClass { Value = 1 };
            var b = a; // b references the same object as a
            b.Value = 2; // mutate via b
            Console.WriteLine("Class: a.Value after b.Value = 2 => " + a.Value); // prints 2

            // Struct example (value type)
            var x = new MyStruct { Value = 1 };
            var y = x; // y is a copy of x
            y.Value = 2; // mutates only y
            Console.WriteLine("Struct: x.Value after y.Value = 2 => " + x.Value); // prints 1

            // Passing to methods
            SetClass(a);
            Console.WriteLine("Class: a.Value after SetClass(a) => " + a.Value); // prints 100

            SetStruct(x);
            Console.WriteLine("Struct: x.Value after SetStruct(x) => " + x.Value); // prints 1

            // Nullability
            MyClass maybeNull = null; // allowed for classes
            Console.WriteLine("maybeNull is null => " + (maybeNull == null));

            MyStruct? nullableStruct = null; // use nullable if you need null semantics
            Console.WriteLine("nullableStruct.HasValue => " + nullableStruct.HasValue);

            // Keep console open when running from Visual Studio without debugger auto-close
            Console.WriteLine("\nExpected output summary:\n2\n1\n100\n1");
            Console.WriteLine("Press any key to exit...");
            // Run the simple Movie Ticket Booking flow
            RunBooking();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        #endregion

        #region Question5
        /*
         Simple Movie Ticket Booking System

         - TicketType: enum with Standard, VIP, IMAX
         - Seat: value type (struct) with Row (char) and Number (int)
         - Ticket: class with MovieName (public), Type (public), Seat (public), Price (private)
           Constructors: full-info and movieName-only (defaults applied via constructor chaining)
         - Methods:
           CalcTotal(double taxPercent) -> returns total without changing original price
           ApplyDiscount(ref double discountAmount) -> consumes discount if valid and reduces Price
           PrintTicket() -> prints ticket details
        */

        enum TicketType
        {
            Standard,
            VIP,
            IMAX
        }

        struct Seat
        {
            public char Row;
            public int Number;

            public Seat(char row, int number)
            {
                Row = row;
                Number = number;
            }

            public override string ToString() => $"{Row}{Number}";
        }

        class Ticket
        {
            public string MovieName { get; private set; }
            public TicketType Type { get; set; }
            public Seat Seat { get; set; }
            private double Price;

            // Full constructor
            public Ticket(string movieName, TicketType type, Seat seat, double price)
            {
                MovieName = movieName;
                Type = type;
                Seat = seat;
                Price = price;
            }

            // Constructor with only movie name; use constructor chaining to avoid duplication
            public Ticket(string movieName)
                : this(movieName, TicketType.Standard, new Seat('A', 1), 50.0)
            {
            }

            // Returns total after tax; original Price stays unchanged
            public double CalcTotal(double taxPercent)
            {
                var taxMultiplier = 1.0 + (taxPercent / 100.0);
                return Price * taxMultiplier;
            }

            // Applies discount if valid (>0 and <= Price). If applied, consume the discount (set to 0).
            public void ApplyDiscount(ref double discountAmount)
            {
                if (discountAmount > 0 && discountAmount <= Price)
                {
                    Price -= discountAmount;
                    discountAmount = 0; // consumed
                }
                // otherwise leave discountAmount unchanged
            }

            public void PrintTicket()
            {
                Console.WriteLine("--- Ticket ---");
                Console.WriteLine("Movie: " + MovieName);
                Console.WriteLine("Type: " + Type);
                Console.WriteLine("Seat: " + Seat);
                Console.WriteLine("Price: " + Price.ToString("C"));
            }
        }

        static void RunBooking()
        {
            Console.WriteLine();
            Console.WriteLine("Movie Ticket Booking");
            Console.Write("Enter movie name: ");
            var movie = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(movie))
            {
                Console.WriteLine("No movie provided, using 'Unknown Movie'.");
                movie = "Unknown Movie";
            }

            // Ask for optional details
            Console.Write("Select ticket type (1- Standard, 2- VIP, 3- IMAX) [default 1]: ");
            var typeInput = Console.ReadLine();
            TicketType type = TicketType.Standard;
            if (int.TryParse(typeInput, out int t) && t >= 1 && t <= 3)
            {
                type = (TicketType)(t - 1);
            }

            Console.Write("Enter seat row (A-Z) [default A]: ");
            var rowInput = Console.ReadLine();
            char row = 'A';
            if (!string.IsNullOrWhiteSpace(rowInput))
            {
                row = char.ToUpper(rowInput.Trim()[0]);
            }

            Console.Write("Enter seat number [default 1]: ");
            var numInput = Console.ReadLine();
            int number = 1;
            if (int.TryParse(numInput, out int n) && n > 0) number = n;

            Console.Write("Enter price [default 50]: ");
            var priceInput = Console.ReadLine();
            double price = 50.0;
            if (double.TryParse(priceInput, out double p) && p >= 0) price = p;

            // Create ticket using full constructor
            var seat = new Seat(row, number);
            var ticket = new Ticket(movie, type, seat, price);

            // Discount
            Console.Write("Enter discount amount to apply (or leave empty): ");
            var discountInput = Console.ReadLine();
            double discount = 0.0;
            if (double.TryParse(discountInput, out double d) && d > 0) discount = d;

            ticket.ApplyDiscount(ref discount);

            // Tax
            Console.Write("Enter tax percent (e.g. 10 for 10%) [default 0]: ");
            var taxInput = Console.ReadLine();
            double tax = 0.0;
            if (double.TryParse(taxInput, out double tx)) tax = tx;

            // Print summary
            Console.WriteLine();
            ticket.PrintTicket();
            var total = ticket.CalcTotal(tax);
            Console.WriteLine($"Tax: {tax}%");
            Console.WriteLine("Total (after tax): " + total.ToString("C"));
            Console.WriteLine("Remaining discount amount (if not applied): " + discount.ToString("C"));
            Console.WriteLine();
        }

        #endregion

        }

        #region Question4
        /*
         What is a class library?

         - A class library is a compiled assembly (DLL) that contains reusable types
           (classes, structs, interfaces, enums, etc.) which can be referenced by
           applications or other libraries.

         Why do we use class libraries?

         - Reuse: share common functionality across multiple projects without copy/paste.
         - Modularity: separate concerns so each project has a focused responsibility.
         - Encapsulation: expose a clear public API while hiding implementation details.
         - Maintainability & versioning: update and version libraries independently.
         - Distribution: deliver functionality as a DLL or NuGet package for teams or public use.
        */

        #endregion

    }
}
