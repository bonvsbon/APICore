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
    public class CheckerController : ControllerBase
    {
        LineApiController api;
        LineMessageTemplate template;
        DACModel DAC;
        LineActionModel action; 
        Functional func;
        DataTable dt;
        
        public CheckerController(IOptions<StateConfigs> config)
        {
            api = new LineApiController(ChannelName: "NextforDealer");
            DAC = new DACModel(config);
            action = new LineActionModel(config);
            func = new Functional();
            template = new LineMessageTemplate();
        }
        
        public async void AcceptTask(string UserLineId, string AppNo)
        {
            PushLineResponseModel response = new PushLineResponseModel();
            MessageResponseModel message = new MessageResponseModel();
            string strmessage = "";
            dt = new DataTable();
            dt = DAC.REST_UpdateStatusApp(UserLineId, AppNo);
            try
            {
                if(dt.Rows.Count > 0)
                {
                    strmessage = template.AcceptTaskMessage();
                    strmessage = string.Format(strmessage, dt.Rows[0]["User_Name"].ToString(), dt.Rows[0]["Application_No"].ToString(), dt.Rows[0]["Application_DealerName"].ToString(), dt.Rows[0]["User_PhoneNumber"].ToString());
                    message = api.SetMessage(strmessage);
                    response.to = dt.Rows[0]["Application_CreateBy"].ToString();
                    response.messages.Add(message);
                    await api.CallApi(response);
                    DAC.REST_KeepEventTransaction("AcceptTask", response.to, "CheckerController -> AcceptTask", "[59]");
                }
                else
                {
                    DAC.REST_KeepEventTransaction("AcceptTask", AppNo, "CheckerController -> AcceptTask is Empty", "REST_UpdateStatusApp " + UserLineId + ", " + AppNo);
                }
            }
            catch(Exception e)
            {
                DAC.REST_KeepEventTransaction("AcceptTask", AppNo, "CheckerController -> AcceptTask", e.StackTrace);
            }
        }
    }
}