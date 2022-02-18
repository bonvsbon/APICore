using System.Net.Http;
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
using System.Net.Http.Headers;

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
            // api = new LineApiController();
            DAC = new DACModel(config);
            action = new LineActionModel(config);
            func = new Functional();
            template = new LineMessageTemplate();
            checker = new CheckerController(config);
        }

        [HttpPost]
        public async Task<IActionResult> TestConnection(LineRequestModel request)
        {
            LineResponseModel response = new LineResponseModel();
            LineMessageTemplate.FlexMessageMain flex = new LineMessageTemplate.FlexMessageMain();
            MessageResponseModel message = new MessageResponseModel();
            LineMessageTemplate.RichMenuMain menu = new LineMessageTemplate.RichMenuMain();
            Task<UserProfile> profile;
            Task<LineMessageTemplate.RichMenuResponse> result;
            DataTable checkHelperCase = new DataTable();
            string msg = "";
            string typeofMenu = "";
            
            try
            {
                message = api.SetMessage("กรุณากรอก Secret Code ที่ได้รับ เพื่อเริ่มต้นใช้งาน");
                for(int i = 0; i < request.events.Count; i++)
                {
                    response.replyToken = request.events[i].replyToken;
                    response.messages.Add(message);  
                    // await api.CallApi(response);
                    try
                    {
                        using(HttpClient client = new HttpClient())
                        {
                            action.SP_InsertLogRequestMessage(
                                "Call API",
                                "",
                                "request.events[0].mode",
                                "",
                                "Before Call API",
                                DateTime.Now,
                                "request.events[0].source.userId",
                                "request.events[0].source.userId"
                            );
                            StringContent content = new StringContent(func.JsonSerialize(response),
                            System.Text.Encoding.UTF8, 
                            "application/json");
                            client.DefaultRequestHeaders.Authorization 
                                        = new AuthenticationHeaderValue("Bearer", "q281ubFyT1L3Z1gAyrcLdLY4mHv2hXJFqAb/MEUO2OncgbgXdSsR6BDCXsrTZh0I3haZwDDaz1lrKF694gC0fTnp/CnbLma8WkiHW3UXwSf6gHxU5lNJP/IYeb1+KQRFeun9E5jJT8qx9lpQpY1S9AdB04t89/1O/w1cDnyilFU=");
                            var sresult = await client.PostAsync("https://api.line.me/v2/bot/message/reply", content);
                            var contents = await sresult.Content.ReadAsStringAsync(); 

                            action.SP_InsertLogRequestMessage(
                                "Call API",
                                "",
                                "request.events[0].mode",
                                "",
                                contents[0].ToString(),
                                DateTime.Now,
                                "request.events[0].source.userId",
                                "request.events[0].source.userId"
                            );

                        }
                    }
                    catch(Exception e)
                    {
                        action.SP_InsertLogRequestMessage(
                        "request.events[0].type",
                        "",
                        "Error",
                        "",
                        e.Message,
                        DateTime.Now,
                        "request.events[0].source.userId",
                        "request.events[0].source.userId"
                        );

                        return Ok();
                    }
                }
                return Ok();   
            }
            catch (Exception e)
            {
                action.SP_InsertLogRequestMessage(
                "request.events[0].type",
                "",
                "request.events[0].mode",
                "",
                e.Message,
                DateTime.Now,
                "request.events[0].source.userId",
                "request.events[0].source.userId"
                );

                return Ok();
            }
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
            DataTable checkHelperCase = new DataTable();
            string msg = "";
            string typeofMenu = "";

            profile = api.GetUserProfile(request.events[0].source.userId);
            checkHelperCase = DAC.REST_CheckHelperCase(request.events[0].source.userId);
           // KeepLog
            action.SP_InsertLogRequestMessage(
                request.events[0].type,
                request.destination,
                request.events[0].mode,
                request.events[0].type == "message" ? request.events[0].message.text : "",
                func.JsonSerialize(request),
                DateTime.Now,
                request.events[0].source.userId,
                request.events[0].source.userId
                );

            if(request.events[0].type == "follow")
            {
                if(profile.Result != null)
                {
                    action.SP_InsertUserFollow(
                        request.events[0].source.userId,
                        profile.Result.displayName == null ? "" : profile.Result.displayName,
                        profile.Result.pictureUrl == null ? "" : profile.Result.pictureUrl,
                        profile.Result.statusMessage == null ? "" : profile.Result.statusMessage,
                        profile.Result.language == null ? "" : profile.Result.language
                    );
                }
                msg = string.Format(template.FollowMessage(), profile.Result.displayName);
                message = api.SetMessage(msg);
                response.replyToken = request.events[0].replyToken;
                response.messages.Add(message);   
                await api.CallApi(response);
            }
            else if (request.events[0].message.text == "เช็คสถานะ")
            {
                dt = new DataTable();
                dt = DAC.REST_CheckStatustoFlexMessage(request.events[0].source.userId);
                flex = template.SetupFlexMessage(dt);
                flex.to = request.events[0].source.userId;
                await api.CallApi(flex);
                return Ok();
            }
            else if (request.events[0].message.text == "ช่วยเหลือ")
            {
                message = api.SetMessage("ต้องการให้เราช่วยอะไรครับ");
                response.replyToken = request.events[0].replyToken;
                response.messages.Add(message);   
                await api.CallApi(response);
                action.SP_InsertLogRequestMessage(
                    "reply",
                    request.events[0].source.userId,
                    request.events[0].mode,
                    request.events[0].message.text,
                    "",
                    DateTime.Now,
                    request.destination,
                    "SYSTEM"
                );
                return Ok();   
            }
            else if (checkHelperCase.Rows.Count > 0) // Helper Case
            {
                if(checkHelperCase.Rows[0]["Log_Message"].ToString() == "ช่วยเหลือ")
                {
                    message = api.SetMessage("ทางเราได้รับทราบปัญหาแล้ว จะเร่งดำเนินการแก้ไขให้ครับ");
                    response.replyToken = request.events[0].replyToken;
                    response.messages.Add(message);   
                    await api.CallApi(response);
                    action.SP_InsertLogRequestMessage(
                        "reply",
                        request.events[0].source.userId,
                        request.events[0].mode,
                        request.events[0].message.text,
                        "",
                        DateTime.Now,
                        request.destination,
                        "SYSTEM"
                    );
                    dt = new DataTable();
                    dt = DAC.REST_GetNeedHelpMessage(request.events[0].source.userId);
                    string alertLine = template.MessageNeedHelp();
                    if(dt.Rows.Count > 0)
                    {
                        alertLine = string.Format(alertLine, dt.Rows[0]["User_Role"].ToString(), dt.Rows[0]["BranchCode"].ToString(), dt.Rows[0]["User_Name"].ToString(), dt.Rows[0]["RequestDate"].ToString(), dt.Rows[0]["message"].ToString());

                        await api.MessageToGroupSupport(alertLine);
                    }

                    return Ok();
                }
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
                        if(dt.Rows[0]["User_Role"].ToString() == "Checker" || dt.Rows[0]["User_Role"].ToString() == "BranchHead")
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
            dupBubbleMulticastNoFooter nofooter = new dupBubbleMulticastNoFooter();
            DAC.REST_KeepEventTransaction("PushMessage", func.JsonSerialize(request), "Push", "[364]");
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
                    dt.Rows[0]["Application_PhoneNumber"].ToString(),
                    dt.Rows[0]["Application_DealerCode"].ToString(),
                    dt.Rows[0]["Dealer_BranchCode"].ToString(),
                    dt.Rows[0]["PhoneNumber"].ToString()
                );
                dt = new DataTable();
                dt = DAC.REST_GetCheckerList(request.AppNo);
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    bubble = new dupBubbleMulticast();
                    nofooter = new dupBubbleMulticastNoFooter();
                    if(dt.Rows[i]["action"].ToString() == "NoFooter")
                    {
                        try
                        {
                            nofooter = api.SetBubbleMessageMultiCastNoFooter(strmessage, request.AppNo);
                            nofooter.to.Add(dt.Rows[i]["User_LineUserId"].ToString());
                            await api.CallApiMultiCast(nofooter);
                            DAC.REST_KeepEventTransaction("PushMessage : NoFooter", func.JsonSerialize(nofooter.to), "Push", "[395]");
                        } 
                        catch(Exception e)
                        {
                            DAC.REST_KeepEventTransaction("PushMessage : NoFooter", func.JsonSerialize(nofooter.to), "Push", e.StackTrace);
                        }
                    }
                    else
                    {
                        try
                        {
                            bubble = api.SetBubbleMessageMultiCast(strmessage, request.AppNo);
                            bubble.to.Add(dt.Rows[i]["User_LineUserId"].ToString());
                            await api.CallApiMultiCast(bubble);
                            DAC.REST_KeepEventTransaction("PushMessage : haveFooter", func.JsonSerialize(bubble.to), "Push", "[409]");
                        } 
                        catch(Exception e)
                        {
                            DAC.REST_KeepEventTransaction("PushMessage : haveFooter", func.JsonSerialize(bubble.to), "Push", e.StackTrace);
                        }
                        
                    }
                }
                

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
            
            DAC.REST_KeepEventTransaction("NoticeTracking", func.JsonSerialize(request), "NoticeTracking", "[436]");

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
            try
            {
                if(dt.Rows.Count > 0)
                {
                    for(int i = 0; i < dt.Rows.Count; i++)
                    {
                        response.to.Add(dt.Rows[i]["Receiver"].ToString());
                    }   
                    await api.CallApiMultiCast(response);
                    DAC.REST_KeepEventTransaction("NoticeTracking", func.JsonSerialize(response.to), "DealerController -> CallApiMultiCast", "[461]");
                }
                else
                {
                    DAC.REST_KeepEventTransaction("NoticeTracking", request.ApplicationNo, "DealerController -> REST_GetUserforNotice is Empty", "REST_UpdateStatusApp " + request.ApplicationNo + ", " + request.State);
                }
            }
            catch (Exception e) 
            {
                DAC.REST_KeepEventTransaction("NoticeTracking", request.ApplicationNo, "DealerController", e.StackTrace);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CheckerAcceptTask(ExternalRequest request)
        {
            PushLineResponseModel response = new PushLineResponseModel();
            MessageResponseModel message = new MessageResponseModel();
            LineMessageTemplate template = new LineMessageTemplate();
            dt = new DataTable();
            DAC.REST_KeepEventTransaction("CheckerAcceptTask", func.JsonSerialize(request), "CheckerAcceptTask", "[487]");
            dt = DAC.CheckApplicationNo(request.AppNo);
            if(dt.Rows.Count > 0)
            {
                dtifExists = new DataTable();
                dtifExists = DAC.REST_CheckAceptTaskExisting(request.AppNo);
                try
                {
                    if(dtifExists.Rows.Count > 0)
                    {
                        if(!string.IsNullOrEmpty(dtifExists.Rows[0]["Application_Responsibility"].ToString()))
                        {
                            message = api.SetMessage("ไม่สามารถทำรายการได้เนื่องจากมีคนกดรับงานไปแล้ว");
                            response.to = request.LineUserId;
                            response.messages.Add(message);   
                            await api.CallApi(response);
                            DAC.REST_KeepEventTransaction("CheckerAcceptTask", response.to, "DealerController -> CallApi", "[496]");
                            return Ok();    
                        }
                    }
                    else
                    {
                        DAC.REST_KeepEventTransaction("CheckerAcceptTask", request.AppNo, "DealerController -> REST_CheckAceptTaskExisting is Empty", "REST_CheckAceptTaskExisting " + request.AppNo);
                    }
                    try
                    {
                        checker.AcceptTask(request.LineUserId, request.AppNo);
                        message = api.SetMessage("บันทึกข้อมูลสำเร็จ");
                        response.to = request.LineUserId;
                        response.messages.Add(message);   
                        await api.CallApi(response);
                        DAC.REST_KeepEventTransaction("CheckerAcceptTask", response.to, "DealerController -> CallApi", "[511]");
                    }
                    catch (Exception e)
                    {
                        DAC.REST_KeepEventTransaction("CheckerAcceptTask", request.AppNo, "DealerController -> AcceptTask", e.StackTrace);
                    }
                }
                catch (Exception e)
                {
                    DAC.REST_KeepEventTransaction("CheckerAcceptTask", request.AppNo, "DealerController -> REST_CheckApplicationNo", e.StackTrace);
                }
                return Ok();
            }
            else
            {
                DAC.REST_KeepEventTransaction("CheckerAcceptTask", request.AppNo, "DealerController -> REST_CheckApplicationNo is Empty", "REST_CheckApplicationNo " + request.AppNo);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PushMessageOnTaskOverTime(List<ExternalRequest> requests)
        {
            dupBubbleMulticast bubble = new dupBubbleMulticast();
            dupBubbleMulticastNoFooter nofooter = new dupBubbleMulticastNoFooter();
            DataTable dtAppInfo;
            DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime", func.JsonSerialize(requests), "PushMessageOnTaskOverTime", "[545]");
            if(requests.Count > 0)
            {
                foreach(ExternalRequest ex in requests)
                {
                    dt = new DataTable();
                    dtAppInfo = new DataTable();
                    dt = DAC.REST_SelectPendingTaskByAppNo(ex.AppNo);
                    
                    if(dt.Rows.Count > 0)
                    {
                        string strmessage = template.MessageAlertTaskList();
                        dtAppInfo = DAC.REST_GetApplicationInformation(ex.AppNo);
                        if(dtAppInfo.Rows.Count > 0)
                        {
                            DateTime date = Convert.ToDateTime(dtAppInfo.Rows[0]["Application_CreateDate"].ToString());
                            strmessage = string.Format(
                                strmessage,
                                dtAppInfo.Rows[0]["Application_DealerName"].ToString(),
                                dtAppInfo.Rows[0]["Application_No"].ToString(), 
                                date.ToString("dd/MM/yyyy HH:mm"),
                                dtAppInfo.Rows[0]["Application_CustomerName"].ToString(),
                                dtAppInfo.Rows[0]["Area"].ToString(),
                                dtAppInfo.Rows[0]["Application_PhoneNumber"].ToString()
                            );
                        }
                        else
                        {
                            DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime", ex.AppNo, "DealerController -> REST_GetApplicationInformation is Empty", "REST_GetApplicationInformation " + ex.AppNo);
                        }

                        foreach(DataRow row in dt.Rows)
                        {
                            try
                            {
                                if(row["action"].ToString() == "NoFooter")
                                {
                                    nofooter = api.SetBubbleMessageMultiCastNoFooter(strmessage, ex.AppNo, "แจ้งเตือนไม่มีผู้รับงาน", "#fdb813");
                                    nofooter.to.Add(row["User_LineUserId"].ToString());
                                    await api.CallApiMultiCast(nofooter);
                                    DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime : State is " + row["action"].ToString(), func.JsonSerialize(nofooter), "DealerController -> CallApiMultiCast", "[578]");
                                }
                                else
                                {
                                    bubble = api.SetBubbleMessageMultiCast(strmessage, ex.AppNo, "แจ้งเตือนไม่มีผู้รับงาน", "#fdb813");
                                    bubble.to.Add(row["User_LineUserId"].ToString());
                                    await api.CallApiMultiCast(bubble);
                                    DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime : State is " + row["action"].ToString(), func.JsonSerialize(bubble), "DealerController -> CallApiMultiCast", "[585]");
                                }
                            }
                            catch (Exception e)
                            {
                                if(row["action"].ToString() == "NoFooter")
                                {
                                    DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime", func.JsonSerialize(nofooter), "DealerController -> CallApiMultiCast", e.StackTrace);
                                }
                                else
                                {
                                    DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime", func.JsonSerialize(bubble), "DealerController -> CallApiMultiCast", e.StackTrace);
                                }
                            }
                        }
                    }
                    else
                    {
                        DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime", ex.AppNo, "DealerController -> REST_SelectPendingTaskByAppNo is Empty", "REST_SelectPendingTaskByAppNo " + ex.AppNo);
                    }
                }
            }
            else
            {
                DAC.REST_KeepEventTransaction("PushMessageOnTaskOverTime", requests.Count.ToString(), "DealerController -> ExternalRequest is Empty", "PushMessageOnTaskOverTime");
            }

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> ServiceWorking()
        {
            return Ok("Working");
        }


        [HttpPost]
        public async Task<IActionResult> TestNotice(ExternalRequest request)
        {
            dupBubbleMulticast bubble = new dupBubbleMulticast();
            dupBubbleMulticastNoFooter nofooter = new dupBubbleMulticastNoFooter();
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
                    dt.Rows[0]["Application_PhoneNumber"].ToString(),
                    dt.Rows[0]["Application_DealerCode"].ToString(),
                    dt.Rows[0]["Dealer_BranchCode"].ToString(),
                    dt.Rows[0]["PhoneNumber"].ToString()
                );
                dt = new DataTable();
                dt = DAC.REST_GetCheckerList(request.AppNo);
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    bubble = new dupBubbleMulticast();
                    nofooter = new dupBubbleMulticastNoFooter();
                    if(dt.Rows[i]["action"].ToString() == "NoFooter")
                    {
                        nofooter = api.SetBubbleMessageMultiCastNoFooter(strmessage, request.AppNo);
                        nofooter.to.Add(dt.Rows[i]["User_LineUserId"].ToString());
                        // await api.CallApiMultiCast(nofooter);
                    }
                    else
                    {
                        bubble = api.SetBubbleMessageMultiCast(strmessage, request.AppNo);
                        bubble.to.Add(dt.Rows[i]["User_LineUserId"].ToString());
                        // await api.CallApiMultiCast(bubble);
                    }
                }
            }

            return Ok();
        }
    }
}