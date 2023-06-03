﻿using API.Data;
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

        [HttpGet] //500
        public async Task<ActionResult> CalcularInvestimento([FromQuery] int valorInvestimento , [FromQuery] int idUsuario)
        {
            var listAtivos = await _dc.ativos.Where(id => id.IdUsuario == idUsuario).ToListAsync();
            var metaInvestimento = await _dc.metaInvestimento.Where(id => id.IdUsuario == idUsuario).FirstOrDefaultAsync();

            if (listAtivos == null) return BadRequest("Ativos não encontrados");
            if (metaInvestimento == null) return BadRequest("Meta não encontrada");

            var porcentagemAcoes = metaInvestimento.Acoes; //20%
            var porcentagemFIIs = metaInvestimento.Fiis; //20%
            var porcentagemRendaFixa = metaInvestimento.RendaFixa; //60%

            var valorTotalAcoes = valorInvestimento * porcentagemAcoes / 100; //100
            var valorTotalFIIs = valorInvestimento * porcentagemFIIs / 100; //100
            var valorTotalRendaFixa = valorInvestimento * porcentagemRendaFixa / 100;//00

            var valorTotalRecomendado = valorTotalAcoes + valorTotalFIIs + valorTotalRendaFixa;
            var totalPontos = listAtivos.Sum(item => item.Nota);
            decimal valorTotalDistribuido = 0;

            foreach (var item in listAtivos)
            {
                var porcentagem = (item.Nota / (decimal)totalPontos) * 100M;
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

                var valorPorNota = valorRecomendado * item.Nota;

                //var quantidadeUnidadesRecomendadas = valorPorNota / item.ValorAtualDoAtivo;
                item.RecomendacaoPorcentagem = valorRecomendado;
                item.SugestaoInvestimento = valorPorNota;

                valorTotalDistribuido += valorPorNota;

            }

            var valorFaltante = valorInvestimento - valorTotalDistribuido;

            // Ordena os ativos 
            listAtivos = listAtivos.OrderByDescending(item => item.Nota).ToList();

            // Calcula a soma das sugestões de investimento
            var somaSugestoes = listAtivos.Sum(item => item.SugestaoInvestimento);

            // Distribui o valor faltante proporcionalmente pela sugestões de investimento
            foreach (var item in listAtivos)
            {
                var porcentagemSugestao = item.SugestaoInvestimento / somaSugestoes;
                var valorParaAdicionar = valorFaltante * porcentagemSugestao;
                item.SugestaoInvestimento += valorParaAdicionar;
            }

            // Recalcula o valor total distribuído
            valorTotalDistribuido = listAtivos.Sum(item => item.SugestaoInvestimento);

            return Ok(listAtivos);
        }
    }

}
