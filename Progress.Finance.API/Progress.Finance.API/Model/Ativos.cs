using API.Model;
using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class Ativos
    {
        [Key]
        public int Id_Ativo { get; set; }
        public int Id_Usuario { get; set; }
        public int Id_Meta { get; set; }
        public string Nome { get; set; }
        public int Nota { get; set; }
        public decimal Recomendacao_Porcentagem { get; set; }
        public decimal Sugestao_Investimento { get; set; }
        public TipoAtivo Tipo { get; set; }
    }
}
