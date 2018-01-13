using LaYumba.Functional;
using NUnit.Framework;
using System;
using Microsoft.Rest.Serialization;

namespace Exercises.Chapter7
{
    static class Exercises
    {
        // 1. Partial application with a binary arithmethic function:
        // Write a function `Remainder`, that calculates the remainder of 
        // integer division(and works for negative input values!). 
        public static Func<int, int, int> Remainder = (dividend, divisor)
            => dividend - ((dividend / divisor) * divisor);

        // Notice how the expected order of parameters is not the
        // one that is most likely to be required by partial application
        // (you are more likely to partially apply the divisor).

        // Write an `ApplyR` function, that gives the rightmost parameter to
        // a given binary function (try to write it without looking at the implementation for `Apply`).
        // Write the signature of `ApplyR` in arrow notation, both in curried and non-curried form
        public static Func<T1, TR> ApplyR<T1, T2, TR>(this Func<T1, T2, TR> @this, T2 t2)
            => t1 => @this(t1, t2);

        // Use `ApplyR` to create a function that returns the
        // remainder of dividing any number by 5. 
        public static Func<int, int> ReminderOfDividingBy5(int x) => Remainder.ApplyR(5);

        // Write an overload of `ApplyR` that gives the rightmost argument to a ternary function
        public static Func<T1, T2, TR> ApplyR<T1, T2, T3, TR>(this Func<T1, T2, T3, TR> @this, T3 t3)
            => (t1, t2) => @this(t1, t2, t3);

        // 2. Let's move on to ternary functions. Define a class `PhoneNumber` with 3
        // fields: number type(home, mobile, ...), country code('it', 'uk', ...), and number.
        // `CountryCode` should be a custom type with implicit conversion to and from string.
        public enum PhoneType
        {
            Home,
            Mobile,
            Work
        }

        public class CountryCode
        {
            private readonly string _value;

            public CountryCode(string code) => _value = code;

            public static implicit operator CountryCode(string code) => new CountryCode(code);

            public static implicit operator string(CountryCode countryCode) => countryCode._value;

            public override string ToString() => _value;
        }

        public class PhoneNumber
        {
            public PhoneType Type { get; }

            public CountryCode Code { get; }

            public string Number { get; }

            public PhoneNumber(CountryCode code, PhoneType type, string number)
            {
                Type = type;
                Code = code;
                Number = number;
            }
        }

        // Now define a ternary function that creates a new number, given values for these fields.
        // What's the signature of your factory function? 
        static readonly Func<CountryCode, PhoneType, string, PhoneNumber> CreatePhoneNumber =
            (code, type, number) => new PhoneNumber(code, type, number);

        // Use partial application to create a binary function that creates a UK number, 
        // and then again to create a unary function that creates a UK mobile
        private static readonly Func<PhoneType, string, PhoneNumber> CreateUkPhoneNumber =
            CreatePhoneNumber.Apply((CountryCode) "UK");

        private static readonly Func<string, PhoneNumber> CreateUkMobilePhoneNubmer =
            CreateUkPhoneNumber.Apply(PhoneType.Mobile);


        // 3. Functions everywhere. You may still have a feeling that objects are ultimately 
        // more powerful than functions. Surely, a logger object should expose methods 
        // for related operations such as Debug, Info, Error? 
        // To see that this is not necessarily so, challenge yourself to write 
        // a very simple logging mechanism without defining any classes or structs. 
        // You should still be able to inject a Log value into a consumer class/function, 
        // exposing operations like Debug, Info, and Error, like so:

        //static void ConsumeLog(Log log) 
        //   => log.Info("look! no objects!");

        enum Level
        {
            Debug,
            Info,
            Error
        }
    }
}