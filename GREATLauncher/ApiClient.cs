﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace GREATLauncher
{
    public class ApiClient
    {
        private const string BASE_URI = "http://172.17.104.126:3000/api/v1/";

        private WebClient client = new WebClient();

        private string token;
        public string Token
        {
            get
            {
                return this.token;
            }
        }

        public class User
        {
            public int id { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public bool admin { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public bool SignIn(string email, string password)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] reqData = enc.GetBytes("email=" + HttpUtility.UrlEncode(email) + "&password=" + HttpUtility.UrlEncode(password));

            HttpWebRequest req = WebRequest.CreateHttp(BASE_URI + "sessions");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = reqData.Length;

            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(reqData, 0, reqData.Length);
                reqStream.Flush();
                reqStream.Close();
            }

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse()) {
                if (resp.StatusCode != HttpStatusCode.InternalServerError) {
                    using (StreamReader respReader = new StreamReader(resp.GetResponseStream())) {
                        string respData = respReader.ReadToEnd();
                        respReader.Close();

                        Dictionary<string, string> respJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(respData);
                        if (resp.StatusCode == HttpStatusCode.OK) {
                            this.token = respJson["token"];
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool SignOut()
        {
            if (String.IsNullOrEmpty(this.token)) throw new InvalidOperationException();
            HttpWebRequest req = WebRequest.CreateHttp(BASE_URI + "sessions/" + this.token);
            req.Method = "DELETE";
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse()) {
                if (resp.StatusCode != HttpStatusCode.InternalServerError) {
                    using (StreamReader respReader = new StreamReader(resp.GetResponseStream())) {
                        string respData = respReader.ReadToEnd();
                        respReader.Close();

                        Dictionary<string, string> respJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(respData);
                        if (resp.StatusCode == HttpStatusCode.OK) {
                            this.token = respJson["token"];
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public User GetUser()
        {
            if (String.IsNullOrEmpty(this.token)) throw new InvalidOperationException();
            HttpWebRequest req = WebRequest.CreateHttp(BASE_URI + "users/?auth_token=" + this.token);
            req.Method = "GET";
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse()) {
                if (resp.StatusCode != HttpStatusCode.InternalServerError) {
                    using (StreamReader respReader = new StreamReader(resp.GetResponseStream())) {
                        string respData = respReader.ReadToEnd();
                        respReader.Close();

                        if (resp.StatusCode == HttpStatusCode.OK) {
                            return JsonConvert.DeserializeObject<User>(respData);
                        }
                    }
                }
            }
            return null;
        }

        public User GetUser(int id)
        {
            if (String.IsNullOrEmpty(this.token)) throw new InvalidOperationException();
            HttpWebRequest req = WebRequest.CreateHttp(BASE_URI + "users/" + id.ToString() + "?auth_token=" + this.token);
            req.Method = "GET";
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse()) {
                if (resp.StatusCode != HttpStatusCode.InternalServerError) {
                    using (StreamReader respReader = new StreamReader(resp.GetResponseStream())) {
                        string respData = respReader.ReadToEnd();
                        respReader.Close();

                        if (resp.StatusCode == HttpStatusCode.OK) {
                            return JsonConvert.DeserializeObject<User>(respData);
                        }
                    }
                }
            }
            return null;
        }
    }
}