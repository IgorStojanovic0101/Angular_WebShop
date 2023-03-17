using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.BusinessLogic;
using System.Linq.Expressions;
using System.Web.Http.Cors;

namespace Diplomski.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class UserController : ApiController
    {

        private User user = new User();
        public UserController()
         {  }



        [HttpGet]
        public List<UserModel> GetUsers() => user.GetUsers();


        [HttpPost]
        public UserModel FindUsers(UserModel model) => user.FindUsers(model);


        [HttpPost]
        public SharedModel SetUp(UserModel model) => user.SetUp(model);


        [HttpPost]
        public UserModel GetUserById( int userId) => user.GetUserById(userId);

    }
}