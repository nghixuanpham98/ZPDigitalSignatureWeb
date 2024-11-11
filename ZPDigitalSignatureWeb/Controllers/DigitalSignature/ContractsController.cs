using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static ZPDigitalSignatureWeb.Common.OutputApi;
using ZPDigitalSignatureWeb.Models.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace ZPDigitalSignatureWeb.Controllers.DigitalSignature
{
    public class ContractsController : ApiController
    {
        #region -- Configuration --

        DBContext db = new DBContext();

        public class SignContractEntity
        {
            public Guid ID { get; set; }
            public Guid? CustomerID { get; set; }
            public Guid? FileSignID { get; set; }
            public Guid? FileImageID { get; set; }
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
            public string FileBase64_Signed { get; set; }
            public DateTime? CreatedOn { get; set; }
            public Guid? CreatedBy { get; set; }
            public DateTime? ModifiedOn { get; set; }
            public Guid? ModifiedBy { get; set; }
        }

        public class ContractsEntity {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
            public int Role { get; set; }
            public string KeySearch { get; set; }
            public Guid? CustomerID { get; set; }
        }

        #endregion

        #region -- Method --

        #region -- Contracts --

        #region -- Create contract need to sign --

        [HttpPost]
        [Route("api/create-contract")]

        public IHttpActionResult CreateContractToSign(tbl_Contracts entity)
        {
            try
            {
                var id = Guid.NewGuid();
                var item = new tbl_Contracts();

                item.ID = id;
                item.CustomerID = entity.CustomerID;
                item.FileContractID = entity.FileContractID;
                item.ContractTypeID = entity.ContractTypeID;
                item.ContractName = entity.ContractName;
                item.Note = entity.Note;
                item.Status = entity.Status;
                item.CreatedOn = DateTime.Now;
                item.CreatedBy = Guid.Empty;
                item.ModifiedOn = DateTime.Now;
                item.ModifiedBy = Guid.Empty;

                db.tbl_Contracts.Add(item);
                db.SaveChanges();

                var rs = new
                {
                    code = Code.Success,
                    messVN = Language.VN.alertCreateDataSuccess
                };

                return Json(rs);
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Sign contract --

        [HttpPost]
        [Route("api/sign-contract")]

        public IHttpActionResult SignContract(SignContractEntity entity)
        {
            try
            {
                var checkSign = db.tbl_Contracts
                    .FirstOrDefault(x => x.ID == entity.ID);

                var checkFile = db.tbl_ContractFiles
                    .FirstOrDefault(x => x.ID == entity.FileSignID);

                if ((checkSign != null) && (checkFile != null))
                {
                    checkSign.FileImageID = entity.FileImageID;
                    checkSign.Status = entity.Status;
                    checkSign.PosX = entity.PosX;
                    checkSign.PosY = entity.PosY;
                    checkSign.WidthPlace = entity.WidthPlace;
                    checkSign.HeightPlace = entity.HeightPlace;
                    checkSign.PageSign = entity.PageSign;
                    checkSign.Reason = entity.Reason;
                    checkSign.Contact = entity.Contact;
                    checkSign.Location = entity.Location;
                    checkSign.ModifiedOn = DateTime.Now;
                    checkSign.ModifiedBy = Guid.Empty;

                    // Update file after sign
                    checkFile.FileBase64_Signed = entity.FileBase64_Signed;
                    checkFile.ModifiedOn = DateTime.Now;
                    checkFile.ModifiedBy = Guid.Empty;

                    db.SaveChanges();

                    var rs = new
                    {
                        code = Code.Success,
                        messVN = Language.VN.alertUpdateDataSuccess
                    };

                    return Json(rs);
                }
                else
                {
                    var rs = new
                    {
                        code = Code.Invalid_Data,
                        messVN = Language.VN.alertNotFoundData
                    };

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Get contract list --

        [HttpPost]
        [Route("api/contracts")]

        public IHttpActionResult GetContracts(ContractsEntity entity)
        {
            try
            {
                // Role 1: Admin get all
                // Role 2: Customer get all by CustomerID

                if (entity.Role == 1)
                {
                    var files = db.vw_Contracts
                         .Where(x => (x.SAPCode.Contains(entity.KeySearch))
                         || (x.TaxCode.Contains(entity.KeySearch))
                         || (x.FullName.Contains(entity.KeySearch))
                         || (x.ContractName.Contains(entity.KeySearch))
                         || (x.Note.Contains(entity.KeySearch)))
                         .OrderByDescending(x => x.ModifiedOn)
                         .ToList();

                    if (files.Count > 0 && files != null)
                    {
                        var count = files.Count();
                        int TotalCount = count;
                        int CurrentPage = entity.PageNumber;
                        int PageSize = entity.PageSize;
                        int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                        var items = files.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                        var rs = new
                        {
                            data = items,
                            paging = new
                            {
                                PageIndex = CurrentPage,
                                PageSize = PageSize,
                                TotalRecords = TotalCount,
                                TotalPages = TotalPages,
                            },
                            code = Code.Success
                        };

                        return Json(rs);
                    }
                    else
                    {
                        var rs = new
                        {
                            code = Code.Invalid_Data,
                            messVN = Language.VN.alertNotFoundData,
                        };
                        return Json(rs);
                    }
                }
                else
                {
                    if (entity.CustomerID != null)
                    {
                        var files = db.vw_Contracts
                           .Where(x => (x.CustomerID == entity.CustomerID) && (x.Status != 2) 
                           && ((x.SAPCode.Contains(entity.KeySearch)) 
                           || (x.TaxCode.Contains(entity.KeySearch))
                           || (x.FullName.Contains(entity.KeySearch))
                           || (x.ContractName.Contains(entity.KeySearch))
                           || (x.Note.Contains(entity.KeySearch))))
                           
                           .OrderByDescending(x => x.ModifiedOn)
                           .ToList();

                        if (files.Count > 0 && files != null)
                        {
                            var count = files.Count();
                            int TotalCount = count;
                            int CurrentPage = entity.PageNumber;
                            int PageSize = entity.PageSize;
                            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                            var items = files.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                            var rs = new
                            {
                                data = items,
                                paging = new
                                {
                                    PageIndex = CurrentPage,
                                    PageSize = PageSize,
                                    TotalRecords = TotalCount,
                                    TotalPages = TotalPages,
                                },
                                code = Code.Success
                            };

                            return Json(rs);
                        }
                        else
                        {
                            var rs = new
                            {
                                code = Code.Invalid_Data,
                                messVN = Language.VN.alertNotFoundData,
                            };
                            return Json(rs);
                        }
                    }
                    else
                    {
                        var rs = new
                        {
                            code = Code.Invalid_ID,
                            messVN = Language.VN.alertIdDoesNotExist
                        };

                        return Json(rs);
                    }

                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Get contract details --

        [HttpGet]
        [Route("api/contracts/{id}")]

        public IHttpActionResult ContractDetails(Guid id) {
            try
            {
                var checkContract = db.vw_Contracts
                    .FirstOrDefault(x => x.ID == id);

                if (checkContract != null)
                {
                    var rs = new
                    {
                        data = checkContract,
                        code = Code.Success
                    };

                    return Json(rs);
                }
                else {
                    var rs = new
                    {
                        code = Code.Invalid_ID,
                        messVN = Language.VN.alertIdDoesNotExist
                    };

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Update contract --

        [HttpPost]
        [Route("api/update-contract")]

        public IHttpActionResult UpdateContract(tbl_Contracts entity) {
            try
            {
                var checkContract = db.tbl_Contracts
                    .FirstOrDefault(x => x.ID == entity.ID);

                if (checkContract != null)
                {
                    checkContract.CustomerID = entity.CustomerID;
                    //checkContract.FileContractID = entity.FileContractID;
                    checkContract.ContractTypeID = entity.ContractTypeID;
                    checkContract.ContractName = entity.ContractName;
                    checkContract.Status = entity.Status;
                    checkContract.Note = entity.Note;
                    checkContract.ModifiedOn = DateTime.Now;
                    checkContract.ModifiedBy = Guid.Empty;

                    db.SaveChanges();

                    var rs = new
                    {
                        code = Code.Success,
                        messVN = Language.VN.alertUpdateDataSuccess
                    };

                    return Json(rs);
                }
                else {
                    var rs = new
                    {
                        code = Code.Invalid_ID,
                        messVN = Language.VN.alertIdDoesNotExist
                    };

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Delete contract --

        [HttpDelete]
        [Route("api/contracts/{id}")]

        public IHttpActionResult deleteContract(Guid id)
        {
            try
            {
                var contract = db.tbl_Contracts.FirstOrDefault(x => x.ID == id);

                if (contract != null)
                {
                    db.tbl_Contracts.Remove(contract);
                    db.SaveChanges();

                    var rs = new
                    {
                        code = Code.Success,
                        messVN = Language.VN.alertDeleteDataSuccess
                    };

                    return Json(rs);
                }
                else
                {
                    var rs = new
                    {
                        code = Code.Invalid_ID,
                        messVN = Language.VN.alertIdDoesNotExist
                    };

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Reject contract --

        [HttpPost]
        [Route("api/reject-contract")]

        public IHttpActionResult rejectContract(tbl_Contracts entity) {
            try
            {
                var contract = db.tbl_Contracts
                    .FirstOrDefault(x => x.ID == entity.ID);

                if (contract != null)
                {
                    contract.Status = 3;
                    contract.RejectNote = entity.RejectNote;
                    contract.ModifiedOn = DateTime.Now;
                    contract.ModifiedBy = Guid.Empty;

                    db.SaveChanges();

                    var rs = new
                    {
                        code = Code.Success,
                        messVN = Language.VN.alertUpdateDataSuccess
                    };

                    return Json(rs);
                }
                else {
                    var rs = new
                    {
                        code = Code.Invalid_ID,
                        messVN = Language.VN.alertIdDoesNotExist
                    };

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Get contract new for customer --

        [HttpGet]
        [Route("api/contract-news")]

        public IHttpActionResult contractNews(Guid id, bool isCus) {
            try
            {
                if (isCus == true && id != Guid.Empty)
                {
                    var contract = db.vw_ContractNews
                        .Where(x => (x.CustomerID == id) 
                        && (x.Status == 0) )
                        .OrderByDescending(x => x.ModifiedOn)
                        .ToList();

                    if (contract != null)
                    {
                        var rs = new
                        {
                            data = contract,
                            totalData = contract.Count,
                            code = Code.Success
                        };

                        return Json(rs);
                    }
                    else
                    {
                        var rs = new
                        {
                            code = Code.Invalid_Data,
                            messVN = Language.VN.alertNotFoundData
                        };

                        return Json(rs);
                    }
                }
                else {
                    var contract = db.vw_ContractNews
                            .Where(x => (x.Status == 1) || (x.Status == 3))
                            .OrderByDescending(x => x.ModifiedOn)
                            .ToList();

                    if (contract != null)
                    {
                        var rs = new
                        {
                            data = contract,
                            totalData = contract.Count,
                            code = Code.Success
                        };

                        return Json(rs);
                    }
                    else
                    {
                        var rs = new
                        {
                            code = Code.Invalid_Data,
                            messVN = Language.VN.alertNotFoundData
                        };

                        return Json(rs);
                    }
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #endregion

        #region -- Contract types --

        #region -- Create contract type --

        [HttpPost]
        [Route("api/create-contract-type")]

        public IHttpActionResult CreateContractType(tbl_ContractTypes entity)
        {
            try
            {
                var id = Guid.NewGuid();
                var item = new tbl_ContractTypes();

                item.ID = id;
                item.Code = entity.Code;
                item.Name = entity.Name;
                item.Note = entity.Note;
                item.CreatedOn = DateTime.Now;
                item.CreatedBy = Guid.Empty;
                item.ModifiedOn = DateTime.Now;
                item.ModifiedBy = Guid.Empty;

                db.tbl_ContractTypes.Add(item);
                db.SaveChanges();

                var rs = new
                {
                    code = Code.Success,
                    messVN = Language.VN.alertCreateDataSuccess
                };

                return Json(rs);
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Get contract type list --

        [HttpGet]
        [Route("api/contract-types")]

        public IHttpActionResult GetContractTypes(int pageNumber = 1, int pageSize = 10
            , string keySearch = "", bool isGetAll = false)
        {
            try
            {
                if (isGetAll == false)
                {
                    var contractTypes = db.vw_ContractTypes
                        .Where(x => (x.Name.Contains(keySearch)) 
                        || (x.Code.Contains(keySearch)) 
                        || (x.Note.Contains(keySearch)))
                        .OrderBy(x => x.Name)
                        .ToList();

                        if (contractTypes.Count > 0 && contractTypes != null)
                        {
                            var count = contractTypes.Count();
                            int TotalCount = count;
                            int CurrentPage = pageNumber;
                            int PageSize = pageSize;
                            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                            var items = contractTypes.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                            var rs = new
                            {
                                data = items,
                                paging = new
                                {
                                    PageIndex = CurrentPage,
                                    PageSize = PageSize,
                                    TotalRecords = TotalCount,
                                    TotalPages = TotalPages,
                                },
                                code = Code.Success
                            };

                            return Json(rs);
                        }
                        else
                        {
                            var rs = new
                            {
                                code = Code.Invalid_Data,
                                messVN = Language.VN.alertNotFoundData,
                            };

                            return Json(rs);
                        }
                }
                else {
                    var contractTypes = db.vw_ContractTypes
                        .Select(x => new {
                            ID = x.ID,
                            Name = x.Name
                        })
                        .OrderBy(x => x.Name)
                        .ToList();

                        if (contractTypes.Count > 0 && contractTypes != null)
                        {
                            var rs = new
                            {
                                data = contractTypes,
                                code = Code.Success
                            };

                            return Json(rs);
                        }
                        else
                        {
                            var rs = new
                            {
                                code = Code.Invalid_Data,
                                messVN = Language.VN.alertNotFoundData,
                            };

                            return Json(rs);
                        }
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }

        #endregion

        #region -- Get contract type details --
        [HttpGet]
        [Route("api/contract-types/{id}")]

        public IHttpActionResult ContractTypeDetails(Guid id)
        {
            try
            {
                var checkContractType = db.vw_ContractTypes
                    .FirstOrDefault(x => x.ID == id);

                if (checkContractType != null)
                {
                    var rs = new
                    {
                        data = checkContractType,
                        code = Code.Success
                    };

                    return Json(rs);
                }
                else
                {
                    var rs = new
                    {
                        code = Code.Invalid_ID,
                        messVN = Language.VN.alertIdDoesNotExist
                    };

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }
        #endregion

        #region -- Delete contract type --
        [HttpDelete]
        [Route("api/contract-types/{id}")]

        public IHttpActionResult deleteContractType(Guid id)
        {
            try
            {
                var contractType = db.tbl_ContractTypes.FirstOrDefault(x => x.ID == id);

                if (contractType != null)
                {
                    db.tbl_ContractTypes.Remove(contractType);
                    db.SaveChanges();

                    var rs = new
                    {
                        code = Code.Success,
                        messVN = Language.VN.alertDeleteDataSuccess
                    };

                    return Json(rs);
                }
                else
                {
                    var rs = new
                    {
                        code = Code.Invalid_ID,
                        messVN = Language.VN.alertIdDoesNotExist
                    };

                    return Json(rs);
                }
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    err = ex.Message,
                    code = Code.Exception,
                    messVN = Language.VN.alertException
                };

                return Json(rs);
            }
        }
        #endregion

        #endregion

        #endregion
    }
}
