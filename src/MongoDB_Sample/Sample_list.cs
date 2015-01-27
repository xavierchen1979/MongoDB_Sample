namespace MongoDB_Sample
{
    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Validation;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders; 
    using MongoDB_Sample.Model;
    using MongoDB.Bson;
    using System.Web;
    using Newtonsoft.Json;

    public class Sample_list : NancyModule
    {
        private readonly string connectStr;

        public Sample_list()
        {
            this.Get["adddata", "/adddata"] = this.InsertUserData;

            this.Get["/"] = parameters =>
            { 
                return Negotiate
                    .WithModel((object)GetUserList(Request.Query))
                    .WithAllowedMediaRange(mediaRange: "application/json");
            };

            this.Get["/getListbyname"] = parameters =>
            {
                return Negotiate
                    .WithModel((object)GetUserListByName(Request.Query))
                    .WithAllowedMediaRange(mediaRange: "application/json");
            };
             
            this.connectStr = "mongodb://tester:1234@ds053438.mongolab.com:53438/xsampledb";
        }
 
        private RspUser GetUserListByName(dynamic p)
        {
            RspUser rspUser;

            if (p.ContainsKey("req"))
            {
                //req={"name": "9"} 

                var queryStr = JsonConvert.DeserializeObject(p["req"]);

                string qName = queryStr.name;

                var server = MongoServer.Create(this.connectStr);

                var db = server.GetDatabase("xsampledb");

                MongoCollection<UserModel> coll = db.GetCollection<UserModel>("Users");

                List<RspUserResult> newResultList = new List<RspUserResult>();

                var query = Query.EQ("Name", new BsonRegularExpression(qName));

                foreach (var i in coll.Find(query))
                {
                    RspUserResult newResult = new RspUserResult();
                    newResult.Age = i.Age;
                    newResult.Name = i.Name;
                    newResult.Id = i.Id.ToString();
                    newResultList.Add(newResult);
                }

                server.Disconnect();

                rspUser = new RspUser
                {
                    code = 1,
                    msg = "Success",
                    result = newResultList
                };
            }
            else
            {
                rspUser = new RspUser
                {
                    code = 0,
                    msg = "Failed",
                    result = null
                };
            }
            
            return rspUser;
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