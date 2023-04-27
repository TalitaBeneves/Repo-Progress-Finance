using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Finance.API.Model;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Finance.API.Controllers
{
    [Controller]
    [Route("[Controller]")]
    public class UsuariosController : ControllerBase
    {
        private DataContext _dc;

        public UsuariosController(DataContext context)
        {
            _dc = context;
        }

        [HttpGet("buscarMetaInvestimentoPeloId/{idUsuario}")]
        public async Task<ActionResult> buscarMetaPeloId(int idUsuario)
        {

            if (idUsuario == null) return BadRequest($"IdUsuario não pode ser null");

            var usuario = await _dc.usuarioMetaInvestimento.FindAsync(idUsuario);
            return Ok(usuario);

        }

        [HttpPost("cadastrarUsuario")]
        public async Task<ActionResult> CadastrarUsuario([FromBody] Usuarios user)
        {
            try
            {
                _dc.usuarios.Add(user);
                await _dc.SaveChangesAsync();

                return Ok("Usuário criado com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao cadastrar usuário: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] Usuarios user)
        {
            try
            {

                var usuario = await _dc.usuarios.FirstOrDefaultAsync(u => u.Email == user.Email && u.Senha == user.Senha);
                if (usuario == null) throw new InvalidOperationException("Usuário não encontrado");

                var response = new Usuarios
                {
                    Email = user.Email,
                    IdUsuario = usuario.IdUsuario,
                    ImagemUrl = user.ImagemUrl,
                    Nome = user.Nome
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao fazer login: {ex.Message}");
            }
        }

        [HttpPut("EditarUsuario")]
        public async Task<ActionResult> EditarUsuario([FromBody] Usuarios user)
        {
            try
            {
                var usuario = await _dc.usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == user.IdUsuario);
                if (usuario == null) throw new InvalidOperationException("Id não encontrado");

                if (usuario.ImagemUrl != user.ImagemUrl)
                {
                    //deletarimg
                    //adicionar img

                }
                var editarUser = new Usuarios
                {
                    Email = user.Email,
                    IdUsuario = user.IdUsuario,
                    Nome = user.Nome,
                    Senha = user.Senha
                };

                _dc.usuarios.Update(editarUser);
                await _dc.SaveChangesAsync();

                return Ok(editarUser);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        [HttpDelete("DeletarUsuario/{id}")]
        public async Task<ActionResult> DeletarUsuario(int id)
        {
            try
            {
                var usuario = await _dc.usuarios.FindAsync(id);

                _dc.usuarios.Remove(usuario);
                await _dc.SaveChangesAsync();

                return Ok("Usuário deletado com sucesso");

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        [NonAction]
        public async Task<ActionResult> atualizarImagem([FromBody] Usuarios user)
        {
            return null;
        }
    }
}
