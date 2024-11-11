using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ZPDigitalSignatureWeb.Models.EntityModels
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }

        #region -- Table --

        public virtual DbSet<tbl_Accounts> tbl_Accounts { get; set; }
        public virtual DbSet<tbl_ContractFiles> tbl_ContractFiles { get; set; }
        public virtual DbSet<tbl_Contracts> tbl_Contracts { get; set; }
        public virtual DbSet<tbl_ContractTypes> tbl_ContractTypes { get; set; }
        public virtual DbSet<tbl_Customers> tbl_Customers { get; set; }

        #endregion

        #region -- View --

        public virtual DbSet<vw_Accounts> vw_Accounts { get; set; }
        public virtual DbSet<vw_ContractNews> vw_ContractNews { get; set; }
        public virtual DbSet<vw_ContractFiles> vw_ContractFiles { get; set; }
        public virtual DbSet<vw_Contracts> vw_Contracts { get; set; }
        public virtual DbSet<vw_ContractTypes> vw_ContractTypes { get; set; }
        public virtual DbSet<vw_Customers> vw_Customers { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_Customers>()
                .Property(e => e.CustomerBillCode)
                .IsFixedLength();
        }
    }
}
