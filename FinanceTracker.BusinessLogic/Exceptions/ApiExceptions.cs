namespace FinanceTracker.BusinessLogic.Exceptions;

public abstract class ApiException(string message) : Exception(message)
{
    public abstract int StatusCode { get; }
}

public class NotFoundApiException(string message = "The requested resource was not found.") : ApiException(message)
{
    public override int StatusCode => 404;
}

public class ForbiddenApiException(string message = "You do not have access to this resource.") : ApiException(message)
{
    public override int StatusCode => 403;
}

public class ConflictApiException(string message) : ApiException(message)
{
    public override int StatusCode => 409;
}

public class BadRequestApiException(string message) : ApiException(message)
{
    public override int StatusCode => 400;
}

public class UnauthorizedApiException(string message = "Invalid credentials.") : ApiException(message)
{
    public override int StatusCode => 401;
}
