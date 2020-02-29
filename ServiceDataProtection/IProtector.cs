using System;

namespace ServiceDataProtection
{
    public interface IProtector
    {
        string EncryptData<T>(T data);
        T GetDecryptData<T>(string data);
    }
}