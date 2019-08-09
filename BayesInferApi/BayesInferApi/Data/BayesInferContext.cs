using BayesInferApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BayesInferApi.ViewModels;

namespace BayesInferApi.Data
{
    public class BayesInferContext : DbContext
    {
        public DbSet<ArquivoRedeBayesiana> ArquivosRedeBayesiana { get; set; }

        public BayesInferContext(DbContextOptions<BayesInferContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=BayesInfer.db");
        }

        //public DbSet<BayesInferApi.ViewModels.NodeBeliefViewModel> NodeBeliefViewModel { get; set; }


    }
}
