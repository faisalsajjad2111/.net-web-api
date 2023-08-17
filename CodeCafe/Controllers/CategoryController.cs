﻿using CodeCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CodeCafe.Controllers
{
    public class CategoryController : ApiController
    {
        mysqliEntities db = new mysqliEntities();
        Response response = new Response();
        [HttpPost Route("addNewCategory")]
        // [CustomAuthenticationFilter]

        public HttpResponseMessage AddNewCategory([FromBody] Category category)
        {
            try
            {
                /*  var token = Request.Headers.GetValues("authorization").First();
                  TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                  if (tokenClaim.Role != "admin")
                  {
                      return Request.CreateResponse(HttpStatusCode.Unauthorized);
                  }
                  */
                db.Categories.Add(category);
                db.SaveChanges();
                response.message = "Category added successfuly";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }
        [HttpGet Route("getAllCategory")]
      //  [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllCategory()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, db.Categories.ToList());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }
        [HttpPost Route("updateCategory")]
        // [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateCategory(Category category)
        {
            try
            {
                /*  var token = Request.Headers.GetValues("authorization").First();
                  TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                  if (tokenClaim.Role != "admin")
                  {
                      return Request.CreateResponse(HttpStatusCode.Unauthorized);
                  }
                  */
                Category categoryObj = db.Categories.Find(category.id);
                if(categoryObj == null)
                {
                    response.message = "Category id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                categoryObj.name = category.name;
                db.Entry(categoryObj).State = System.Data.Entity.EntityState.Modified;
               
                db.SaveChanges();
                response.message = "Category Updated successfuly";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }
    }
}