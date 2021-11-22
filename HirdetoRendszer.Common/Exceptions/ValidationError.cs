using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Common.Exceptions
{
    public class ValidationError
    {
        public ValidationError(string key, string error)
        {
            Key = key;
            Error = error;
        }

        public string Key { get; }

        public string Error { get; }
    }
}
