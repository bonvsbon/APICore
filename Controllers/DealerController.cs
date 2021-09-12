using System.Diagnostics;
using System.Data;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using APICore.Models;
using static APICore.Models.appSetting;
using Microsoft.Extensions.Options;
using APICore.Common;
using bBarcode = BarcodeLib.Barcode;
using System.Drawing;
using QRCoder;
using System.IO;

namespace APICore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class DealerController : ControllerBase
    {
        LineApiController api;
        LineMessageTemplate template;
        DACModel DAC;
        LineActionModel action; 
        Functional func;
        DataTable dt;
        DataTable dtifExists;
        CheckerController checker;
        
        public DealerController(IOptions<StateConfigs> config)
        {
            api = new LineApiController(ChannelName: "NextforDealer");
            DAC = new DACModel(config);
            action = new LineActionModel(config);
            func = new Functional();
            template = new LineMessageTemplate();
            checker = new CheckerController(config);
        }
        [HttpPost]
        public async Task<IActionResult> WebHook(LineRequestModel request)
        {
            LineResponseModel response = new LineResponseModel();
            LineMessageTemplate.FlexMessageMain flex = new LineMessageTemplate.FlexMessageMain();
            MessageResponseModel message = new MessageResponseModel();
            LineMessageTemplate.RichMenuMain menu = new LineMessageTemplate.RichMenuMain();
            Task<UserProfile> profile;
            Task<LineMessageTemplate.RichMenuResponse> result;
            string msg = "";
            string typeofMenu = "";

            profile = api.GetUserProfile(request.events[0].source.userId);

            if(request.events[0].type == "follow")
            {
                msg = string.Format(template.FollowMessage(), profile.Result.displayName);
                message = api.SetMessage(msg);
                response.replyToken = request.events[0].replyToken;
                response.messages.Add(message);   
                await api.CallApi(response);
            }
            else if (request.events[0].message.text == "เช็คสถานะ")
            {
                flex = template.SetupFlexMessage();
                flex.replyToken = request.events[0].source.userId;
            }
            else
            {
                if(request.events[0].message.text == "เริ่มต้นใช้งาน")
                {
                    message = api.SetMessage("กรุณากรอก Secret Code ที่ได้รับ เพื่อเริ่มต้นใช้งาน");
                    response.replyToken = request.events[0].replyToken;
                    response.messages.Add(message);   
                    await api.CallApi(response);
                    return Ok();   
                }
                // Check is AppNo
                dt = new DataTable();
                dt = DAC.CheckApplicationNo(request.events[0].message.text);
                if(dt.Rows.Count > 0)
                {
                    dtifExists = new DataTable();
                    dtifExists = DAC.REST_CheckAceptTaskExisting(request.events[0].message.text);
                    if(dtifExists.Rows.Count > 0)
                    {
                        if(!string.IsNullOrEmpty(dtifExists.Rows[0]["Application_Responsibility"].ToString()))
                        {
                            message = api.SetMessage("ไม่สามารถทำรายการได้เนื่องจากมีคนกดรับงานไปแล้ว");
                            response.replyToken = request.events[0].replyToken;
                            response.messages.Add(message);   
                            await api.CallApi(response);
                            return Ok();    
                        }
                    }
                    checker.AcceptTask(request.events[0].source.userId, dt.Rows[0]["Application_No"].ToString());
                    message = api.SetMessage("บันทึกข้อมูลสำเร็จ");
                    response.replyToken = request.events[0].replyToken;
                    response.messages.Add(message);   
                    await api.CallApi(response);
                    return Ok();
                }

                dt = new DataTable();
                dt = DAC.CheckSecretCode(request.events[0].message.text);
                if(dt.Rows.Count == 0)
                {
                    msg = string.Format(template.UnAuthorizeMessage(), profile.Result.displayName);
                    message = api.SetMessage(msg);
                    response.replyToken = request.events[0].replyToken;
                    response.messages.Add(message);   
                    await api.CallApi(response);
                }
                else
                {
                    dt = new DataTable();
                    dt = DAC.MatchingUser(request.events[0].source.userId, request.events[0].message.text);

                    if(dt.Rows.Count > 0)
                    {
                        if(dt.Rows[0]["User_Role"].ToString() == "Checker")
                        {
                            typeofMenu = "checker";
                            menu = template.SetupRichMenuChecker(request.events[0].source.userId);
                        }
                        else
                        {
                            typeofMenu = "dealer";
                            menu = template.SetupRichMenuDealer(request.events[0].source.userId);
                        }

                        if(string.IsNullOrEmpty(dt.Rows[0]["User_RichMenuID"].ToString()))
                        {
                            result = api.SetupMenu(menu, typeofMenu);
                            await api.SetupBackgroundMenu(result.Result.richMenuId, typeofMenu + ".png");
                            if(result != null)
                            {
                                try
                                {
                                    await api.SetDefaultMenu(result.Result.richMenuId, request.events[0].source.userId);
                                }
                                catch(Exception e)
                                {
                                    await api.DeleteMenu(result.Result.richMenuId);
                                    return BadRequest(e.Message);
                                }
                                finally
                                {
                                    DAC.SetupRichmenubyUser(request.events[0].source.userId, result.Result.richMenuId);
                                }
                            }
                        }
                        
                        dt = new DataTable();
                        dt = DAC.GetUserInformation(request.events[0].source.userId);
                        if(dt.Rows.Count > 0)
                        {
                            msg = string.Format(
                                            template.PassAuthorizeMessage()
                                            , dt.Rows[0]["User_Name"].ToString()
                                            , dt.Rows[0]["User_Code"].ToString()
                                            , dt.Rows[0]["User_Area"].ToString()
                                            );
                            message = api.SetMessage(msg);
                            response.replyToken = request.events[0].replyToken;
                            response.messages.Add(message);   
                            await api.CallApi(response);
                        }
                    }
                    else
                    {

                    }
                }

            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Push(ExternalRequest request)
        {
            dupBubbleMulticast bubble = new dupBubbleMulticast();
            string strmessage = template.MessageAlertTaskList();
            // Get Data for string format
            dt = new DataTable();
            dt = DAC.REST_GetApplicationInformation(request.AppNo);
            if(dt.Rows.Count > 0)
            {
                DateTime date = Convert.ToDateTime(dt.Rows[0]["Application_CreateDate"].ToString());
                strmessage = string.Format(
                    strmessage,
                    dt.Rows[0]["Application_DealerName"].ToString(),
                    dt.Rows[0]["Application_No"].ToString(), 
                    date.ToString("dd/MM/yyyy HH:mm"),
                    dt.Rows[0]["Application_CustomerName"].ToString(),
                    dt.Rows[0]["Area"].ToString(),
                    dt.Rows[0]["Application_PhoneNumber"].ToString()
                );
                bubble = api.SetBubbleMessageMultiCast(strmessage, request.AppNo);
                dt = new DataTable();
                dt = DAC.REST_GetCheckerList();
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    bubble.to.Add(dt.Rows[i]["User_LineUserId"].ToString());
                }
                await api.CallApiMultiCast(bubble);
            }
                        

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> NoticeTracking(ExternalNotice request)
        {
            PushLineResponseMultiCastModel response = new PushLineResponseMultiCastModel();
            MessageResponseModel message = new MessageResponseModel();
            LineMessageTemplate template = new LineMessageTemplate();
            dt = new DataTable();
            string msg = template.MessageNotice(request.State, request);
            string statecancel;
            message.type = "text";
            message.text = msg;

            response.messages.Add(message);
            
            if(request.State == "cancel" && string.IsNullOrEmpty(request.CheckerName))
            {
                // Dealer Cancel
               request.State = "dealerupdate"; 
            }
            else if (request.State == "cancel" && !string.IsNullOrEmpty(request.CheckerName))
            {
                // Checker Cancel
                request.State = "checkerupdate";
            }

            dt = DAC.REST_GetUserforNotice(request.ApplicationNo, request.State);
            if(dt.Rows.Count > 0)
            {
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    response.to.Add(dt.Rows[i]["Receiver"].ToString());
                }   
            }
            await api.CallApiMultiCast(response);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CheckerAcceptTask(ExternalRequest request)
        {
            PushLineResponseModel response = new PushLineResponseModel();
            MessageResponseModel message = new MessageResponseModel();
            LineMessageTemplate template = new LineMessageTemplate();
            dt = new DataTable();
            dt = DAC.CheckApplicationNo(request.AppNo);
            if(dt.Rows.Count > 0)
            {
                dtifExists = new DataTable();
                dtifExists = DAC.REST_CheckAceptTaskExisting(request.AppNo);
                if(dtifExists.Rows.Count > 0)
                {
                    if(!string.IsNullOrEmpty(dtifExists.Rows[0]["Application_Responsibility"].ToString()))
                    {
                        message = api.SetMessage("ไม่สามารถทำรายการได้เนื่องจากมีคนกดรับงานไปแล้ว");
                        response.to = request.LineUserId;
                        response.messages.Add(message);   
                        await api.CallApi(response);
                        return Ok();    
                    }
                }
                checker.AcceptTask(request.LineUserId, request.AppNo);
                message = api.SetMessage("บันทึกข้อมูลสำเร็จ");
                response.to = request.LineUserId;
                response.messages.Add(message);   
                await api.CallApi(response);
                return Ok();
            }

            return Ok();
        }

    }
}