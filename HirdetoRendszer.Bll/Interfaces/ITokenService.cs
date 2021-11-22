using HirdetoRendszer.Dal.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface ITokenService
    {
        public Task<string> CreateAccessToken(Felhasznalo user);
    }
}
