﻿using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Text;
using Sadna_17_B_Backend.Controllers;

namespace Sadna_17_B_Frontend.Views
{

    public partial class SignUp : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        private async Task<string> SignUpUser(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UserDto { Username = username, Password = password };
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string prefix = "https://localhost:44334";
                HttpResponseMessage response = await client.PostAsync(prefix+ "/api/RestAPI/signup", content); // add relative path
                if (response.IsSuccessStatusCode)
                {
                    return null; // Sign up successful
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Sign up failed: {errorMessage}";
                }
            }
        }

        protected async void Page_Load(object sender, EventArgs e)
        {

        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected async void btnSignUp_Click(object sender, EventArgs e)
        {
            string message = "";
            if (txtUsername.Text.Length == 0)
            {
                message += "Username cannot be empty;";
            }
            if (txtPassword.Text.Length == 0)
            {
                message += " Password cannot be empty;";
            }
            if (txtConfirmPassword.Text.Length == 0)
            {
                message += " Password confirmation cannot be empty.";
            }
            else if (!txtPassword.Text.Equals(txtConfirmPassword.Text))
            {
                message += " Passwords do not match.";
            }
            if (message != "")
            {
                lblMessage.Text = message;
            }
            else
            {
                message = await SignUpUser(txtUsername.Text, txtPassword.Text);
                if (message != null)
                {
                    lblMessage.Text = message;
                }
                else
                {
                    MessageBox("Signed up successfuly, redirecting to login page");
                    //Response.Redirect("~/Views/Login"); // Redirects to the login page after a successful sign up.
                }
            }
        }

    }

}