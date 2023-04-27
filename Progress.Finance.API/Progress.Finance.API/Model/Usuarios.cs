using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public string ImagemUrl { get; set; }
    }

    public class UsuarioAtivos
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdAtivo { get; set; }
        public decimal ValoTotalInvestido { get; set; }
    }
    public class UsuarioMetaInvestimento
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdMeta { get; set; }

    }
}