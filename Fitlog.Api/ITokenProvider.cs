using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api
{
    public interface ITokenProvider
    {
        Token GetToken();
        void UpdateToken(Token token);
    }
}
