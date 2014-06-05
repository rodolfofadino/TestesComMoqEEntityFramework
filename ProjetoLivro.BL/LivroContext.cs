using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoLivro.BL
{
    public class LivroContext : DbContext
    {
        public virtual DbSet<Livro> Livros { get; set; }
    }
}
