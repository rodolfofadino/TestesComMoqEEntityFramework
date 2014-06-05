using System.Data.Entity;

namespace ProjetoLivro.BL
{
    public class LivroContext : DbContext
    {
        public virtual DbSet<Livro> Livros { get; set; }
    }
}
