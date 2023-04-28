using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Finance.API.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Progress.Finance.API.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class CalcularInvestimentoController : ControllerBase
    {
        private readonly DataContext _dc;

        public CalcularInvestimentoController(DataContext context)
        {
            _dc = context;
        }

        [HttpGet]
        public async Task<ActionResult> CalcularInvestimento([FromQuery] int valorInvestimento, [FromQuery] int idUsuario)
        {
            var listAtivos = await _dc.ativos.Where(id => id.IdUsuario == idUsuario).ToListAsync();
            var metaInvestimento = await _dc.metaInvestimento.Where(id => id.IdUsuario == idUsuario).FirstOrDefaultAsync();

            if (listAtivos == null) return BadRequest("Ativos não encontrados");
            if (metaInvestimento == null) return BadRequest("Meta não encontrada");

            var porcentagemAcoes = metaInvestimento.Acoes;
            var porcentagemFIIs = metaInvestimento.Fiis;
            var porcentagemRendaFixa = metaInvestimento.RendaFixa;

            var valorTotalAcoes = valorInvestimento * porcentagemAcoes / 100;
            var valorTotalFIIs = valorInvestimento * porcentagemFIIs / 100;
            var valorTotalRendaFixa = valorInvestimento * porcentagemRendaFixa / 100;

            var valorTotalRecomendado = valorTotalAcoes + valorTotalFIIs + valorTotalRendaFixa;
            var totalPontos = listAtivos.Sum(item => item.Nota);
            decimal valorTotalDistribuido = 0;

            foreach (var item in listAtivos)
            {
                var porcentagem = item != null ? item.RecomendacaoPorcentagem : 0;
                decimal valorRecomendado;
                decimal valorRecomendadoAtivo;

                switch (item.TipoAtivo)
                {
                    case TipoAtivo.ACOES:
                        valorRecomendado = porcentagem / 100M * valorTotalAcoes;
                        break;
                    case TipoAtivo.FUNDOS_IMOBILIARIOS:
                        valorRecomendado = porcentagem / 100M * valorTotalFIIs;
                        break;
                    case TipoAtivo.RENDA_FIXA:
                        valorRecomendado = porcentagem / 100M * valorTotalRendaFixa;
                        break;
                    default:
                        valorRecomendado = 0;
                        break;
                }

                var quantidadeUnidadesRecomendadas = valorRecomendado / item.ValorDoAtivo;

                var quantidadeUnidadesArredondada = (int)Math.Round(quantidadeUnidadesRecomendadas, MidpointRounding.AwayFromZero);
                valorRecomendadoAtivo = quantidadeUnidadesArredondada * item.ValorDoAtivo;

                item.RecomendacaoPorcentagem = porcentagem;
                item.ValorRecomendado = valorRecomendadoAtivo;

                valorTotalDistribuido += valorRecomendadoAtivo;
            }

            if (valorTotalDistribuido > valorInvestimento)
            {
                return BadRequest("Valor total recomendado é maior que o valor investido pelo usuário");
            }

            await _dc.SaveChangesAsync();
            return Ok(listAtivos);
        }

    }
}
