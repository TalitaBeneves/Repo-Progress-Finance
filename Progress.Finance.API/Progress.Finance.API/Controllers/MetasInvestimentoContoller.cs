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

        [HttpGet]
        public async Task<ActionResult> ListarMetaInvestimento(MetaInvestimento meta)
        {
            var listMeta = await _dc.metaInvestimento.ToListAsync();

            if (meta == null) throw new InvalidOperationException("Metas não encontradas");

            return Ok(listMeta);
        }

        [HttpGet("{id}")]
        public ActionResult<MetaInvestimento> ListarMetaInvestimentoById(int id)
        {

            var listById = _dc.metaInvestimento.FirstOrDefault(pf => pf.IdUsuario == id);

            if (listById == null) throw new InvalidOperationException("Metas não encontradas");

            return Ok(listById);
        }


        [HttpPost]
        public async Task<ActionResult> CadastrarMetaInvestimento([FromBody] MetaInvestimento meta)
        {
            if (meta == null)
            {
                return BadRequest();
            }

            _dc.metaInvestimento.Add(meta);
            await _dc.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarMetaInvestimento), new { id = meta.IdMeta }, meta);
        }
    }

}
