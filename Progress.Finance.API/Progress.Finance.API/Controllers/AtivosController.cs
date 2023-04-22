using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Finance.API.Model;
using System;
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
