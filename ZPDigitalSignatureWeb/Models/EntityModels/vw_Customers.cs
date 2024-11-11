namespace ZPDigitalSignatureWeb.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_Customers
    {
        public Guid ID { get; set; }

        [StringLength(50)]
        public string SAPCode { get; set; }

        [StringLength(50)]
        public string TaxCode { get; set; }

        [StringLength(250)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Name2 { get; set; }

        [StringLength(100)]
        public string Name3 { get; set; }

        [StringLength(100)]
        public string Name4 { get; set; }

        public Guid? LocalGroup1 { get; set; }

        public Guid? LocalGroup2 { get; set; }

        public Guid? LocalGroup3 { get; set; }

        [StringLength(50)]
        public string SAGroup { get; set; }

        public Guid? PrimaryContact { get; set; }

        public Guid? StoreBinder { get; set; }

        public Guid? ChannelID { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        public Guid? AreaID { get; set; }

        public Guid? ProvinceID { get; set; }

        public Guid? DistrictID { get; set; }

        public double? Lng { get; set; }

        public double? Lat { get; set; }

        [StringLength(50)]
        public string Level { get; set; }

        public string TermsofPayment { get; set; }

        public double? CreditLimit { get; set; }

        [StringLength(50)]
        public string SAGroup1 { get; set; }

        [StringLength(50)]
        public string PriceGroup { get; set; }

        [StringLength(50)]
        public string CusGroup3 { get; set; }

        [StringLength(50)]
        public string CusGroup4 { get; set; }

        [StringLength(50)]
        public string CusGroup5 { get; set; }

        [StringLength(10)]
        public string CustomerBillCode { get; set; }

        [StringLength(1)]
        public string oldcode { get; set; }

        public DateTime? createdon { get; set; }

        [StringLength(1)]
        public string Centralorderblock { get; set; }
    }
}
