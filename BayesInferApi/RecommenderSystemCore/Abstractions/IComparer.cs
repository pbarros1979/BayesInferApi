﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderSystemCore.Comparers
{
    public interface IComparer
    {
        double CompareVectors(double[] userFeaturesOne, double[] userFeaturesTwo);
    }
}
