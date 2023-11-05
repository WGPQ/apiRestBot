using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Configuracion
{
    public class MailConfiguracion
    {
        public string Mail { get; set; }
        public string NombreEnvia { get; set; }
        public string Contrasenia { get; set; }
        public string Host { get; set; }
        public int Puerto { get; set; }
    }
}
