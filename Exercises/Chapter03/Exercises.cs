using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
//using System.Configuration;
using LaYumba.Functional;
using NUnit.Framework;
using Enum = System.Enum;

namespace Exercises.Chapter3
{
    public static class Exercises
    {
        // 1 Write a generic function that takes a string and parses it as a value of an enum. It
        // should be usable as follows:

        // Enum.Parse<DayOfWeek>("Friday") // => Some(DayOfWeek.Friday)
        // Enum.Parse<DayOfWeek>("Freeday") // => None
        public static Option<T> Parse<T>(string input)
        {
            return Enum.TryParse(typeof(T), input, out var parseResult)
                ? F.Some((T) parseResult)
                : F.None;
        }


        // 2 Write a Lookup function that will take an IEnumerable and a predicate, and
        // return the first element in the IEnumerable that matches the predicate, or None
        // if no matching element is found. Write its signature in arrow notation:

        // bool isOdd(int i) => i % 2 == 1;
        // new List<int>().Lookup(isOdd) // => None
        // new List<int> { 1 }.Lookup(isOdd) // => Some(1)
        public static Option<T> Lookup<T>(IEnumerable<T> input, Func<T, bool> predicate)
        {
            if (input != null && predicate != null)
            {
                foreach (var val in input)
                {
                    if (predicate(val)) return val;
                }
            }

            return F.None;
        }

        // 3 Write a type Email that wraps an underlying string, enforcing that it’s in a valid
        // format. Ensure that you include the following:
        // - A smart constructor
        // - Implicit conversion to string, so that it can easily be used with the typical API
        // for sending emails

        public class Email
        {
            private readonly string _email;

            private Email(string email) => _email = email;

            public static Option<Email> Of(string email) 
                => IsValid(email) ? F.Some(new Email(email)) : F.None;

            private static bool IsValid(string email)
                => !string.IsNullOrEmpty(email) && Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

            public static implicit operator string(Email email) => email._email;
        }

        // 4 Take a look at the extension methods defined on IEnumerable inSystem.LINQ.Enumerable.
        // Which ones could potentially return nothing, or throw some
        // kind of not-found exception, and would therefore be good candidates for
        // returning an Option<T> instead?
    }

    [TestFixture]
    public class EnumPerserTests
    {
        [TestCase("Monday", DayOfWeek.Monday)]
        [TestCase("Tuesday", DayOfWeek.Tuesday)]
        [TestCase("Wednesday", DayOfWeek.Wednesday)]
        [TestCase("Thursday", DayOfWeek.Thursday)]
        [TestCase("Friday", DayOfWeek.Friday)]
        [TestCase("Saturday", DayOfWeek.Saturday)]
        [TestCase("Sunday", DayOfWeek.Sunday)]
        public void Parse_ValidDayOfWeekString_ReturnsSomeWithCorrespondingEnumValue(string input, DayOfWeek dayOfWeek)
            => Assert.AreEqual(F.Some(dayOfWeek), Exercises.Parse<DayOfWeek>(input));

        [TestCase(null)]
        [TestCase("")]
        [TestCase("some random string")]
        public void Parse_InvaidDayOfWeekString_ReturnsNone(string input)
            => Assert.AreEqual(F.None, Exercises.Parse<DayOfWeek>(input));
    }

    [TestFixture]
    public class LookupTests
    {
        [Test]
        public void Lookup_ItemIsInCollection_ReturnsFirstItemMatchingPredicate()
            => Assert.AreEqual(F.Some(1), Exercises.Lookup(new[] {1, 2, 3}, OneOrThreePredicate));

        private static bool OneOrThreePredicate(int x) => x == 3 || x == 1;

        [Test]
        public void Lookup_ItemIsNotInCollecton_ReturnsNone()
            => Assert.AreEqual(F.None, Exercises.Lookup(new[] {1, 2, 3}, _ => false));

        [Test]
        public void Lookup_CollectionIsNull_ReturnsNone()
            => Assert.AreEqual(F.None, Exercises.Lookup(null, (Func<int, bool>) (_ => true)));

        [Test]
        public void Lookup_PredicateIsNull_ReturnsNone()
            => Assert.AreEqual(F.None, Exercises.Lookup(new[] {1, 2, 3}, null));
    }

    [TestFixture]
    public class EmailTests
    {
        [Test]
        public void Of_GivenValidEmail_ReturnsEmail()
            => Exercises.Email.Of("valid@email.com").Match(
                () => Assert.Fail("Expected Some(Email(valid@email.com)) but was None"),
                email => Assert.AreEqual("valid@email.com", (string)email));

        [TestCase(null)]
        [TestCase("")]
        [TestCase("not a valid email address")]
        [TestCase("not a valid@email.address")]
        [TestCase("not_a_valid@email address")]
        [TestCase("not@a@valid@email.address")]
        public void Of_GivenInvalidEmail_ReturnsNone(string email)
            => Assert.AreEqual(F.None, Exercises.Email.Of(email));
    }

    // 5.  Write implementations for the methods in the `AppConfig` class
    // below. (For both methods, a reasonable one-line method body is possible.
    // Assume settings are of type string, numeric or date.) Can this
    // implementation help you to test code that relies on settings in a
    // `.config` file?
    public class AppConfig
    {
        NameValueCollection source;

        //public AppConfig() : this(ConfigurationManager.AppSettings) { }

        public AppConfig(NameValueCollection source)
        {
            this.source = source;
        }

        public Option<T> Get<T>(string name)
        {
            var value = source[name];
            return value == null ? F.None : ConvertToType<T>(value);
        }

        public T Get<T>(string name, T defaultValue)
            => Get<T>(name).Match(() => defaultValue, value => value);

        private static Option<T> ConvertToType<T>(object value)
        {
            try
            {
                var convertedValue = (T) Convert.ChangeType(value, typeof(T));
                return convertedValue;
            }
            catch
            {
                return F.None; 
            }
        }
    }
}