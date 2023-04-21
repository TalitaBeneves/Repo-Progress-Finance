using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class MetaInvestimento
    {
        [Key]
        public int Id_Meta { get; set; }
        public int Id_Usuario { get; set; }
        public string Nome { get; set; }
        public int Acoes { get; set; }
        public int Fiis { get; set; }
        public int Renda_Fixa { get; set; }
    }
}
