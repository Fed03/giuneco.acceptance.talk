using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace Acceptance.Tests;

public static class AssertionExtensions
{
    public static async Task<AndWhichConstraint<GenericCollectionAssertions<T>, T>> ContainSingleMatching<T>(
        this GenericCollectionAssertions<T> ass,
        Func<T, Task> matcher,
        string because = "",
        params object[] becauseArgs
    )
    {
        ArgumentNullException.ThrowIfNull(matcher);

        const string expectationPrefix =
            "Expected {context:collection} to contain a single item satisfying the passed matcher{reason}, ";

        bool success = Execute.Assertion
                              .BecauseOf(because, becauseArgs)
                              .WithExpectation(expectationPrefix)
                              .Given(() => ass.Subject)
                              .ForCondition(subject => subject is not null)
                              .FailWith("but collection is <null>.")
                              .Then
                              .ForCondition(subject => subject.Any())
                              .FailWith("but collection is empty.")
                              .Then
                              .ClearExpectation();

        var matches = Array.Empty<T>();
        if (success)
        {
            var actualItems = ass.Subject as ICollection<T> ?? ass.Subject.ToList();

            (T Item, bool Failed)[] matcherResults;
            string[] failures;
            using (var collScope = new AssertionScope())
            {
                matcherResults = await Task.WhenAll(
                    actualItems.Select(
                        async (element, index) =>
                        {
                            using var itemScope = new AssertionScope();

                            await matcher(element);
                            var errors = itemScope.Discard();

                            if (errors.Length > 0)
                            {
                                var itemFailures = string.Join(
                                    Environment.NewLine,
                                    errors.Select(x => x.IndentLines().TrimEnd('.'))
                                );
                                collScope.AddPreFormattedFailure(
                                    $"At index {index}:{Environment.NewLine}{itemFailures}"
                                );
                            }

                            return (Item: element, Failed: errors.Any());
                        }
                    ).ToArray()
                );
                failures = collScope.Discard();
            }

            matches = matcherResults.Where(x => !x.Failed).Select(x => x.Item).ToArray();
            var count = matches.Length;
            switch (count)
            {
                case 0:
                    string failureMessage = Environment.NewLine
                                            + string.Join(Environment.NewLine, failures.Select(x => x.IndentLines()));
                    Execute.Assertion
                           .BecauseOf(because, becauseArgs)
                           .WithExpectation(expectationPrefix + $"but none was found:{Environment.NewLine}")
                           .FailWith(failureMessage)
                           .Then
                           .ClearExpectation();
                    break;
                case > 1:
                    Execute.Assertion
                           .BecauseOf(because, becauseArgs)
                           .FailWith(
                               expectationPrefix + "but " + count.ToString(CultureInfo.InvariantCulture) +
                               " such items were found."
                           );
                    break;
                default:
                    break;
            }
        }

        return new AndWhichConstraint<GenericCollectionAssertions<T>, T>(ass, matches);
    }

    private static string IndentLines(this string @this)
    {
        return string.Join(
            Environment.NewLine,
            @this.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => $"\t{x}")
        );
    }
}