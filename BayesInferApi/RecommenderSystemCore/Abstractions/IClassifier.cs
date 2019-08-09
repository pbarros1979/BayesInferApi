﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommenderSystemCore.Objects;
using RecommenderSystemCore.Parsers;

namespace RecommenderSystemCore.Recommenders
{
    public interface IRecommender
    {
        void Train(UserBehaviorDatabase db);
        
        List<Suggestion> GetSuggestions(int userId, int numSuggestions);

        double GetRating(int userId, int articleId);
        
        void Save(string file);

        void Load(string file);
    }
}
