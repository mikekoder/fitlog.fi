using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api
{
    public interface ITokenProvider
    {
        Token GetToken();
        void UpdateToken(Token token);
    }
}
