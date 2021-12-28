using System;
namespace CSharpLib
{
    /// <summary>
    /// 
    /// </summary>
    public class UserNotFoundException : Exception
    {
        internal UserNotFoundException(string msg) : base(msg) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public class InvalidPasswordException : Exception
    {
        internal InvalidPasswordException(string msg) : base(msg) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public class UserAlreadyExistsException : Exception
    {
        internal UserAlreadyExistsException(string msg) : base(msg) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public class LoginErrorException : Exception
    {
        internal LoginErrorException(string msg) : base(msg) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DirectoryAlreadyExistsException : Exception
    {
        internal DirectoryAlreadyExistsException(string msg) : base(msg) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public class UnsupportedOperatingSystemException : Exception
    {
        internal UnsupportedOperatingSystemException(string msg) : base(msg) { }
    }
}
