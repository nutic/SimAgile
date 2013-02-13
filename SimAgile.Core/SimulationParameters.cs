//  
//  Copyright (c) 2005-2013 TargetProcess. All rights reserved.
//  TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System.ComponentModel.DataAnnotations;

namespace SimAgile.Core
{
	public class SimulationParameters
	{
	    public SimulationParameters()
	    {
	        InitialBacklogSize = 15;
            DateRange = 30;
	        MeanDevTime = 10.5;
	        VarDevTime = 0.1;
            TestTimeRate = 6;
            BacklogGrowthRate = 2;
	        DeveloperCount = 3;
	        QaCount = 2;
		    Quality = 1;
	    }

		[Display(Name = "Initial backlog size")]
		public int InitialBacklogSize { get; set; }

        [Display(Name = "Initial coded size")]
		public int InitialCodedSize { get; set; }

		[Display(Name = "Date range", Description = "Number of days passed from start of work")]
		public int DateRange { get; set; }

	    [Display(Name = "Backlog growth rate", Description = "How often new user stories added to backlog (2 means approx. 1 user story per 2 days, 0.5 means approx. 2 user stories per day)")]
		public double BacklogGrowthRate { get; set; }

		[Display(Name = "Developers #")]
		public int DeveloperCount { get; set; }

		[Display(Name = "Mean development time", Description = "How many days does it usually take to implement a story")]
		public double MeanDevTime { get; set; }

		[Display(Name = "Development time variation", Description = "The bigger value the more variation in development time. (0.1 ... 10)")]
		public double VarDevTime { get; set; }

		[Display(Name = "QA #")]
		public int QaCount { get; set; }

		[Display(Name = "Mean test time", Description = "How many days does it approx. take to test a story")]
		public double TestTimeRate { get; set; }

		[Display(Name = "Quality rate", Description = "Probability of story to be done after testing without reopen")]
		public double Quality { get; set; }
	}
}