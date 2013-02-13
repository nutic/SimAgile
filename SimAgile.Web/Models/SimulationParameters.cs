//  
//  Copyright (c) 2005-2013 TargetProcess. All rights reserved.
//  TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System.ComponentModel;

namespace SimAgile.Web.Models
{
	public class SimulationParameters
	{
		[DisplayName("Initial backlog size")]
		public int InitialBacklogSize { get; set; }
	}
}