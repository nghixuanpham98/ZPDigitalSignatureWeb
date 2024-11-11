using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZPDigitalSignatureWeb.Models.EntityModels;
using static ZPDigitalSignatureWeb.Common.Function;
using static ZPDigitalSignatureWeb.Common.OutputApi;

namespace ZPDigitalSignatureWeb.Controllers.DigitalSignature
{
    public class LoginController : ApiController
    {
        #region -- Configuration --

        DBContext db = new DBContext();

        public class UserLoginEntity
        {
            public string SoldCode { get; set; }
            public string Password { get; set; }
        }

        public class UserLoginNewEntity
        {
            public string SoldCode { get; set; }
            public string Password { get; set; }
            public string PasswordNew { get; set; }
        }

        #endregion

        #region -- Method --

        #region -- Register account --
        [HttpPost]
        [Route("api/register")]

        public IHttpActionResult RegisterAccount(UserLoginEntity input) {
            try
            {
                var checkCustomer = db.tbl_Customers
                    .FirstOrDefault(x => x.SAPCode == input.SoldCode);

                if (checkCustomer != null)
                {
                    var checkAccount = db.tbl_Accounts
                    .FirstOrDefault(x => x.SoldCode == input.SoldCode);

                    if (checkAccount == null)
                    {
                        var id = Guid.NewGuid();
                        var item = new tbl_Accounts();
                        var pass = VerifyMD5.GetMd5Hash(input.Password);

                        // Role 1: Admin
                        // Role 2: Customer

                        item.ID = id;
                        item.CustomerID = checkCustomer.ID;
                        item.SoldCode = input.SoldCode;
                        item.Password = pass;
                        item.DisplayName = checkCustomer.FullName;
                        item.PhoneNumber = checkCustomer.PhoneNumber;
                        item.Email = checkCustomer.Email;
                        item.Role = 2;
                        item.CreatedOn = DateTime.Now;
                        item.CreatedBy = Guid.Empty;
                        item.ModifiedOn = DateTime.Now;
                        item.ModifiedBy = Guid.Empty;

                        db.tbl_Accounts.Add(item);
                        db.SaveChanges();

                        var rs = new
                        {
                            code = Code.Success,
                            messVN = Language.VN.alertCreateDataSuccess
                        };

                        return Json(rs);
                    }
                    else {
                        var rs = new
                        {
                            code = Code.Exception,
                            messVN = Language.VN.alertUsAlreadyExisted
                        };

                        return Json(rs);
                    }
                }
                else {
                    var rs = new
                    {
                        code = Code.Invalid_Data,
                        messVN = Language.VN.alertInvalidCode
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

        #region -- Login --

        [HttpPost]
        [Route("api/login")]

        public IHttpActionResult Login(UserLoginEntity input)
        {
            try
            {
                var pass = VerifyMD5.GetMd5Hash(input.Password);

                var check = db.vw_Accounts
                    .FirstOrDefault(x => (x.SoldCode == input.SoldCode)
                    && (x.Password == pass));

                if (check != null)
                {
                    var rs = new
                    {
                        data = check,
                        code = Code.Success
                    };

                    return Json(rs);
                }
                else
                {
                    var rs = new
                    {
                        code = Code.Invalid_User,
                        messVN = Language.VN.alertInvalidUsPw
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

        #region -- Change password --

        [HttpPost]
        [Route("api/change-password")]

        public IHttpActionResult ChangePassword(UserLoginNewEntity input) {
            try
            {
                var pass = VerifyMD5.GetMd5Hash(input.Password);

                var checkAcc = db.tbl_Accounts
                    .FirstOrDefault(x => (x.SoldCode == input.SoldCode) 
                    && (x.Password == pass));

                if (checkAcc != null) {
                    var passNew = VerifyMD5.GetMd5Hash(input.PasswordNew);

                    checkAcc.Password = passNew;
                    checkAcc.ModifiedOn = DateTime.Now;
                    checkAcc.ModifiedBy = Guid.Empty;

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
                        code = Code.Invalid_User,
                        messVN = Language.VN.alertInvalidUsPw
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
    }
}
