using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EmployeeDataAccess;
using System.Threading;

namespace EmployeeService.Controllers
{
    public class EmployeesController : ApiController
    {
        [BasicAuthentication]
        public HttpResponseMessage Get(string gender="All")
        {
            try
            {
                string username = Thread.CurrentPrincipal.Identity.Name;
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    switch (username.ToLower())
                    {
                        case "all":
                            return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.ToList());
                        case "male":
                            return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.Where(x=>x.Gender == "male").ToList());
                        case "female":
                            return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.Where(x => x.Gender == "female").ToList());
                        default:
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            
        }

        //[HttpGet]
        public HttpResponseMessage GetEmployee(int id)
        {
            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                var emp = entities.Employees.FirstOrDefault(x=>x.ID ==  id);

                if (emp != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, emp);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, $"Employee with id:{id} not found.");
                }
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Employee emp)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    entities.Employees.Add(emp);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, emp);
                    message.Headers.Location = new Uri(Request.RequestUri + "/" + emp.ID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var emp = entities.Employees.FirstOrDefault(x => x.ID == id);

                    if (emp == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, $"Employee with id={id} not found");
                    }
                    else
                    {
                        entities.Employees.Remove(emp);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, $"Record with Id:{id} deleted successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]Employee emp)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var dbemp = entities.Employees.FirstOrDefault(x => x.ID == id);

                    if (dbemp == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, $"Employee with id={id} not found");
                    }
                    else
                    {
                        dbemp.FirstName = emp.FirstName;
                        dbemp.LastName = emp.LastName;
                        dbemp.Gender = emp.Gender;
                        dbemp.Salary = emp.Salary;
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, $"Record with Id:{id} updated successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
