using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraWZiemniaka
{
    public class WyjatekNieprawidlowyRuch : Exception
    {
        public WyjatekNieprawidlowyRuch(string message) : base(message) { }
    }
}
