using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUILibrary.Models
{
    public class UserUIModel
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
    }
}
