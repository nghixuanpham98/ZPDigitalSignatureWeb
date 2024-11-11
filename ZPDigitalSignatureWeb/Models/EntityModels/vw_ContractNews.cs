namespace ZPDigitalSignatureWeb.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_ContractNews
    {
        public Guid ID { get; set; }

        public Guid? CustomerID { get; set; }

        public Guid? FileContractID { get; set; }

        [StringLength(250)]
        public string ContractName { get; set; }

        public int? Status { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
