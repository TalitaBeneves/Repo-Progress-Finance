using API.Model;
using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class Perguntas
    {
        [Key]
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Pergunta { get; set; }
        public TipoAtivoParaPergunta Tipo{ get; set; }
        public bool Marked { get; set; }
    }
}
