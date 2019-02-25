using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Jwtissuer
    {
        public string JwtissuerId { get; set; }
        public string IssuerName { get; set; }
        public string SubjectClaimName { get; set; }
    }
}
