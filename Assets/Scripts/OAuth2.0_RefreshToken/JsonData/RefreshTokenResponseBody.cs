using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.OAuth2._0_RefreshToken
{
    [Serializable]
    class RefreshTokenResponseBody
    {
        public string access_token;
        public int expires_in;
        public string scope;
        public string token_type;

        public DateTime ExpiresDate => DateTime.Now.AddSeconds(expires_in);
    }
}