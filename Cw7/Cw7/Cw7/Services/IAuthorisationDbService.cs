using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw7.Services
{
   public  interface IAuthorisationDbService
    {

        public string Login(string Username, string Password);
    }
}
