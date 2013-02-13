//  
//  Copyright (c) 2005-2013 TargetProcess. All rights reserved.
//  TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimAgile.Core;

namespace SimAgile.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new SimulationParameters());
        }

		[HttpPost]
		public ActionResult Index(SimulationParameters model)
		{
		    var simEnginePath = Path.Combine(Server.MapPath(@"~\"), @"..\SimAgile.Py\");
		    TempData["simres"] = new SimulationEngine(simEnginePath, PyConfig.Default).Run(model);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult CumulativeFlow()
        {
            var data = TempData["simres"];
            if (data == null)
            {
                return Content("<empty>");
            }
            return PartialView((object)new JavaScriptSerializer().Serialize(data));
        }

        [ChildActionOnly]
        public ActionResult CycleTimeDistr()
        {
            var data = TempData["simres"];
            if (data == null)
            {
                return Content("no data");
            }
            return PartialView((object)new JavaScriptSerializer().Serialize(data));
        }
    }
}
