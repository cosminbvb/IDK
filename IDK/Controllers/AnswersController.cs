using IDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IDK.Controllers
{
    public class AnswersController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        // GET: Answers
        public ActionResult Index() //ar trebui stearsa ?
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Answer ans = db.Answers.Find(id); 
            db.Answers.Remove(ans);
            db.SaveChanges();
            //ii dam redirect spre aceeasi pagina
            return Redirect("/Questions/Show/" + ans.QuestionId); //redirect catre show ul intrebarii la care s-a sters answer ul
        }

        // GET
        public ActionResult Edit(int id)
        {
            //aici nu sunt sigur ca e bine pentru ca probabil ar trebui facut ceva in front end decat inca un request idk
            //deci cand se cere editarea unui answer primim id ul lui
            //il in db si il trimitem prin ViewBag
            //In plus, vrem sa trimitem si intrebarea cu restul answer urilor
            //ca utilizatorul sa vada contextul la editare
            Answer ans = db.Answers.Find(id); 
            Question q = db.Questions.Find(ans.QuestionId);
            ViewBag.answerToEdit = ans;
            return View(q);
        }

        [HttpPut]
        public ActionResult Edit(int id, Answer answerEdit)
        {
            try
            {
                Answer ans = db.Answers.Find(id); //cautam answer ul dupa id
                if (TryValidateModel(ans))
                {
                    ans.Content = answerEdit.Content; //ii updatam continutul cu cel din obiectul primit de la user
                    db.SaveChanges();
                }
                return Redirect("/Questions/Show/" + answerEdit.QuestionId); //redirect catre show ul intrebarii la care s-a modificat answer ul
            }
            catch (Exception e)
            {
                return Redirect("/Questions/Show/" + answerEdit.QuestionId); //aici nu stiu daca e bine
            }
        }
    }
}