namespace ZPDigitalSignatureWeb.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_Contracts
    {
        public Guid ID { get; set; }

        public Guid? CustomerID { get; set; }

        [StringLength(50)]
        public string SAPCode { get; set; }

        [StringLength(50)]
        public string TaxCode { get; set; }

        [StringLength(250)]
        public string FullName { get; set; }

        public Guid? FileContractID { get; set; }

        [StringLength(250)]
        public string FileName { get; set; }

        [StringLength(250)]
        public string FileType { get; set; }

        public Guid? ContractTypeID { get; set; }

        [StringLength(250)]
        public string ContractTypeName { get; set; }

        [StringLength(250)]
        public string ContractName { get; set; }

        public Guid? FileImageID { get; set; }

        public string Note { get; set; }

        public int? Status { get; set; }

        public string RejectNote { get; set; }

        public double? PosX { get; set; }

        public double? PosY { get; set; }

        public double? WidthPlace { get; set; }

        public double? HeightPlace { get; set; }

        public int? PageSign { get; set; }

        [StringLength(250)]
        public string Reason { get; set; }

        [StringLength(250)]
        public string Contact { get; set; }

        [StringLength(250)]
        public string Location { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
