namespace ZPDigitalSignatureWeb.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ContractFiles
    {
        public Guid ID { get; set; }

        [StringLength(250)]
        public string FileName { get; set; }

        [StringLength(250)]
        public string FileType { get; set; }

        public string FileBase64 { get; set; }

        public string FileBase64_Signed { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
