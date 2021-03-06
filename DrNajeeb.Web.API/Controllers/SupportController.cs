﻿using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using DrNajeeb.Web.API.Models;
using System.Net.Mail;
using System.Text;
using DrNajeeb.EF;
using DrNajeeb.Web.API.Helpers;

namespace DrNajeeb.Web.API.Controllers
{
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class SupportController : BaseController
    {

        public SupportController(IUow uow)
        {
            _Uow = uow;
        }

        [ActionName("SupportMessage")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SupportMessage(SupportModel model)
        {
            try
            {
                //var userId = User.Identity.GetUserId();
                //var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();

                ////Assign the smtp credentials for gmail
                //SmtpClient smtp = new SmtpClient();
                //if (true)
                //{
                //    smtp.Host = "mail.ps-demo.com";
                //    smtp.Port = 25;
                //    smtp.Credentials = new NetworkCredential("info@ps-demo.com", "@Dmin123");
                //    smtp.Timeout = 20000;
                //}

                //MailAddress fromAddress = new MailAddress("info@ps-demo.com", "Support Request From " + user.FullName);
                //MailAddress toAddress = new MailAddress("support@drnajeeblectures.com");

                ////Passing values to smtp object
                //dynamic message = new MailMessage(fromAddress, toAddress);
                //message.Subject = model.Subject;

                //var sb = new StringBuilder("");
                //sb.Append("<b>From : </b>" + user.Email);
                //sb.Append("<br />");
                //sb.Append("<b>Time : </b>" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                //sb.Append("<br />");
                //sb.Append("<b>Name : </b>" + user.FullName);
                //sb.Append("<br />");
                //sb.Append("<h3>Message</h3>");
                //sb.Append(model.Message);
                //message.Body = sb.ToString();
                //message.IsBodyHtml = true;


                ////Send email
                //smtp.Send(message);
                //return Ok();

                var message = new SupportMessage();
                message.Active = true;
                message.FromUserId = User.Identity.GetUserId();
                message.IsFromAdmin = false;
                message.IsFromUser = true;
                message.IsRead = false;
                message.MessageDatetime = DateTime.UtcNow;
                message.MessageText = model.Message;

                _Uow._SupportMessages.Add(message);
                await _Uow.CommitAsync();

                var messageModel = new UserMessagesModel();
                messageModel.Id = message.Id;
                messageModel.IsFromAdmin = message.IsFromAdmin.GetValueOrDefault();
                messageModel.IsFromUser = message.IsFromUser.GetValueOrDefault();
                messageModel.IsRead = message.IsRead.GetValueOrDefault();
                messageModel.MessageDateTime = message.MessageDatetime.GetValueOrDefault();
                messageModel.MessageText = message.MessageText;

                return Ok(messageModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("SupportMessageReply")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> SupportMessageReply(SupportModel model)
        {
            try
            {

                var message = new SupportMessage();
                message.Active = true;
                message.FromUserId = User.Identity.GetUserId();
                message.IsFromAdmin = true;
                message.IsFromUser = false;
                message.IsRead = false;
                message.MessageDatetime = DateTime.UtcNow;
                message.MessageText = model.Message;
                message.ToUserId = model.ToUserId;

                _Uow._SupportMessages.Add(message);
                await _Uow.CommitAsync();

                var messageModel = new UserMessagesModel();
                messageModel.Id = message.Id;
                messageModel.IsFromAdmin = message.IsFromAdmin.GetValueOrDefault();
                messageModel.IsFromUser = message.IsFromUser.GetValueOrDefault();
                messageModel.IsRead = message.IsRead.GetValueOrDefault();
                messageModel.MessageDateTime = message.MessageDatetime.GetValueOrDefault();
                messageModel.MessageText = message.MessageText;

                await LogHelpers.SaveLog(_Uow, "Reply To Message. Reply : " + model.Message, User.Identity.GetUserId());

                return Ok(messageModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("LoadUserMessages")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> LoadUserMessages()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var messagesList = new List<UserMessagesModel>();

                var messages = await _Uow._SupportMessages
                                .GetAll(x => x.FromUserId == userId || x.ToUserId == userId)
                                .Include(x => x.AspNetUser).ToListAsync();

                foreach (var message in messages)
                {
                    var messageModel = new UserMessagesModel();
                    messageModel.Id = message.Id;
                    messageModel.IsFromAdmin = message.IsFromAdmin.GetValueOrDefault();
                    messageModel.IsFromUser = message.IsFromUser.GetValueOrDefault();
                    messageModel.IsRead = message.IsRead.GetValueOrDefault();
                    messageModel.MessageDateTime = message.MessageDatetime.GetValueOrDefault();
                    messageModel.MessageText = message.MessageText;
                    messageModel.ReplierName = message.AspNetUser.FullName;

                    messagesList.Add(messageModel);

                    if (message.IsFromAdmin.Value && message.IsRead == false)
                    {
                        message.IsRead = true;
                        _Uow._SupportMessages.Update(message);
                    }
                }

                await _Uow.CommitAsync();
                return Ok(messagesList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetUserMessages")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetUserMessages(string userId)
        {
            try
            {

                var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();

                var messagesList = new List<UserMessagesModel>();

                var messages = await _Uow._SupportMessages
                                .GetAll(x => x.FromUserId == userId || x.ToUserId == userId).ToListAsync();

                foreach (var message in messages)
                {
                    var messageModel = new UserMessagesModel();
                    messageModel.Id = message.Id;
                    messageModel.IsFromAdmin = message.IsFromAdmin.GetValueOrDefault();
                    messageModel.IsFromUser = message.IsFromUser.GetValueOrDefault();
                    messageModel.IsRead = message.IsRead.GetValueOrDefault();
                    messageModel.MessageDateTime = message.MessageDatetime.GetValueOrDefault();
                    messageModel.MessageText = message.MessageText;

                    messagesList.Add(messageModel);


                }

                foreach (var message in messages)
                {
                    if (message.IsFromUser.Value && message.IsRead == false)
                    {
                        message.IsRead = true;
                        _Uow._SupportMessages.Update(message);
                    }
                }

                await _Uow.CommitAsync();

                //await LogHelpers.SaveLog(_Uow, "View All Messages Of " + user.UserName, User.Identity.GetUserId());

                return Ok(messagesList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetContactRequestUsers")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetContactRequestUsers(int page = 1, int itemsPerPage = 20)
        {
            try
            {
                var usersModelList = new List<SupportUsersModel>();
                var users = _Uow._SupportMessages.GetAll(x => x.Active == true && x.IsFromAdmin == false)
                    .OrderByDescending(x => x.MessageDatetime)
                    .Select(x => x.AspNetUser)
                    .Distinct();

                var totalContactedUsers = await users.CountAsync();

                var selectedUsers = await _Uow._SupportMessages.GetAll(x => x.Active == true && x.IsFromAdmin == false)
                    .OrderByDescending(x => x.MessageDatetime)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .Select(x => x.AspNetUser)
                    .Distinct()
                    .ToListAsync();


                foreach (var user in selectedUsers)
                {
                    var supportUser = new SupportUsersModel();
                    supportUser.UserId = user.Id;
                    supportUser.UserName = user.UserName;
                    supportUser.FullName = user.FullName;
                    supportUser.TotalUnreadMessages = await _Uow._SupportMessages.CountAsync(x => (x.FromUserId == user.Id && x.IsRead == false));

                    usersModelList.Add(supportUser);
                }

                var json = new
                {
                    data = usersModelList,
                    count = totalContactedUsers
                };

                //await LogHelpers.SaveLog(_Uow, "View All Contacted Users", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("TotalUserNewMesage")]
        [HttpGet]
        public async Task<IHttpActionResult> TotalUserNewMessages()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var totalUnreadMessages = await _Uow._SupportMessages
                    .CountAsync(x => x.ToUserId == userId && x.IsRead == false);
                return Ok(totalUnreadMessages);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("SendMessageToAll")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> SendMessageToAll(SupportModel model)
        {
            try
            {
                _Uow._MessageToAll.Add(new MessageToAll
                {
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedOn = DateTime.UtcNow,
                    IsSendToAll = false,
                    MesssageText = model.Message
                });
                await _Uow.CommitAsync();
                var totalUsers = _Uow.SendMessageToAll();

                await LogHelpers.SaveLog(_Uow, "Send Message To All. Message : " + model.Message, User.Identity.GetUserId());

                return Ok(totalUsers);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [ActionName("GetTotalUnreadMessagesForAdmin")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetTotalUnreadMessagesForAdmin()
        {
            try
            {
                var totalUnreadMessages = await _Uow._SupportMessages
                    .GetAll(x => x.Active == true && x.IsFromAdmin == false && x.IsRead == false)
                    .CountAsync();

                //await LogHelpers.SaveLog(_Uow, "Check Total Unread Messages", User.Identity.GetUserId());

                return Ok(totalUnreadMessages);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
