using CodeCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CodeCafe.Controllers
{
    public class DashboardController : ApiController
    {
        mysqliEntities db = new mysqliEntities();
        [HttpGet Route("details")]
        public HttpResponseMessage GetDetails()
        {
            try
            {
                var data = new
                {
                    Category = db.Categories.Count(),
                    product = db.Products.Count(),
                    bill = db.Bills.Count(),
                    user = db.Users.Count()
                };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
