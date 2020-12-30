using System;
using Nancy;
using Nancy.ModelBinding;
using Cpaas.Sdk;
using Cpaas.Sdk.resources;
using System.Collections.Generic;

namespace fa {
  public class App : NancyModule {
    public static Client client = null;

    public App() {
      Get("/", args => {
        if (client == null) {
          Console.WriteLine("Inizializing ...");
          client = new Client(
            Environment.GetEnvironmentVariable("CLIENT_ID"),
            Environment.GetEnvironmentVariable("CLIENT_SECRET"),
            Environment.GetEnvironmentVariable("BASE_URL")
          );
          Console.WriteLine("Inizialization Complete ...");
        }

        Session["credentialsVerified"] = "false";
        Session["codeVerified"] = "false";
        Session["codeId"] = "";

        return Response.AsRedirect("/login");
      });

      Get("/login", args => {
        if ((string)Session["credentialsVerified"] == "true" && (string)Session["codeVerified"] == "true") {
          return Response.AsRedirect("/dashboard");
        }

        return View["login.cshtml"];
      });

      Post("/login", args => {
        Auth auth = this.Bind();

        if (auth.email != Environment.GetEnvironmentVariable("EMAIL") || auth.password != Environment.GetEnvironmentVariable("PASSWORD")) {
          var alert = new Alert() {
            Message = "Invalid username or password",
            Type = "error"
          };

          return View["login.cshtml", alert];
        }

        Session["credentialsVerified"] = "true";
        Session["codeVerified"] = "false";

        return Response.AsRedirect("/verify");
      });

      Get("/verify", args => {
        if ((string)Session["credentialsVerified"] == "true" && (string)Session["codeVerified"] == "true") {
          return Response.AsRedirect("/dashboard");
        }

        if ((string)Session["credentialsVerified"] == "false") {
          return Response.AsRedirect("/logout");
        }

        return View["verify.cshtml"];
      });

      Post("/send-code", args => {
        Code code = this.Bind();

        Twofactor.TwofactorResponse response = null;

        if (code.type == "sms") {
          response = client.twofactor.SendCode(Environment.GetEnvironmentVariable("PHONE_NUMBER"), new Dictionary<string, string> {
            ["message"] = "Your verification code: {code}",
            ["method"] = "sms"
          });
        } else {
          response = client.twofactor.SendCode(Environment.GetEnvironmentVariable("DESTINATION_EMAIL"), new Dictionary<string, string> {
            ["message"] = "Your verification code: {code}",
            ["method"] = "email",
            ["subject"] = "Twofactor verification"
          });
        }

        if (response.hasError) {
          var failureAlert = new Alert() {
            Message = ErrorMessageFrom(response),
            Type = "error"
          };

          return View["verify.cshtml", failureAlert];
        }

        Session["codeId"] = response.codeId;

        var successAlert = new Alert() {
          Message = "Twofactor verification code sent successfully",
          Type = "success"
        };


        return View["verify.cshtml", successAlert];
      });

      Post("/verify", args => {
        Verification verification = this.Bind();

        var response = client.twofactor.VerifyCode(new Dictionary<string, string> {
          ["codeId"] = (string)Session["codeId"],
          ["verificationCode"] = verification.code
        });

        if (response.hasError) {
          var failureAlert = new Alert() {
            Message = ErrorMessageFrom(response),
            Type = "error"
          };

          return View["verify.cshtml", failureAlert];
        }

        if (response.verified) {
          Session["codeVerified"] = "true";
          return Response.AsRedirect("/dashboard");
        }

        var alert = new Alert() {
          Message = response.verificationMessage,
          Type = "error"
        };

        return View["verify.cshtml", alert];
      });


      Get("/dashboard", args => {
        if ((string)Session["codeVerified"] != "true" || (string)Session["credentialsVerified"] != "true") {
          return Response.AsRedirect("/login");
        }

        return View["dashboard.cshtml"];
      });

      Get("/logout", args => {
        Session["credentialsVerified"] = "false";
        Session["codeVerified"] = "false";
        Session["codeId"] = "";

        return Response.AsRedirect("/login");
      });

    }

    public class Auth {
      public string email;
      public string password;
    }

    public class Verification {
      public string code;
    }

    public class Code {
      public string type;
    }

    // Helper methods

    string ErrorMessageFrom(dynamic obj) {
      return $"{obj.errorName}: {obj.errorMessage} ({obj.errorName})";
    }
  }
}
