﻿using NavalVessels.Models;
using NavalVessels.Models.Contracts;
using NavalVessels.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavalVessels.Repositories
{
    public class VesselRepository : IRepository<IVessel>
    {
        private List<IVessel> models;

        public VesselRepository()
        {
            models = new List<IVessel>();
        }
        public IReadOnlyCollection<IVessel> Models => models.AsReadOnly();

        public void Add(IVessel model)
        {
            models.Add(model);
        }

        public bool Remove(IVessel model)
        {
            return models.Remove(model);
        }

        public IVessel FindByName(string name)
        {
            return models.FirstOrDefault(v => v.Name == name);
        }

    }
}
