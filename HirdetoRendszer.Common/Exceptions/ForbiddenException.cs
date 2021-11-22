﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Common.Exceptions
{
    public class ForbiddenException : BusinessException
    {
        public ForbiddenException(string message, string title = null, Uri type = null)
            : base(message, title, type)
        {
        }
    }
}
