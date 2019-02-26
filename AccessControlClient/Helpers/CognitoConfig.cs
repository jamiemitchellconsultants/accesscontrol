using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlClient.Helper
{
    public class CognitoConfig
    {
        public string ResponseType { get; set; }
        public string MetadataAddress { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

    }
}
