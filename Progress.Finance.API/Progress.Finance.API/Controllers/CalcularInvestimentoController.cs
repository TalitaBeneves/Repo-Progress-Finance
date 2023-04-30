using API.Data;
using API.Model;
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

            var totalPorcentagem = porcentagemAcoes + porcentagemFIIs + porcentagemRendaFixa;

            if (totalPorcentagem != 100)
                return BadRequest("A soma das porcentagens não é igual a 100%");

            var valorAcoes = Convert.ToDouble(porcentagemAcoes / totalPorcentagem) * valorInvestimento;
            var valorFIIs = Convert.ToDouble(porcentagemFIIs / totalPorcentagem) * valorInvestimento;
            var valorRendaFixa = Convert.ToDouble(porcentagemRendaFixa / totalPorcentagem) * valorInvestimento;

            decimal valorTotalDistribuido = 0;

            var newListAtivos = new List<Ativos>();
            foreach (var item in listAtivos)
            {
                var valorAtivo = Convert.ToDouble(item.ValorAtualDoAtivo);

                if (item.TipoAtivo == TipoAtivo.ACOES)
                {
                    valorAtivo = valorAcoes;
                }
                else if (item.TipoAtivo == TipoAtivo.FUNDOS_IMOBILIARIOS)
                {
                    valorAtivo = valorFIIs;
                }
                else if (item.TipoAtivo == TipoAtivo.RENDA_FIXA)
                {
                    valorAtivo = valorRendaFixa;
                }


                var valorPorNota = valorAtivo / item.Nota;
                var quantidade = Math.Floor(valorAtivo / item.ValorAtualDoAtivo);


                item.ValorTotalInvestido = Convert.ToInt32(valorPorNota);
                item.QuantidadeDeAtivo = Convert.ToInt32(quantidade);

                valorTotalDistribuido += Convert.ToDecimal(valorPorNota);

                newListAtivos.Add(item);
            }

            if (valorTotalDistribuido < valorInvestimento)
            {
                var valorRestante = valorInvestimento - valorTotalDistribuido;

                // add a diferença proporcionalmente aos ativos já alocados
                if (valorTotalDistribuido > 0)
                {
                    foreach (var item in newListAtivos)
                    {
                        var porcentagemDoValorTotal = (decimal)item.ValorTotalInvestido / (decimal)valorTotalDistribuido;
                        item.ValorTotalInvestido += Convert.ToInt32(valorRestante * porcentagemDoValorTotal);
                    }
                }
                else // não há nenhum ativo alocado, adiciona a diferença em ordem decrescente de recomendação
                {
                    var ativosOrdenadosPorRecomendacao = listAtivos.OrderByDescending(item => item.Nota);

                    foreach (var item in ativosOrdenadosPorRecomendacao)
                    {
                        if (valorRestante <= 0)
                            break;

                        if (valorRestante >= item.SugestaoInvestimento)
                        {
                            item.ValorTotalInvestido += Convert.ToInt32(item.SugestaoInvestimento);
                            valorRestante -= item.SugestaoInvestimento;
                        }
                        else
                        {
                            var quantidadeRestante = Convert.ToInt32(valorRestante / item.ValorAtualDoAtivo);
                            item.ValorTotalInvestido += quantidadeRestante * item.ValorAtualDoAtivo;
                            valorRestante = 0;
                        }
                    }
                }
            }

            await _dc.SaveChangesAsync();
            return Ok(newListAtivos);

        }

    }
}