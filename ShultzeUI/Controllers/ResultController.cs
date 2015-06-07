using ShultzeUI.Models.Result;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShultzeUI.Controllers
{
    public class ResultController : Controller
    {
        //
        // GET: /Index/
        private static string connectionString = @"Server=LENOVO-PC\SQLEXPRESS;Database=Shultze; User Id=sa;  Password=su;";

        public ActionResult Index(Int32? id) //nullable Int32!!!1111
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                ResultModel model = new ResultModel();

                if (id.HasValue)
                {
                    model.Responses = ShultzeResponse.ShultzeMethods.ProcessResponses(connection, id.Value);
                }
                model.Themes = ShultzeResponse.ShultzeMethods.GetThemes(connection);
                return View("Index", model);
            }
        }

    }
}
