﻿using System.ComponentModel.DataAnnotations;

namespace Progress.Finance.API.Model
{
    public class Usuarios
    {
        [Key]
        public int Id_Usuario { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public string ImagemUrl { get; set; }

    }
}