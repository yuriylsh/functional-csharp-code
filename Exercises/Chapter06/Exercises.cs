using LaYumba.Functional;
using System;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using NSubstitute;
using NUnit.Framework;

namespace Exercises.Chapter6
{
    static class Exercises
    {
        // 1. Write a `ToOption` extension method to convert an `Either` into an
        // `Option`. Then write a `ToEither` method to convert an `Option` into an
        // `Either`, with a suitable parameter that can be invoked to obtain the
        // appropriate `Left` value, if the `Option` is `None`. (Tip: start by writing
        // the function signatures in arrow notation)
        public static Option<TRight> ToOption<TLeft, TRight>(this Either<TLeft, TRight> @this)
            => @this.Match(_ => F.None, F.Some);

        public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this Option<TRight> @this, Func<TLeft> leftProducer)
            => @this.Match<Either<TLeft, TRight>>(() => leftProducer(), right => right);

        // 2. Take a workflow where 2 or more functions that return an `Option`
        // are chained using `Bind`.
        // Then change the first one of the functions to return an `Either`.
        // This should cause compilation to fail. Since `Either` can be
        // converted into an `Option` as we have done in the previous exercise,
        // write extension overloads for `Bind`, so that
        // functions returning `Either` and `Option` can be chained with `Bind`,
        // yielding an `Option`.
        public static Either<string, int> ParseInt(string s)
        {
            if (int.TryParse(s, out var parsedInt))
            {
                return parsedInt;
            }

            return $"Cannot convert {s} to integer";
        }

        public static Option<TimeSpan> ToLargeTimeSpan(int x) => x > 365 ? F.Some(TimeSpan.FromDays(x)) : F.None;

        public static Option<TimeSpan> ParseLargeTimeSpan(string s) =>
            ParseInt(s)
                .Bind(ToLargeTimeSpan);

        public static Option<TOption> Bind<TLeft, TRight, TOption>(this Either<TLeft, TRight> @this,
            Func<TRight, Option<TOption>> f)
            => @this.Match(left => F.None, f);


        [TestFixture]
        public class ExercisesTest
        {
            [TestCase("not integer", null)]
            [TestCase("365", null)]
            [TestCase("366", 366)]
            public void ParseLargeTimeSpan_ReturnsCorrectOption(string input, int? expectedDays)
            {
                var result = ParseLargeTimeSpan(input);

                result.Match(FailNoneIfNotNull, FailSomeIfNull);

                void FailNoneIfNotNull()
                {
                    if(expectedDays.HasValue) Assert.Fail($"Expected Some({expectedDays.Value:d}d) but found None instead.");
                }

                void FailSomeIfNull(TimeSpan largeTimeSpan)
                {
                    if (!expectedDays.HasValue)
                        ((object)largeTimeSpan).Should().Be(F.None);
                    else
                        largeTimeSpan.Should().Be(TimeSpan.FromDays(expectedDays.Value));
                }
                
            }
        }


        // 3. Write a function `Safely` of type ((() → R), (Exception → L)) → Either<L, R> that will
        // run the given function in a `try/catch`, returning an appropriately
        // populated `Either`.

        public static Either<TLeft,TRight> Safely<TLeft,TRight>(Func<TRight> right, Func<Exception, TLeft> left)
        {
            try
            {
                return right();
            }
            catch (Exception e)
            {
                return left(e);
            }
        }

        [TestFixture]
        public class SafelyTest
        {
            [Test]
            public void Safely_NonThrowing_ReturnsRight()
            {
               var result = Safely(() => 100, ex => ex.Message);

               result.Match(errorMessage => Assert.Fail() , x => x.Should().Be(100));
            }
            [Test]
            public void Safely_Throwing_ReturnsLeft()
            {
               var result = Safely<string, int>(() => throw new Exception("I'm failing"), ex => ex.Message);

               result.Match(ex => { ex.Should().Be("I'm failing");}, x => Assert.Fail());
            }
        }

        // 4. Write a function `Try` of type (() → T) → Exceptional<T> that will
        // run the given function in a `try/catch`, returning an appropriately
        // populated `Exceptional`.

        public static Exceptional<T> Try<T>(Func<T> f)
        {
            try
            {
                return F.Exceptional(f());
            }
            catch (Exception e)
            {
                return Exceptional.Of<T>(e);
            }
        }

        [TestFixture]
        public class Try_Test
        {
            [Test]
            public void Try_NoException_ReturnsNonExceptionalValue()
            {
                Exceptional<int> result = Try(() => 123);

                result.Match(ex => Assert.Fail(), x => x.Should().Be(123));
            }
            [Test]
            public void Try_Exception_ReturnsExceptionalValue()
            {
                var thrownException = new Exception("I'll be thrown");
                Exceptional<int> result = Try<int>(() => throw thrownException);

                result.Match(ex => ex.Should().Be(thrownException), x => Assert.Fail());
            }
        }
    }
}