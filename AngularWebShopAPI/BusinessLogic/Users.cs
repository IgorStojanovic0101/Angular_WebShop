using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Api.Models.myDatabase;
using Api.Models.myModels;
using Api.Models.myEnums;
namespace Api.BusinessLogic
{
    public class User : Base
    {
        public User() : base(null) { }

        public User(mydatabaseEntities _dbContext)
             : base(_dbContext)
        { }

        public List<UserModel> GetUsers()
        {
            var users = dBContext.tb_users.Where(x=> !x.admin).Select(x => new UserModel
            {
                UserPk = x.user_pk,
                FirstName = x.user_firstName,
                LastName = x.user_lastName,
                Username = x.user_username,
                
                

            }).ToList();

         
            return users;
            //return users;
        }
        public UserModel GetUserById(int userId)
        {
            
            return dBContext.tb_users.Where(x => x.user_pk == userId).Select(x => new UserModel
            {
                UserPk = x.user_pk,
                FirstName = x.user_firstName,
                LastName = x.user_lastName,
                Username = x.user_username,
                isAdmin = x.admin

            }).SingleOrDefault();
        }
       
        public  UserModel FindUsers(UserModel model)
        {
            var users = dBContext.tb_users.Where(x => x.user_username == model.Username).Select(x => new UserModel
            {
                UserPk = x.user_pk,   
                FirstName = x.user_firstName,
                LastName = x.user_lastName,
                Username = x.user_username,
                isAdmin = x.admin

            }).SingleOrDefault();


            return users;    
        }

        public SharedModel SetUp(UserModel model)
        {
            var returnModel = new SharedModel { errors = new List<string>() };

            if (this.GetUsers().Any(x => x.Username == model.Username))
            {
                returnModel.errors.Add("Username is in use!");
                returnModel.Status = CreateStatus.validation;
            }
            if (!model.Password.Equals(model.CnfPassword))
            {
                returnModel.errors.Add("Passwords are not the same!");
                returnModel.Status = CreateStatus.validation;
            }


            if (returnModel.errors.Count == 0)
            {
                try
                {
                    var user = new tb_users
                    {
                        user_firstName = model.FirstName,
                        user_lastName = model.LastName,
                        user_username = model.Username,
                        user_password = model.Password,
                        createdby = "admin",
                        createdondate = DateTime.Now,
                        start_date = DateTime.Now,

                    };

                     dBContext.tb_users.Add(user);
                     dBContext.SaveChanges();

                    returnModel.RecordPk = user.user_pk;

                    returnModel.Status = CreateStatus.sucess;
                }
                catch (Exception ex)
                {
                    returnModel.errors.Add(ex.Message);
                    returnModel.Status = CreateStatus.failed;
                    //throw ex;
                }
            }
            return returnModel;
        }
    }
}