using API.Model;
using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class Ativos
    {
        [Key]
        public int IdAtivo { get; set; }
        public int IdUsuario { get; set; }
        public int IdMeta { get; set; }
        public string Nome { get; set; }
        public int Nota { get; set; }
        public decimal RecomendacaoPorcentagem { get; set; }
        public decimal SugestaoInvestimento { get; set; }
        public TipoAtivo TipoAtivo { get; set; }
    }
}
