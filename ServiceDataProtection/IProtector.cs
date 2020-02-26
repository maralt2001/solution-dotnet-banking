using System;

namespace ServiceDataProtection
{
    public interface IProtector
    {
        string EncryptData<T>(T data, string purpose);
        T GetDecryptData<T>(string data, string purpose);
    }
}