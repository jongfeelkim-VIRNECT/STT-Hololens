using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.OAuth2._0_RefreshToken
{
    [Serializable]
    class RefreshTokenParameters
    {
        public string client_id;
        public string client_secret;
        public string grant_type;
        public string refresh_token;
    }
}