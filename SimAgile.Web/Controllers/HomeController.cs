//  
//  Copyright (c) 2005-2013 TargetProcess. All rights reserved.
//  TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimAgile.Core;
using SimAgile.Web.Models;
using System.Linq;

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

		[ChildActionOnly]
	    public ActionResult Metrics()
		{
			var data = TempData["simres"];
			if (data == null)
			{
				return Content("no data");
			}
			return PartialView(GetMetrics());
		}

	    private IEnumerable<MetricsModel> GetMetrics()
	    {
		    var serializer = new JavaScriptSerializer();
		    var str = serializer.Serialize(TempData["simres"]);
		    var data = serializer.Deserialize<SimulationData>(str);
			yield return new MetricsModel { Name = "Avg. WIP", Value = data.History["WIP"].Average(), Unit = "units" };
			foreach (var state in data.States.Skip(1).Reverse().Skip(1))
		    {
				yield return new MetricsModel { Name = "Avg. " + state, Value = data.History[state].Average(), Unit = "units" };
		    }
			yield return new MetricsModel { Name = "Throughput", Value = data.History[data.States.Last()].Last()/data.History[data.States.Last()].Length, Unit = "units/d" };
			yield return new MetricsModel { Name = "Avg. cycle time", Value = data.Done.Average(x => x.CycleTime.Value), Unit = "d" };
	    }
    }

	public class SimulationData
	{
		public string[] States { get; set; }
		public Dictionary<string, double[]> History { get; set; }
		public Unit[] Work { get; set; }
		public Unit[] Done { get; set; }

		public class Unit
		{
			public double? LeadTime { get; set; }
			public double? CycleTime { get; set; }
		}
	}
}
