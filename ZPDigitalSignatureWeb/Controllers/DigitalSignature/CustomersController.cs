using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZPDigitalSignatureWeb.Models.EntityModels;
using static ZPDigitalSignatureWeb.Common.OutputApi;

namespace ZPDigitalSignatureWeb.Controllers.DigitalSignature
{
    public class CustomersController : ApiController
    {
        #region -- Configuration --

        DBContext db = new DBContext();

        #endregion

        #region -- Method --

        #region -- Create customer --
        #endregion

        #region -- Get customer list --

        [HttpGet]
        [Route("api/customers")]

        public IHttpActionResult GetCustomers(int pageNumber = 1, int pageSize = 10
            , string keySearch = "", bool isGetAll = false)
        {
            try
            {
                if (isGetAll == false) {
                    var customers = db.vw_Customers
                        .Where(x => (x.FullName.Contains(keySearch))
                        || (x.SAPCode.Contains(keySearch))
                        || (x.Email.Contains(keySearch))
                        || (x.Name.Contains(keySearch))
                        || (x.Name2.Contains(keySearch))
                        || (x.Name3.Contains(keySearch))
                        || (x.Name4.Contains(keySearch))
                        || (x.TaxCode.Contains(keySearch)))
                        .OrderBy(x => x.FullName)
                        .ToList();

                    if (customers.Count > 0 && customers != null)
                    {
                        var count = customers.Count();
                        int TotalCount = count;
                        int CurrentPage = pageNumber;
                        int PageSize = pageSize;
                        int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                        var items = customers.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
                    var customers = db.vw_Customers
                        .Select(x => new {
                            ID = x.ID,
                            FullName = x.FullName,
                            Email = x.Email
                        })
                        .OrderBy(x => x.FullName)
                        .ToList();

                        if (customers.Count > 0 && customers != null)
                        {
                            var count = customers.Count();
                            int TotalCount = count;
                            int CurrentPage = pageNumber;
                            int PageSize = pageSize;
                            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                            var items = customers.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
