using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit;
using Smtp = MailKit.Net.Smtp;
using System.Net.Mail;
using Test.Site.umbraco.models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using MailKit.Security;

namespace Test.Site.umbraco.controllers;

public class EmailController : SurfaceController
{
    private readonly IConfiguration _configuration;
    public EmailController(IConfiguration configuration, IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Submit(ContactViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }
        SendEmail(model);
        TempData["SubmitSuccess"] = true;
        return RedirectToCurrentUmbracoPage();
    }

    private void SendEmail(ContactViewModel contact)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(contact.Name, contact.Email));
            message.To.Add(new MailboxAddress("Website Support", _configuration.GetValue<string>("Smtp:MainAddress")));
            message.Subject = "Fox Website Message - sent at " + DateTime.Now.ToString() + ", from " + contact.Name + " at " + contact.Email;
            message.Body = new TextPart("plain") { Text = contact.MessageBody };


            using (var smtp = new Smtp.SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_configuration.GetValue<string>("Smtp:MainAddress"), _configuration.GetValue<string>("Smtp:AuthenticatePassword")); 
                smtp.Send(message);
                smtp.Disconnect(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }        
    }
}
