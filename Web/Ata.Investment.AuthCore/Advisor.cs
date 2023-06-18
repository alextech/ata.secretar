using System;
using Microsoft.AspNetCore.Identity;

namespace Ata.Investment.AuthCore
{
    public class Advisor : IdentityUser
    {

        public string Name { get; set; } = "";
        public string Credentials { get; set; } = "";

    }
}