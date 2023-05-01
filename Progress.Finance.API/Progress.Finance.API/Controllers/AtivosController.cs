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


        [HttpPost("CadastrarAtivo")]
        public async Task<ActionResult> CadastrarAtivo([FromBody] Ativos ativo)
        {
            if (ativo == null) return BadRequest("Ativo está null");

            var verificaAtivo = await _dc.ativos.Where(i => i.Nome == ativo.Nome).FirstOrDefaultAsync();
            
            if (verificaAtivo != null)
                return BadRequest("Esse ativo já esta cadastrado");
            

            _dc.ativos.Add(ativo);
            await _dc.SaveChangesAsync();

            return Created("Ativo criado com sucesso!", ativo); ;
        }

        [HttpPut("EditarAtivo")]
        public async Task<ActionResult> EditarAtivo([FromBody] Ativos ativo)
        {
            if (ativo == null) return BadRequest("Ativo está null");

            var request = await _dc.ativos.FirstOrDefaultAsync(i => i.IdAtivo == ativo.IdAtivo);
           
            if (request == null)
                return BadRequest("Não foi encontrado nenhum ativo com esse id");

            request.Nome = ativo.Nome;
            request.LocalAlocado = ativo.LocalAlocado;
            request.Nota = ativo.Nota;
            request.QuantidadeDeAtivo = ativo.QuantidadeDeAtivo;
            request.RecomendacaoPorcentagem = ativo.RecomendacaoPorcentagem;
            request.SugestaoInvestimento = ativo.SugestaoInvestimento;
            request.TipoAtivo = ativo.TipoAtivo;
            request.ValorAtualDoAtivo = ativo.ValorAtualDoAtivo;
            request.ValorTotalInvestido = ativo.ValorTotalInvestido;

            _dc.ativos.Update(request);
            await _dc.SaveChangesAsync();

            return Ok(request);
        }

        [HttpDelete("Deletar/{idAtivo}")]
        public async Task<ActionResult> ExcluirAtivo(int idAtivo)
        {
            if (idAtivo == null) return BadRequest("Ativo está null");

            var delet = await _dc.ativos.FirstOrDefaultAsync(i => i.IdAtivo == idAtivo);

            if (delet == null)
                return BadRequest("Não foi encontrado nenhum ativo com esse id");

            _dc.ativos.Remove(delet); ;
            await _dc.SaveChangesAsync();

            return Ok("Ativo deletado com sucesso!");
        }

    }
}
