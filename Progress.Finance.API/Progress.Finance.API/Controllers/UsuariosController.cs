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

        [HttpPost("Logar")]
        public async Task<ActionResult> Logar([FromBody] Usuarios user)
        {
            try
            {
                var usuario = await _dc.usuarios.FirstOrDefaultAsync(u => u.Email == user.Email && u.Senha == user.Senha);


                _dc.usuarios.Find(user);

                return Ok("Usuário logado com sucesso");

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao fazer login: {ex.Message}");
            }
        }
    }
}
