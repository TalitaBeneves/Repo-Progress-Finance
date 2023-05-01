using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Finance.API.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Finance.API.Controllers
{
    [Controller]
    [Route("[Controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _dc;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UsuariosController(DataContext context, IWebHostEnvironment hostEnvironment)
        {
            _dc = context;
            _hostEnvironment = hostEnvironment;
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
                    Email = usuario.Email,
                    IdUsuario = usuario.IdUsuario,
                    ImagemUrl = usuario.ImagemUrl,
                    Nome = usuario.Nome
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao fazer login: {ex.Message}");
            }
        }

        [HttpPut("EditarUsuario")]
        public async Task<ActionResult> EditarUsuario([FromBody] AtualizaUsuairo user)
        {
            try
            {
                var usuario = await _dc.usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == user.IdUsuario);
                if (usuario == null) throw new InvalidOperationException("Id não encontrado");

                if (!string.IsNullOrEmpty(user.NovaSenha))
                {
                    if (user.SenhaAtual != usuario.Senha)
                    {
                        return BadRequest("Senhas não conferem!");
                    }
                    usuario.Senha = user.NovaSenha;
                }

                usuario.Email = user.Email;
                usuario.Nome = user.Nome;

                _dc.usuarios.Update(usuario);
                await _dc.SaveChangesAsync();

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        [HttpPost("upload-image/{idUsuario}")]
        public async Task<ActionResult> AtualizarImagem(int idUsuario)
        {
            try
            {

                var usuario = await _dc.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
                if (usuario == null) return BadRequest("Usuário não encontrado");

                var file = Request.Form.Files[0];
                if (usuario.ImagemUrl != null || usuario.ImagemUrl == null)
                {
                    DeleteImage(usuario.ImagemUrl);
                    usuario.ImagemUrl = await SaveImage(file);
                }

                var res = _dc.usuarios.Update(usuario);
                await _dc.SaveChangesAsync();

                return Ok(usuario);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar imagem: {ex.Message}");
            }
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName)
                                              .Take(10)
                                              .ToArray())
                                              .Replace(' ', '-');
            imageName = $"{imageName}{DateTime.UtcNow.ToString("yymmss")}{Path.GetExtension(imageFile.FileName)}";

            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, @"Resources/images", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;

        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            if (imageName != null)
            {
                var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, @$"Resources/Images", imageName);
                if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
            }

        }

        [HttpDelete("DeletarUsuario/{id}")]
        public async Task<ActionResult> DeletarUsuario(int id)
        {
            try
            {
                var usuario = await _dc.usuarios.FindAsync(id);

                DeleteImage(usuario.ImagemUrl);

                _dc.usuarios.Remove(usuario);
                await _dc.SaveChangesAsync();

                return Ok("Usuário deletado com sucesso");

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

    }
}
