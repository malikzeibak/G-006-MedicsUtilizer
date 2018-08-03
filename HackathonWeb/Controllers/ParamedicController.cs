using HackathonWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackathonWeb.Controllers
{
    public class ParamedicController : Controller
    {
        // GET: Paramedic
        public ActionResult Index(int? id)
        {
            HajjHackathonDBEntities db = new HackathonWeb.HajjHackathonDBEntities();
            Request REQ = null;
            if (id != null)
                REQ = db.Requests.Where(x => x.ID == id).FirstOrDefault();
            else
               REQ = db.RequestFollowUps.Where(x => x.Status == ERequestStatus.Waiting.ToString()).FirstOrDefault().Request;

            RequestFollowUp FU = db.RequestFollowUps.Where(cc => cc.RequestID == REQ.ID).FirstOrDefault();

            RequestDetailsViewModel model = new Models.RequestDetailsViewModel()
            {
                Comment = REQ.Comment,
                CreateDateTime=REQ.DateTime,
                DiseaseA=REQ.Person.DiseaseA,
                DiseaseB=REQ.Person.DiseaseB,
                DiseaseC=REQ.Person.DiseaseC,
                GPS= REQ.GPS,
                VitalSignA=REQ.VitalSignA,
                VitalSignB=REQ.VitalSignB,
                Type=REQ.Type,
                RequestStatus=REQ.Status,
                ParamedicName=REQ.Paramedic!=null?REQ.Paramedic.Name:"",
                RequestID=REQ.ID,
            };


            return View(model);
        }

        public ActionResult Accept(int id)
        {
            HajjHackathonDBEntities db = new HajjHackathonDBEntities();
           var req= db.Requests.Where(x => x.ID == id).FirstOrDefault();
            req.Status = ERequestStatus.Accepted.ToString();
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult Reject(int id)
        {
            HajjHackathonDBEntities db = new HajjHackathonDBEntities();
            var req = db.Requests.Where(x => x.ID == id).FirstOrDefault();
            req.Status = ERequestStatus.Rejected.ToString();
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult Evaluate(int id)
        {
            HajjHackathonDBEntities db = new HajjHackathonDBEntities();
            var req = db.Requests.Where(x => x.ID == id).FirstOrDefault();
            req.Status = ERequestStatus.Terminated.ToString();
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult Escalate(int id)
        {
            HajjHackathonDBEntities db = new HajjHackathonDBEntities();
            var req = db.Requests.Where(x => x.ID == id).FirstOrDefault();
            req.Status = ERequestStatus.Escalated.ToString();
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult Cancel(int id)
        {
            HajjHackathonDBEntities db = new HajjHackathonDBEntities();
            var req = db.Requests.Where(x => x.ID == id).FirstOrDefault();
            req.Status = ERequestStatus.Cancelled.ToString();
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }


    }
}