using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoLivro.BL
{
    public class LivroService
    {
        private LivroContext _context { get; set; }
        public LivroService(LivroContext context)
        {
            _context = context;
        }

        public List<Livro> GetLivrosAtivos()
        {
            return _context.Livros.Where(a => a.Status== true).ToList();
        }

        public Task<List<Livro>> GetLivrosAtivosAsync()
        {
            return _context.Livros.Where(a => a.Status == true).ToListAsync();
        }

        public void Salvar(Livro livro)
        {
            _context.Livros.Add(livro);
            _context.SaveChanges();
        }
    }
}
