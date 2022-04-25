using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class User
    {
        public string UserName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<Game> Favorites { get; set; }
        public bool Active { get; set; }
    }
}
