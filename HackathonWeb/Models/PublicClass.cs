using FluentScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace HackathonWeb.Models
{

    public enum EParamedicStatus
    {
        Offline,
        Available,
        WaitToConfirm,
        Busy
    }

    public enum EParamedicType
    {
A,B
    }

    public enum ERequestType
    {
        AutoH,AutoL,Escalated,Manual
    }

    public enum ERequestStatus
    {
        Waiting, 
        Accepted,
        Terminated,
        Escalated,
        Cancelled,
        Rejected
    }

    public class MyPublicClass : Registry
    {
        public MyPublicClass()
        {
            Schedule<GenerateOneRequest>().ToRunNow().AndEvery(5).Minutes();
            Schedule<HandleWaitingRequests>().ToRunNow().AndEvery(10).Seconds();

        }
    }

    public class GenerateOneRequest : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;
        public GenerateOneRequest()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            try
            {
                lock (_lock)
                {

                    if (_shuttingDown)
                        return;

                    PublicClass classC = new Models.PublicClass();
                    classC.GenerateOneRequest();
                }
            }
            catch
            {

            }
            finally
            {
                // Always unregister the job when done.
                HostingEnvironment.UnregisterObject(this);
            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }
        }
    }

    public class HandleWaitingRequests : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;
        public HandleWaitingRequests()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            try
            {
                lock (_lock)
                {

                    if (_shuttingDown)
                        return;

                    PublicClass classC = new Models.PublicClass();
                    classC.HandleWaitingRequests();
                }
            }
            catch
            {

            }
            finally
            {
                // Always unregister the job when done.
                HostingEnvironment.UnregisterObject(this);
            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }
        }
    }

    public class PublicClass
    {
        
        public void GenerateInitData()
        {
            HajjHackathonDBEntities db = new HajjHackathonDBEntities();

            if (db.People.Count() > 0)
                return;

            int iii = 0;
            Random r = new Random();
            List<Person> per = new List<HackathonWeb.Person>();
            for (int i = 0; i < 10000; i++)
            {
                iii++;
                Person p1 = new Person()
                {
                    DiseaseA = r.Next(0, 10000) % 2 == 0,
                    DiseaseB = r.Next(0, 10000) % 2 == 0,
                    DiseaseC = r.Next(0, 10000) % 2 == 0,
                    Name = "Person " + iii,
                    PassportNo = "Person " + iii,
                };

                per.Add(p1);

                Person p2 = new Person()
                {
                    DiseaseA = false,
                    DiseaseB = false,
                    DiseaseC = false,
                    Name = "Person " + iii,
                    PassportNo = "Person " + iii,
                };
                per.Add(p2);
            }

            var randomGPS = GetRandomGPS("100");

            List<Paramedic> par = new List<HackathonWeb.Paramedic>();
            for (int i = 0; i < 100; i++)
            {
                iii++;
                Paramedic p1 = new Paramedic()
                {
                    LastGPS=randomGPS[i],
                    LastUpdate=DateTime.Now.AddYears(-1),
                    Status=EParamedicStatus.Available.ToString(),
                    Type=i%2==0?EParamedicType.A.ToString():EParamedicType.B.ToString(),
                };
                p1.Name = p1.Type.ToString() + i;

            par.Add(p1);
            }


            db.Paramedics.AddRange(par);
            db.SaveChanges();
            db.People.AddRange(per);
            db.SaveChanges();
        }

        public void GenerateOneRequest()
        {
            HajjHackathonDBEntities db = new HajjHackathonDBEntities();
            var randomGPS = GetRandomGPS("100");
            var per = db.People.Where(x => x.Requests.Count == 0).ToList();
            if (per.Count()==0)
                return;
            

            Random r = new Random();
            Request req = new HackathonWeb.Request()
            {
                Status=ERequestStatus.Waiting.ToString(),
                DateTime = DateTime.Now,
                GPS = randomGPS[r.Next(0,randomGPS.Count)],
                PersonID = per.ElementAt(r.Next(0,per.Count())).ID,
                VitalSignA=r.Next(40,130),
                VitalSignB=r.Next(20,50),
                
            };
            int rand = r.Next(0, 100);
            Paramedic caller = null;
            if (rand % 4 == 0)
                caller = db.Paramedics.Where(x => x.Status == EParamedicStatus.Available.ToString()
              && x.Type == EParamedicType.A.ToString()).FirstOrDefault();

            if (caller != null)
            {
                req.CallerID = caller.ID;
                req.Type = ERequestType.Escalated.ToString();
                req.Comment = GetRandomRequestComment(EParamedicType.B);
            }
            else
            {
                int tt = r.Next(0, 3);
                switch (tt)
                {
                    case 0:
                        req.Type = ERequestType.AutoL.ToString();
                        req.Comment = "توقع حالة غير حرجة";
                        break;

                    case 1:
                        req.Type = ERequestType.AutoH.ToString();
                        req.Comment = "توقع حالة حرجة";
                        break;

                    default:
                        req.Type = ERequestType.Manual.ToString();
                        req.Comment = "الحاج يطلب مساعدة طبية";
                        break;
                }
            }

            db.Requests.Add(req);
            db.SaveChanges();

        }

        public string GetRandomRequestComment(EParamedicType type)
        {
            Random r = new Random();
            if (type == EParamedicType.A)
            {
                if (r.Next(0, 5) % 2 == 0)
                    return "يعاني المريض من مرض السكري و تحسس رئوي مع ارتفاع ملحوظ لدرحة الحرارة";
                else
                    return " يعاني المريض من مرض السكري وتم اعطائه جرعة انسولين ";
            }
            else
            {
                if (r.Next(0, 5) % 2 == 0)
                    return "تم الكشف على المريض واشتباه بحالة عدوى خطيرة";
                else
                    return "يحتاج المريض الى دخول غرفة العمليات سريعا ";
            }
        }

        public List<string> GetRandomGPS(string num)
        {
            List<string> res = new List<string>();
            StreamReader sr = new StreamReader(@"E:\"+num+".txt");
            while (!sr.EndOfStream)
            {
                var tmp = sr.ReadLine().Split(',');
                res.Add(tmp[1] + " , " + tmp[3]);
            }
            return res;
        }

        public void HandleWaitingRequests()
        {
            HajjHackathonDBEntities db = new HackathonWeb.HajjHackathonDBEntities();

            var allwaiting = db.Requests.Where(x => x.Status != ERequestStatus.Terminated.ToString() && x.Status != ERequestStatus.Accepted.ToString()).ToList();
            foreach (var x in allwaiting)
            {
                if (x.RequestFollowUps.Where(y => y.Status == ERequestStatus.Waiting.ToString()).Count()>0)
                    continue;

                List<Paramedic> res = null;
                if (x.Type == ERequestType.AutoH.ToString() || x.Type == ERequestType.Escalated.ToString())
                {
                    res = db.Paramedics.Where(y => y.Status == EParamedicStatus.Available.ToString()
                    && y.Type == EParamedicType.B.ToString()).ToList();
                }
                else
                {
                    res = db.Paramedics.Where(y => y.Status == EParamedicStatus.Available.ToString()
                    && y.Type == EParamedicType.A.ToString()).ToList();
                }

                Paramedic parRes= GetNearestParamedic(x.GPS, res);
                if(parRes!=null)
                {
                    RequestFollowUp reqFU = new HackathonWeb.RequestFollowUp()
                    {
                        AssignedDateTime=DateTime.Now,
                        AssignedToID=parRes.ID,
                        RequestID=x.ID,
                        Status=ERequestStatus.Waiting.ToString(),
                    };
                    parRes.Status = EParamedicStatus.WaitToConfirm.ToString();
                    db.RequestFollowUps.Add(reqFU);
                    db.SaveChanges();
                }
            }
        }


        public Paramedic GetNearestParamedic(string GPS, List<Paramedic> avaPara) {
            float x = float.Parse( GPS.Split(',')[0]);
            float y = float.Parse( GPS.Split(',')[1]);
            var min = avaPara.First();
            var minVal = 1000000.0f;
            foreach(var PP in avaPara)
            {
                float x1 = float.Parse(PP.LastGPS.Split(',')[0]);
                float y2 = float.Parse(PP.LastGPS.Split(',')[1]);
                float VAL = Math.Abs(x - x1) + Math.Abs(y - y2);
                if(VAL<minVal)
                {
                    minVal = VAL;
                    min = PP;
                }
            }
            return min;
        }
    }
}