namespace MongoDB_Sample
{
    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Validation;
    using MongoDB.Driver;
    using MongoDB_Sample.Model;

    public class Sample_list : NancyModule
    {
        private readonly string connectStr;

        public Sample_list()
        {
            this.Get["index","/index"] = this.InsertUserData;

            this.Get["list", "/list"] = parameters =>
            { 
                return Negotiate
                    .WithModel((object)GetUserList(Request.Query))
                    .WithAllowedMediaRange(mediaRange: "application/json");
            };
             
            this.connectStr = "mongodb://tester:1234@ds053438.mongolab.com:53438/xsampledb";
        }
 

        private RspUser GetUserList(dynamic p)
        {
            var server = MongoServer.Create(this.connectStr);

            var db = server.GetDatabase("xsampledb");

            MongoCollection<UserModel> coll = db.GetCollection<UserModel>("Users");

            List<RspUserResult> newResultList = new List<RspUserResult>();

            foreach (var i in coll.FindAll())
            {
                RspUserResult newResult = new RspUserResult();
                newResult.Age = i.Age;
                newResult.Name = i.Name;
                newResult.Id = i.Id.ToString();
                newResultList.Add(newResult);
            }

            server.Disconnect();

            RspUser rspUser = new RspUser
                   {
                       code = 1,
                       msg = "Success",
                       result = newResultList
                   };
            return rspUser;
        }

        private Negotiator InsertUserData(dynamic p)
        {
            
            var server = MongoServer.Create(connectStr);

            var db = server.GetDatabase("xsampledb");

            MongoCollection<UserModel> coll = db.GetCollection<UserModel>("Users");

            for (int i = 0; i < 10;i++ )
            {
                UserModel newuser = new UserModel { Name = "name" + i, Age = i };
                coll.Insert(newuser);
            }

            server.Disconnect();
            
            return null;
        }
    }
}