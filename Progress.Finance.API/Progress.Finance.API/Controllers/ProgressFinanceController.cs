using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Progress.Finance.API.Model;

namespace Progress.Finance.API.Controllers //RESPONSAVEL PELAS ROTAS!
{
    [Controller]
    [Route("controller")]

    public class MetasController : ControllerBase
    {
        private DataContext _dc;

        public MetasController(DataContext context)
        {
            _dc = context;
        }


        [HttpPost("Metas")]
        public async Task<ActionResult> cadastrar([FromBody] Metas p)
        {

            _dc.progress.Add(p);
            await _dc.SaveChangesAsync();


            return Created("Criado", p);
        }

        [HttpGet("Metas")]
        public async Task<ActionResult> listar()
        {
            var list = await _dc.progress.Include(pf => pf.Items).ToListAsync();

            if (list == null)
            {
                return NotFound();
            }


            //AQUI ESTOU SOMANDO O VALOR INICIAL COM OS VALORES DEPOSITADOS 😁
            foreach (var meta in list)
            {
                var total = meta.ValorInicial;
                foreach (var item in meta.Items)
                {
                    total += item.ValorDepositado;
                }
                meta.ValorTotal = total;
                var formula = (total * 1.0M / meta.ValorMeta) * 100M;

                meta.Porcentagem = decimal.Parse(formula.ToString("N2")); ;
            }

            foreach (var items in list)
            {
                if (items.Porcentagem >= 100)
                {
                    items.Status = Status.CONCLUIDA;
                }
                else
                {
                    items.Status = Status.ANDAMENTO;

                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented,
              new JsonSerializerSettings
              {
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                  ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
              });

            return Ok(json);
        }


        [HttpGet("Metas/{id}")]
        public ActionResult<Metas> GetById(int id)
        {
            var listById = _dc.progress.Include(pf => pf.Items).FirstOrDefault(pf => pf.Id == id);

            if (listById == null)
            {
                return NotFound();
            }

            var total = listById.ValorInicial;
            foreach (var item in listById.Items)
            {
                total += item.ValorDepositado;
            }

            listById.ValorTotal = total;
            var formula = (total * 1.0M / listById.ValorMeta) * 100M;

            listById.Porcentagem = decimal.Parse(formula.ToString("N2"));

            var json = JsonConvert.SerializeObject(listById, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return Ok(json);
        }


        [HttpPut("Metas")]
        public async Task<ActionResult> editar([FromBody] Metas p)
        {
            var json = JsonConvert.SerializeObject(p, Formatting.Indented,
              new JsonSerializerSettings
              {
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                  ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
              });

            if (p.Porcentagem == 100)
            {
                p.Status = Status.CONCLUIDA;

                _dc.progress.Update(p);
                await _dc.SaveChangesAsync();

                return Ok(json);
            }
            else
            {
                p.Status = Status.ANDAMENTO;
            }

            if (p.Items != null)
            {
                foreach (var item in p.Items)
                {
                    var itemAtual = p.Items.FirstOrDefault(i => i.Id == item.Id);

                    if (p.Id != itemAtual.IdMeta)
                    {
                        throw new InvalidOperationException("O IdMeta precisa se igual ao Id da Meta");

                        return BadRequest();
                    }

                    if (itemAtual == null)
                    {
                        p.Items.Add(item);
                    }
                    else
                    {
                        itemAtual.ValorDepositado = item.ValorDepositado;
                        itemAtual.DataDeposito = item.DataDeposito;
                        itemAtual.IdMeta = item.IdMeta;
                    }
                }
            }


            _dc.progress.Update(p);
            await _dc.SaveChangesAsync();




            return Ok(json);
        }


        [HttpDelete("Metas/{id}")]
        public async Task<ActionResult> delete(int id)
        {
            Metas progress = _dc.progress.Include(pf => pf.Items).FirstOrDefault(pf => pf.Id == id);
            if (progress == null)
            {
                return NotFound();
            }
            else
            {
                _dc.progress.Remove(progress);
                await _dc.SaveChangesAsync();

                return Ok();
            }

        }
    }

    public class ItemsController : ControllerBase
    {
        private DataContext _dc;

        public ItemsController(DataContext context)
        {
            _dc = context;
        }


        [HttpGet("Metas/Items")]
        public async Task<ActionResult> getItems()
        {

            var list = await _dc.items.ToListAsync();
            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }


        [HttpPost("Metas/Items")]
        public async Task<ActionResult> cadastrarItems([FromBody] Items i)
        {

            if (i == null)
            {
                return NotFound();
            }
            else
            {
                _dc.items.Add(i);
                await _dc.SaveChangesAsync();

                return Created("Item Criado com sucesso!", i);
            }

        }

        [HttpPut("Metas/Items")]
        public async Task<ActionResult> editarItems([FromBody] Items i)
        {

            _dc.items.Update(i);
            await _dc.SaveChangesAsync();

            return Ok(i);
        }

        [HttpDelete("Metas/Items/{id}")]
        public async Task<ActionResult> deleteItems(int id)
        {
            var getById = _dc.items.Find(id);
            Items i = getById;
            if (i == null)
            {
                return NotFound();
            }
            else
            {
                _dc.items.Remove(i);
                await _dc.SaveChangesAsync();

                return Ok();
            }
        }
    }
}