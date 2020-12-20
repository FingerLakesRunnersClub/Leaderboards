﻿using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class AthleteViewModel : ViewModel
    {
        public override string Title => Athlete.Name;

        public Athlete Athlete { get; set; }
        public IDictionary<Course, IEnumerable<Result>> Results { get; set; }
    }
}