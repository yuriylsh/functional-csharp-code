using LaYumba.Functional;
using System;

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
           => @this.Match<Either<TLeft,TRight>>(() => leftProducer(), right => right);

       // 2. Take a workflow where 2 or more functions that return an `Option`
       // are chained using `Bind`.
       public static Option<int> ParseInt(string s) => int.TryParse(s, out var parsedInt) ? F.Some(parsedInt) : F.None;

       public static Option<TimeSpan> ToLargeTimeSpan(int x) => x > 365 ? F.Some(TimeSpan.FromDays(x)) : F.None;

       public static Option<TimeSpan> ParseLargeTimeSpan(string s) =>
           ParseInt(s)
               .Bind(ToLargeTimeSpan);

       // Then change the first one of the functions to return an `Either`.

       // This should cause compilation to fail. Since `Either` can be
       // converted into an `Option` as we have done in the previous exercise,
       // write extension overloads for `Bind`, so that
       // functions returning `Either` and `Option` can be chained with `Bind`,
       // yielding an `Option`.


       // 3. Write a function `Safely` of type ((() → R), (Exception → L)) → Either<L, R> that will
       // run the given function in a `try/catch`, returning an appropriately
       // populated `Either`.

       // 4. Write a function `Try` of type (() → T) → Exceptional<T> that will
       // run the given function in a `try/catch`, returning an appropriately
       // populated `Exceptional`.
   }
}
