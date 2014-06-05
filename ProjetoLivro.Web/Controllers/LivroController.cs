using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoLivro.BL;

namespace ProjetoLivro.Web.Controllers
{
    public class LivroController : Controller
    {
        //Nao façam isso!!!
        private LivroService livroService = new LivroService(new LivroContext());

        public ActionResult Index()
        {
            return View(livroService.GetLivrosAtivos());
        }

        // GET: /Livro/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Status")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                livroService.Salvar(livro);

                return RedirectToAction("Index");
            }

            return View(livro);
        }
    }
}
