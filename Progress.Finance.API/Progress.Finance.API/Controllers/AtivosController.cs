using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Finance.API.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Progress.Finance.API.Controllers
{
    [Controller]
    [Route("[Controller]")]
    public class AtivosController : ControllerBase
    {
        private DataContext _dc;

        public AtivosController(DataContext context)
        {
            _dc = context;
        }

        [HttpGet]
        public async Task<ActionResult> LitarAtivos(Ativos ativo)
        {

            var listAtivos = await _dc.ativos.ToListAsync();

            if (listAtivos == null) return NotFound();

            return Ok(listAtivos);
        }



        [HttpGet("{id}")]
        public ActionResult<Ativos> ListarAtivoById(int id)
        {

            var listById = _dc.ativos.FirstOrDefault(pf => pf.IdUsuario == id);

            if (listById == null) throw new InvalidOperationException("Metas não encontradas");

            return Ok(listById);
        }


        [HttpPost]
        public async Task<ActionResult> CadastrarAtivo([FromBody] Ativos ativo)
        {
            if (ativo == null) return NotFound();


            _dc.ativos.Add(ativo);

            await _dc.SaveChangesAsync();

            return Created("Ativo criado com sucesso!", ativo); ;
        }

    }
}
