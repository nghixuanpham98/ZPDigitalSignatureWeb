using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using ZPDigitalSignatureWeb.Models.EntityModels;
using static ZPDigitalSignatureWeb.Common.OutputApi;

namespace ZPDigitalSignatureWeb.Controllers.DigitalSignature
{
    public class HandleFilesController : ApiController
    {
        #region -- Configuration --

        DBContext db = new DBContext();

        #endregion

        #region -- Method --

        #region -- Get file details --

        [HttpGet]
        [Route("api/file/{id}")]

        public IHttpActionResult GetFileDetails(Guid id)
        {
            try
            {
                var file = db.vw_ContractFiles
                    .FirstOrDefault(x => x.ID == id);

                if (file != null && file.FileBase64_Signed != null)
                {
                    var rs = new
                    {
                        code = Code.Success,
                        data = new
                        {
                            ID = file.ID,
                            FileBase64 = file.FileBase64_Signed
                        }
                    };

                    return Json(rs);
                }
                else if (file != null && file.FileBase64_Signed == null)
                {
                    var rs = new
                    {
                        code = Code.Success,
                        data = new
                        {
                            ID = file.ID,
                            FileBase64 = file.FileBase64
                        }
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

        #region -- Upload file --

        [HttpPost]
        [Route("api/upload-file")]

        public IHttpActionResult UploadFile(tbl_ContractFiles entity)
        {
            try
            {
                var id = Guid.NewGuid();
                var item = new tbl_ContractFiles();

                item.ID = id;
                item.FileName = entity.FileName;
                item.FileType = entity.FileType;
                item.FileBase64 = entity.FileBase64;
                item.FileBase64_Signed = null;
                item.CreatedOn = DateTime.Now;
                item.CreatedBy = Guid.Empty;
                item.ModifiedOn = DateTime.Now;
                item.ModifiedBy = Guid.Empty;

                db.tbl_ContractFiles.Add(item);
                db.SaveChanges();

                var rs = new
                {
                    code = Code.Success,
                    fileID = id,
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

        #endregion
    }
}
