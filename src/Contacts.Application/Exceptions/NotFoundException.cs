namespace Contacts.Application.Exceptions;

public class NotFoundException(string message) : Exception(message);