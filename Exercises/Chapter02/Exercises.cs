using System;
using NUnit.Framework;

namespace Exercises.Chapter2
{
    // 1. Write a console app that calculates a user's Body-Mass Index:
    //   - prompt the user for her height in metres and weight in kg
    //   - calculate the BMI as weight/height^2
    //   - output a message: underweight(bmi<18.5), overweight(bmi>=25) or healthy weight
    // 2. Structure your code so that structure it so that pure and impure parts are separate
    // 3. Unit test the pure parts
    // 4. Unit test the impure parts using the HOF-based approach

    public static class Bmi
    {
        public static void Run()
        {
            Run(
                measurement =>
                {
                    Console.WriteLine("Please enter your " + measurement);
                    return double.Parse(Console.ReadLine());
                },
                Console.WriteLine);
        }

        public static void Run(Func<string, double> read, Action<string> write)
        {
            var height = read("height");
            var weight = read("weight");
            var message = GetMessage(Calculate(height, weight));
            var output = string.Empty;
            switch (message)
            {
                case "underweight":
                case "overweight":
                    output = "Based on your BMI, you are " + message;
                    break;
                case "healthy weight":
                    output = "Based on your BMI, you have " + message;
                    break;
            }

            write(output + ".");
        }

        public static double Calculate(double height, double weight)
        {
            return Math.Round(weight / (height * height), 2);
        }

        public static string GetMessage(double bmi)
        {
            switch (bmi)
            {
                case double under when under < 18.5:
                    return "underweight";
                case double over when over >= 25.0:
                    return "overweight";
                default:
                    return "healthy weight";
            }
        }
    }

    [TestFixture]
    public class BmiTests
    {
        [TestCase(1.86d, 110d, ExpectedResult = 31.8d)]
        [TestCase(1.86d, 95d, ExpectedResult = 27.46d)]
        public double Calculate_GivenWeightAndHeight_ReturnsCorrectResult(double height, double weight)
            => Bmi.Calculate(height, weight);

        [TestCase(18.5, ExpectedResult = "healthy weight")]
        [TestCase(18.4, ExpectedResult = "underweight")]
        [TestCase(24.99, ExpectedResult = "healthy weight")]
        [TestCase(25.0, ExpectedResult = "overweight")]
        [TestCase(26.1, ExpectedResult = "overweight")]
        public string GetMessage_GivenBmi_ProducesCorrectMessage(double bmi)
            => Bmi.GetMessage(bmi);


        [TestCase(2, 80, ExpectedResult = "Based on your BMI, you have healthy weight.")]
        [TestCase(2, 60, ExpectedResult = "Based on your BMI, you are underweight.")]
        [TestCase(2, 120, ExpectedResult = "Based on your BMI, you are overweight.")]
        public string Run_GivenUserInput_WritesCorrectMessage(double height, double weight)
        {
            Func<string, double> read = measurement =>
            {
                if (measurement.Equals("height")) return height;
                if (measurement.Equals("weight")) return weight;
                throw new ArgumentException($"{measurement} is not a valid value", nameof(measurement));
            };
            string result = string.Empty;
            Bmi.Run(read, msg => result = msg);
            return result;
        }
    }
}