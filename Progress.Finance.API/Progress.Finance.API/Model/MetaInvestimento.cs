using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class MetaInvestimento
    {
        [Key]
        public int IdMeta { get; set; }
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public decimal Acoes { get; set; }
        public decimal Fiis { get; set; }
        public decimal RendaFixa { get; set; }
    }
}
