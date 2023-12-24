using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Zion.Models.User
{
    public class UserInfoModel : ZionModel
    {
        public int UserNO { get; set; }
        public string Name { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
