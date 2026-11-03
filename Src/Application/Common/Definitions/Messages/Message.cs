using Application.Common.Extensions;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Application.Common.Definitions.Messages;

public static partial class Message<T>
{
    private const char Delimiter = '.';
    private const string Prefix = "Message";
    private const string Success = nameof(Success);
    private const string Fail = nameof(Fail);

    public static string Action(MessageActionType type, bool status)
        => BuildMessage(type.ToString(), status);

    public static string Action(MessageErrorType type)
        => BuildMessage(type.ToString());

    public static string Action(MessageErrorType type, string propName)
        => BuildMessage(type.ToString(), propName);

    public static string Action(MessageErrorType type, Expression<Func<T, object?>> propExpression)
        => BuildMessage(type.ToString(), propExpression.GetPropertyName());

    public static string Action(string objectName, string type, string propName)
        => BuildMessage(type, propName, objectName);

    private static string BuildMessage(string type, bool status)
        => BuildMessage(type, status ? Success : Fail);

    /// <summary>
    /// Ex: Message.User.Required.Name
    /// Ex: Message.User.Invalid.Name
    /// Ex: Message.User.Created.Success
    /// Ex: Message.User.Created.Failed
    /// </summary>
    private static string BuildMessage(string type, string? validationStatus = null, string? objectName = null)
    {
        objectName ??= typeof(T).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? typeof(T).Name;

        var message = new StringBuilder()
            .Append(Prefix)
            .Append(Delimiter)
            .Append(objectName) // Appends the class name (e.g., "User", "Role")
            .Append(Delimiter)
            .Append(type); // Appends the action (e.g., "Required", "Invalid")

        if (validationStatus is not null)
        {
            message.Append(Delimiter).Append(validationStatus); // Appends the property name (e.g., "Name", "Age", "Success", "Failed")
        }

        return message.ToString();
    }
}

/// <summary>
/// Message action
/// </summary>
public static partial class Message<T>
{
    public static string Create(bool status = true)
        => Action(MessageActionType.Created, status);

    public static string Update(bool status = true)
        => Action(MessageActionType.Updated, status);

    public static string Delete(bool status = true)
        => Action(MessageActionType.Deleted, status);

    public static string View(bool status = true)
        => Action(MessageActionType.View, status);

    public static string Detail(bool status = true)
        => Action(MessageActionType.Detail, status);

    public static string Search(bool status = true)
        => Action(MessageActionType.Search, status);

    public static string Import(bool status = true)
        => Action(MessageActionType.Import, status);

    public static string Download(bool status = true)
        => Action(MessageActionType.Download, status);

	public static string Login(bool status = true)
		=> Action(MessageActionType.Login, status);
}

/// <summary>
/// Message error
/// </summary>
public static partial class Message<T>
{
    public static string Required(Expression<Func<T, object?>> propExpression)
        => Action(MessageErrorType.Required, propExpression);

    public static string Required(string propName)
        => Action(MessageErrorType.Required, propName);

    public static string Conflict(Expression<Func<T, object?>> propExpression)
    => Action(MessageErrorType.Conflict, propExpression);

    public static string Conflict(string propName)
        => Action(MessageErrorType.Conflict, propName);

    public static string NotFound()
        => Action(MessageErrorType.NotFound);

    public static string NotFound(Expression<Func<T, object?>> propExpression)
        => Action(MessageErrorType.NotFound, propExpression);

    public static string NotFound(string propName)
        => Action(MessageErrorType.NotFound, propName);

    public static string AlreadyExists(Expression<Func<T, object?>> propExpression)
        => Action(MessageErrorType.AlreadyExists, propExpression);

    public static string AlreadyExists(string propName)
        => Action(MessageErrorType.AlreadyExists, propName);

    public static string Duplicate(Expression<Func<T, object?>> propExpression)
        => Action(MessageErrorType.Duplicate, propExpression);

    public static string Duplicate(string propName)
        => Action(MessageErrorType.Duplicate, propName);

    public static string TooLong(Expression<Func<T, object?>> propExpression)
        => Action(MessageErrorType.TooLong, propExpression);

    public static string TooLong(string propName)
        => Action(MessageErrorType.TooLong, propName);

    public static string TooShort(Expression<Func<T, object?>> propExpression)
        => Action(MessageErrorType.TooShort, propExpression);

    public static string TooShort(string propName)
        => Action(MessageErrorType.TooShort, propName);

    public static string Invalid(Expression<Func<T, object?>> propExpression)
        => Action(MessageErrorType.Invalid, propExpression);

    public static string Invalid(string propName)
        => Action(MessageErrorType.Invalid, propName);

	public static string Invalid()
		=> Action(MessageErrorType.Invalid);

	public static string Blocked()
		=> Action(MessageErrorType.Blocked);
}