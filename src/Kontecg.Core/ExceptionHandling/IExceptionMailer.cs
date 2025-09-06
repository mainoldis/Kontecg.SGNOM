using System;
using Kontecg.Runtime.Session;
using Kontecg.Storage;

namespace Kontecg.ExceptionHandling
{
    public interface IExceptionMailer
    {
        void Send(IKontecgSession kontecgSession, Exception exception, string message, TempFileInfo attachment = null);
    }
}
