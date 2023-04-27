using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Finance.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Progress.Finance.API.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class MetasInvestimentoController : ControllerBase
    {
        private readonly DataContext _dc;

        public MetasInvestimentoController(DataContext context)
        {
            _dc = context;
        }

        [HttpGet("{idUsuario}")]
        public async Task<ActionResult> ListarMetaInvestimento(int idUsuario)
        {
            var listMeta = await _dc.metaInvestimento.Where(id => id.IdUsuario == idUsuario).ToListAsync();

            if (listMeta == null) return BadRequest("Meta não encontrada");

            return Ok(listMeta);
        }

        [HttpPost("CadastrarOuAtualizarMetaInvestimento")]
        public async Task<ActionResult> CadastrarOuAtualizarMetaInvestimento([FromBody] MetaInvestimento meta)
        {
            if (meta == null) return BadRequest("meta null");

            var verificaMeta = await _dc.metaInvestimento.Where(id => id.IdUsuario == meta.IdUsuario).FirstOrDefaultAsync();

            if (verificaMeta == null)
            {
                _dc.metaInvestimento.Add(meta);
                await _dc.SaveChangesAsync();

                var userMeta = new UsuarioMetaInvestimento { IdUsuario = meta.IdUsuario, IdMeta = meta.IdMeta };

                _dc.usuarioMetaInvestimento.Add(userMeta);
                await _dc.SaveChangesAsync();


                return CreatedAtAction(nameof(ListarMetaInvestimento), new { id = meta.IdMeta }, meta);
            }
            else
            {
                verificaMeta.Acoes = meta.Acoes;
                verificaMeta.RendaFixa = meta.RendaFixa;
                verificaMeta.Nome = meta.Nome;


                _dc.metaInvestimento.Update(verificaMeta);
                await _dc.SaveChangesAsync();

                return Ok(meta);
            }
        }
    }
}
