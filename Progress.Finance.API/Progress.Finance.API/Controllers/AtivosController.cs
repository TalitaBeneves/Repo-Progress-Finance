using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Finance.API.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Progress.Finance.API.Controllers
{
    [Controller]
    [Route("[Controller]")]
    public class AtivosController : ControllerBase
    {
        private readonly DataContext _dc;
        private static readonly HttpClient client = new HttpClient();

        public AtivosController(DataContext context)
        {
            _dc = context;
        }

        [HttpGet("LitarAtivosById/{idUsuario}")]
        public async Task<ActionResult> LitarAtivosById(int idUsuario)
        {

            var listAtivos = await _dc.ativos.Where(id => id.IdUsuario == idUsuario).ToListAsync();

            if (listAtivos == null) return BadRequest("Ativos não encontrados");

            return Ok(listAtivos);
        }


        [HttpPost]
        public async Task<ActionResult> CadastrarAtivo([FromBody] Ativos ativo)
        {
            if (ativo == null) return BadRequest("Ativo está null");


            _dc.ativos.Add(ativo);
            await _dc.SaveChangesAsync();

            return Created("Ativo criado com sucesso!", ativo); ;
        }



    }
}
