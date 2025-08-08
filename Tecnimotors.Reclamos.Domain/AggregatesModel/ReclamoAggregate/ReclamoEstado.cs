using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public static class ReclamoEstado
    {
        public static readonly string Registrado = "Registrado";
        public static readonly string EnProceso = "EnProceso";
        public static readonly string Atendido = "Atendido";
        public static readonly string Rechazado = "Rechazado";
        public static readonly string Cerrado = "Cerrado";
    }
}
