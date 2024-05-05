using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    public class Mensagem
    {
        public string Emissor { get; set; }
        public string Conteudo { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
