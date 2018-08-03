using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackathonWeb.Controllers
{
    public class SuperController : Controller
    {
        // GET: Super
        public ActionResult Index()
        {
            HajjHackathonDBEntities db = new HackathonWeb.HajjHackathonDBEntities();
            return View(db.Paramedics.ToArray());
        }
    }
}