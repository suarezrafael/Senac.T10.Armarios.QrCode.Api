﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Senac.T10.Armarios.QrCode.Api.Data;
using Senac.T10.Armarios.QrCode.Api.Dtos;
using Senac.T10.Armarios.QrCode.Api.Helper;
using Senac.T10.Armarios.QrCode.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Senac.T10.Armarios.QrCode.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmail _email;

        public UsuariosController(AppDbContext context, IEmail email)
        {
            _context = context;
            _email = email;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'AppDbContext.Usuarios'  is null.");
            }
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Usuarios/Login
        [HttpPost("Login")]
        public async Task<ActionResult<UsuarioResponse>> Login(UsuarioRequest usuario)
        {
            // consultar usuario na base 
            //  SELECT TOP(1) * FROM usuarios
            //  WHERE usuario.Username LIKE 'rafael@hotmail.com'
            //  AND usuario.Senha = '123'
            //  AND usuario.ativo = 1
            // buscar o usuario pelo username, senha e ativo = true
            var usr = await _context.Usuarios.Where(w =>
                w.NomeUsuario.Equals(usuario.Usuario) &&
                w.Senha.Equals(usuario.Senha))
                .FirstOrDefaultAsync();
            // verifica se usuario do banco está nulo
            if (usr == null)
            {
                // retorno HTTP 404 NotFound
                return NotFound(usuario.Usuario);
            }

            // Crie um token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("3e8acfc238f45a314fd4b2bde272678ad30bd1774743a11dbc5c53ac71ca494b");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, usr.Nome),
                new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString())
                    // Adicione outras claims conforme necessário
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Tempo de expiração do token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // retorna HTTP 200 OK { "Nome": "Rafael", "username": "rafael"}
            return Ok(new UsuarioResponse() { Id = usr.Id, Nome = usr.Nome, Usuario = usr.NomeUsuario, Token = tokenString });
        }

        // POST: api/Usuarios/RedefinirSenha
        [HttpPost("RedefinirSenha")]
        public async Task<ActionResult<UsuarioResponse>> RedefinirSenha(RedefinirSenhaRequest usuario)
        {
            // consultar usuario na base 
            //  SELECT TOP(1) * FROM usuarios
            //  WHERE usuario.Username LIKE 'rafael@hotmail.com'
            //  AND usuario.Email = '123'
            // buscar o usuario pelo username, senha e ativo = true
            var usr = await _context.Usuarios.Where(w =>
                w.NomeUsuario.Equals(usuario.Usuario) &&
                w.Email.Equals(usuario.Email))
                .FirstOrDefaultAsync();
            // verifica se usuario do banco está nulo
            if (usr == null)
            {
                // retorno HTTP 404 NotFound
                return NotFound(usuario.Usuario);
            }
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            string mensagem = $"{usr.Nome}. Sua nova senha é: {novaSenha}";

            bool emailEnviado = _email.Enviar(usr.Email, "Sistema Armarios QrCode/Nova senha", mensagem);
            if (emailEnviado)
            {
                // gravar a nova senha no banco de dados
                usr.Senha = novaSenha;
                _context.Entry(usr).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usr.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // retorna HTTP 200 OK { "Usuario": "Rafael", "Mensagem": "rafael"}
                return Ok(new RedefinirSenhaResponse() { Usuario = usr.NomeUsuario, Mensagem = "Senha enviado para seu email." });
            }
            else
            {
                return BadRequest("Erro ao enviar email");
            }

        }
        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
