using API.Model;
using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class Ativos
    {
        [Key]
        public int Id_Ativo { get; set; }
        public int Id_Usuario { get; set; }
        public int Id_Metas { get; set; }
        public string Nome { get; set; }
        public int Nota { get; set; }
        public decimal RecomendacaoPorcentagem { get; set; }
        public decimal SugestaoInvestimento { get; set; }
        public TipoAtivo TipoAtivo { get; set; }
    }
}
