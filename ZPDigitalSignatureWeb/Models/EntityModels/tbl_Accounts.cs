namespace ZPDigitalSignatureWeb.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Accounts
    {
        public Guid ID { get; set; }

        public Guid? CustomerID { get; set; }

        [StringLength(50)]
        public string SoldCode { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(500)]
        public string DisplayName { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(500)]
        public string Email { get; set; }

        public int? Role { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
