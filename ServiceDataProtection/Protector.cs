using System;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace ServiceDataProtection
{
    public class Protector : IProtector
    {
        private readonly IDataProtectionProvider _provider;
        private IDataProtector _protector;

        public Protector()
        {
            _provider = new EphemeralDataProtectionProvider();

        }

        public string EncryptData<T>(T data, string purpose)
        {
            _protector = _provider.CreateProtector(purpose);

            if (data is object)
            {
                string result = JsonConvert.SerializeObject(data);
                return _protector.Protect(result);
            }
            else
            {
                return _protector.Protect(Convert.ToString(data));
            }


        }

        public T GetDecryptData<T>(string data, string purpose)
        {
            try
            {
                _protector = _provider.CreateProtector(purpose);
                string unprotectedPayload = _protector.Unprotect(data);
                return JsonConvert.DeserializeObject<T>(unprotectedPayload);

            }
            catch (Exception)
            {
                return JsonConvert.DeserializeObject<T>(string.Empty);

            }

        }

    }
}
