using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommenderSystemCore.Objects;

namespace RecommenderSystemCore.Abstractions
{
    public interface IRater
    {
        double GetRating(List<UserAction> actions);
    }
}
