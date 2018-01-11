using System;
using System.Collections.Generic;
using System.Linq;
using Exercises.Chapter3;
using LaYumba.Functional;

namespace Exercises.Chapter4
{
    static class Exercises
    {
        // 1 Implement Map for ISet<T> and IDictionary<K, T>. (Tip: start by writing down
        // the signature in arrow notation.)
        public static ISet<R> Map<T, R>(this ISet<T> ts, Func<T, R> f)
        {
            var result = new HashSet<R>();
            foreach (var t in ts)
            {
                result.Add(f(t));
            }

            return result;
        }

        public static IDictionary<K, R> Map<K, T, R>(this IDictionary<K, T> ts, Func<T, R> f)
        {
            var result = new Dictionary<K, R>(ts.Count);
            foreach (var t in ts)
            {
                result.Add(t.Key, f(t.Value));
            }

            return result;
        }

        // 2 Implement Map for Option and IEnumerable in terms of Bind and Return.

        // 3 Use Bind and an Option-returning Lookup function (such as the one we defined
        // in chapter 3) to implement GetWorkPermit, shown below.

        // Then enrich the implementation so that `GetWorkPermit`
        // returns `None` if the work permit has expired.

        static Option<WorkPermit> GetWorkPermit(Dictionary<string, Employee> people, string employeeId) 
            => people.Lookup(employeeId).Bind(emp => emp.WorkPermit);

        // 4 Use Bind to implement AverageYearsWorkedAtTheCompany, shown below (only
        // employees who have left should be included).

        static double AverageYearsWorkedAtTheCompany(List<Employee> employees)
        {
            return employees.Bind(emp => emp.LeftOn.Map(leftOn => YearsBetween(emp.JoinedOn, leftOn))).Average();
            double YearsBetween(DateTime start, DateTime end) => (end - start).Days / 365d;
        }
    }

    public struct WorkPermit
    {
        public string Number { get; set; }
        public DateTime Expiry { get; set; }
    }

    public class Employee
    {
        public string Id { get; set; }
        public Option<WorkPermit> WorkPermit { get; set; }

        public DateTime JoinedOn { get; }
        public Option<DateTime> LeftOn { get; }
    }
}