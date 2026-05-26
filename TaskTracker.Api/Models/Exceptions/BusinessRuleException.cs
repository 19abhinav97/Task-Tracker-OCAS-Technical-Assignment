namespace TaskTracker.Api.Models.Exceptions;

// Thrown when a service-layer business rule is violated.
// The controller catches this and returns HTTP 400.
public class BusinessRuleException : Exception
{
    // Initialises the exception with a descriptive message about the violated business rule.
    public BusinessRuleException(string message) : base(message) { }
}
