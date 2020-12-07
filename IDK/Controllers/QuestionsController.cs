using IDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Data.Entity;
using System.Collections.ObjectModel;
using Microsoft.AspNet.Identity;

namespace IDK.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        // GET
        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Index()
        {
            // https://docs.microsoft.com/en-us/ef/ef6/querying/related-data
            // https://www.tutorialspoint.com/entity_framework/entity_framework_eager_loading.htm
            // eager loading
            var questions = db.Questions.Include("Tags").Include("User"); //acelasi efect il are si var questions = db.Questions.ToList(); 
            ViewBag.Questions = questions; //trimitem toate intrebarile spre view (aici ar trebui sa trimitem de ex un top 50 sau ceva, sau cele mai noi x intrebari)
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }

        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Show(int id)
        {
            //cautam intrebarea dupa id
            Question question = db.Questions.Find(id);
            return View(question);
        }
        
        [HttpPost]
        [Authorize(Roles = "User, Editor, Admin")]
        public ActionResult Show(Answer ans)
        {
            ans.Date = DateTime.Now; //data va fi cea din back end
            try
            {
                if (ModelState.IsValid)
                {
                    //incercam sa adaugam answer ul primit de la user
                    db.Answers.Add(ans);
                    db.SaveChanges();
                    return Redirect("/Questions/Show/" + ans.QuestionId); //redirect catre show ul intrebarii la care s-a adaugat un answer nou
                }
                else
                {
                    Question q = db.Questions.Find(ans.QuestionId);
                    return View(q);
                }
            }
            catch (Exception e)
            {
                Question q = db.Questions.Find(ans.QuestionId);
                return View(q);
            }
        }

        // GET
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult New()
        {
            //cream un nou obiect de tip question si ii punem in proprietatea Tg toate
            //tag urile din care user ul poate alege
            Question question = new Question();
            question.Tg = getAllTags();

            //Preluam id-ul utilizatorului curent
            question.UserId = User.Identity.GetUserId();
            return View(question);
        }

        [HttpPost]
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult New(Question question)
        {
            question.Tags = new Collection<Tag>();
            question.Date = DateTime.Now; //ii setam data ca fiind cea din back, la care s-a trimis question-ul
            question.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var selectedTagId in question.SelectedTags)
                    {
                        //pentru fiecare tag selectat il cautam il baza de date si il adaugam la proprietatea
                        //Tags a question ului
                        Tag dbTag = db.Tags.Find(selectedTagId);
                        question.Tags.Add(dbTag);
                    }

                    //in final adaugam question ul
                    db.Questions.Add(question);
                    db.SaveChanges();
                    TempData["message"] = "Your Question was posted";
                    return RedirectToAction("Index");
                }
                else
                {
                    question.Tg = getAllTags();
                    return View(question);
                    //se reincarca formul si i se paseaza obiectul deja "alterat"
                    //pentru ca utilizatorul sa nu fie nevoit sa recompleteze la fiecare greseala
                }
            }
            catch (Exception e)
            {
                question.Tg = getAllTags();
                return View(question);
            }
            
        }

        // GET
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Edit(int id)
        {
            Question question = db.Questions.Find(id); //cautam obiectul dupa id
            question.Tg = getAllTags(); //ii punem in Tg toate optiunile de taguri

            //deoarece "SelectedTags" este un array de intregi(array - ul in care stocam id - urile tagurilor)
            //nu putem face Add direct in acest array deoarece trebuie sa stim de la inceput dimensiunea lui. Asadar, exista doua variante: 
            //1. Aflam dimensiunea cu ajutorul metodei count, apoi parcurgem si pentru fiecare pozitie din array adaugam un nou id de tag. 
            //2. Metoda de mai jos - cream o lista noua, adaugam pe rand taguri dupa id, in final aplicam metoda ToArray pentru a converti lista in array 
            //si pentru a pasa toata lista array-ului initial "SelectedTags"

            List<int> currentSelection = new List<int>(); //aici retinem selectia actuala, inainte de modificare (id urile tagurilor)

            foreach (var tag in question.Tags)
            {
                currentSelection.Add(tag.Id); //adaugam tagurile selectate inainte de editare in currentSelection
            }
            question.SelectedTags = currentSelection.ToArray();//punem currentSelection in SelectedTags ca utilizatorul sa vada ce taguri selectate are

            return View(question);
        }

        [HttpPut]
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Edit(int id, Question questionEdit)
        {
            questionEdit.Tg = getAllTags();
            try
            {
                if (ModelState.IsValid)
                {
                    Question question = db.Questions.Find(id); //cautam intrebarea dupa id
                    if (TryUpdateModel(question))
                    {
                        foreach (Tag currentTag in question.Tags.ToList())
                        {
                            //inainte sa adaugam noile taguri trebuie sa le stergem pe cele vechi
                            question.Tags.Remove(currentTag);
                        }

                        foreach (var selectedTagId in questionEdit.SelectedTags)
                        {
                            Tag dbTag = db.Tags.Find(selectedTagId);
                            question.Tags.Add(dbTag);
                            //adaugam noile taguri
                        }

                        //updatam si titlul si continutul
                        question.Title = questionEdit.Title;
                        question.Content = questionEdit.Content;
                        db.SaveChanges();

                        TempData["message"] = "Your Question has been edited";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(questionEdit); 
                }
            }
            catch
            {
                return View(questionEdit);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor, Admin")]
        public ActionResult Delete(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            TempData["message"] = "Your Question was deleted";
            return RedirectToAction("Index");
        }
        
        // HELPER METHODS:

        [NonAction]
        public IEnumerable<SelectListItem> getAllTags()
        {
            //metoda returneaza toate tag urile din care un utilizator poate alege

            var selectList = new List<SelectListItem>();
            var tags = from tag in db.Tags select tag;
            foreach(var tag in tags)
            {
                var listItem = new SelectListItem();
                listItem.Value = tag.Id.ToString();
                listItem.Text = tag.TagName.ToString();
                selectList.Add(listItem);
            }
            return selectList;
        }
    }
}