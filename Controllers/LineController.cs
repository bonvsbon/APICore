using System.Data;
using System.Globalization;
using System.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APICore.Models;
using APICore.Common;
using Microsoft.Extensions.Options;
using static APICore.Models.appSetting;

namespace APICore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LineController : ControllerBase
    {
        LineApiController api;
        CustomerModel customer;
        LineActionModel action; 
        Functional func;
        DataTable dt;
        public LineController(IOptions<StateConfigs> config)
        {
            api = new LineApiController();
            customer = new CustomerModel(config);
            action = new LineActionModel(config);
            func = new Functional();
        }
        [HttpPost]
        public async Task<IActionResult> Webhook(LineRequestModel request)
        {
            LineResponseModel response = new LineResponseModel();
            LineCardTemplateResponseModel cardResponse = new LineCardTemplateResponseModel();
            BubbleMain bubble = new BubbleMain();
            dupBubbleMain dupbubble = new dupBubbleMain();
            MessageResponseModel message = new MessageResponseModel();
            StickerModel sticker = new StickerModel();
            StickerMessageModel stickerMessage = new StickerMessageModel();
            CarouselMain master = new CarouselMain();
            DateTime date;
            Task<UserProfile> profile;
            string holyType = "text";
            decimal txtnumber;
            string strMessage = "";
            profile = api.GetUserProfile(request.events[0].source.userId);
            if(profile.Result != null)
            {
                action.SP_InsertUserFollow(
                    request.events[0].source.userId,
                    profile.Result.displayName,
                    profile.Result.pictureUrl,
                    profile.Result.statusMessage,
                    profile.Result.language
                );
            }

            if(request.events[0].message == null)
            {
                request.events[0].message = new MessageModel();
            }
            
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

            if(request.events[0].message.text == "เช็คยอดคงเหลือ")
            {
                customer.REST_InitialStep(request.events[0].source.userId, "1", "");
                strMessage = "รบกวนให้ลูกค้า กรอกข้อมูล รหัสบัตรประชาชน 13 หลัก";
                message = api.SetMessage("รบกวนให้ลูกค้า กรอกข้อมูล รหัสบัตรประชาชน 13 หลัก");
            }
            else if (decimal.TryParse(request.events[0].message.text, out txtnumber) && request.events[0].message.text.Length == 13)
            {
                customer.REST_InitialStep(request.events[0].source.userId, "2", request.events[0].message.text);
                strMessage = "รบกวนขอ วัน/เดือน/ปี (คศ) เกิด ของลูกค้าครับ";
                message = api.SetMessage("รบกวนขอ วัน/เดือน/ปี (คศ) เกิด ของลูกค้าครับ");
            }
            else if (DateTime.TryParseExact(request.events[0].message.text, "dd'/'MM'/'yyyy",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out date))
            {
                customer.REST_InitialStep(request.events[0].source.userId, "3", request.events[0].message.text);
                
                dt = new DataTable();
                
                dt = customer.REST_SelectFilterCondition(request.events[0].source.userId);

                if(dt.Rows.Count > 0)
                {
                    List<CustomerData> data = new List<CustomerData>();
                    data = customer.REST_GetAccountInformation(dt.Rows[0]["IDCard"].ToString(), dt.Rows[0]["BirthDay"].ToString(), "");

                    if(data.Count > 0)
                    {
                        foreach(CustomerData cus in data)
                        {
                            strMessage = func.GetMessageCondition();
                            strMessage = string.Format(strMessage, 
                                cus.AgreementNo,
                                cus.Model,
                                cus.NetFinance > 0 ? cus.NetFinance.ToString("#,###.00") : "-",
                                cus.InstallmentAmount > 0 ? cus.InstallmentAmount.ToString("#,###.00") : "-",
                                cus.DueDate,
                                cus.LastDueDate.Date.ToString("dd-MM-yyyy"),
                                cus.OSBalance > 0 ? cus.OSBalance.ToString("#,###.00") : "-",
                                cus.PeriodDue,
                                cus.CollectionAmount > 0 ? cus.CollectionAmount.ToString("#,###.00") : "-",
                                cus.PenaltyAmount > 0 ? cus.PenaltyAmount.ToString("#,###.00") : "-",
                                cus.OtherFee > 0 ? cus.OtherFee.ToString("#,###.00") : "-",
                                cus.PaymentDue > 0 ? cus.PaymentDue.ToString("#,###.00") : "-",
                                cus.PayDueDate.Date.ToString("dd-MM-yyyy") == "01-01-1900" ? "-" : cus.PayDueDate.Date.ToString("dd-MM-yyyy"),
                                cus.ODAmount > 0 ? cus.ODAmount.ToString("#,###.00") : "-",
                                cus.ODPeriodDue,
                                cus.isPastDue,
                                cus.CurrentInstallment > 0 ? cus.CurrentInstallment.ToString("#,###.00") : "-",
                                cus.ContractStatus,
                                cus.DiscountAmount > 0 ? cus.DiscountAmount.ToString("#,###.00") : "-",
                                cus.SuspensionTenor
                            );
                            message = api.SetMessage(strMessage);
                            dupbubble = new dupBubbleMain();
                            dupbubble = api.SetBubbleMessage(strMessage);
                            dupbubble.to = request.events[0].source.userId;
                            await api.CallApi(dupbubble);
                            action.SP_InsertLogRequestMessage(
                                "reply",
                                request.events[0].source.userId,
                                request.events[0].mode,
                                strMessage,
                                "",
                                DateTime.Now,
                                request.destination,
                                "SYSTEM"
                            );
                        }
                        cardResponse = api.SetCardMessage(strMessage);
                        return Ok();
                    }
                    else
                    {
                        action.SP_InsertLogRequestMessage(
                                "reply",
                                request.events[0].source.userId,
                                request.events[0].mode,
                                strMessage,
                                "",
                                DateTime.Now,
                                request.destination,
                                "SYSTEM"
                        );
                        strMessage = "สถานะสัญญาไม่สอดคล้องกับการแสดงผล รบกวนขอเบอร์ติดต่อ คุณลูกค้าเพื่อให้เจ้าหน้าที่ติดต่อให้ข้อมูลครับ";
                        message = api.SetMessage("สถานะสัญญาไม่สอดคล้องกับการแสดงผล รบกวนขอเบอร์ติดต่อ คุณลูกค้าเพื่อให้เจ้าหน้าที่ติดต่อให้ข้อมูลครับ");

                    }
                }
                else
                {
                    strMessage = "ไม่พบข้อมูลที่ต้องการค้นหา กรุณาตรวจสอบข้อมูล หรือ ลองทำรายการใหม่";
                    message = api.SetMessage("ไม่พบข้อมูลที่ต้องการค้นหา กรุณาตรวจสอบข้อมูล หรือ ลองทำรายการใหม่");
                }                
            }
            else if (request.events[0].message.text == "เช็คยอดปิดบัญชี")
            {
                message = api.SetMessage(
@"
รบกวน แจ้งข้อมูล ดังนี้ครับ
1. เลขที่สัญญา หรือ เลขหลังบัตร Next Card หรือ ทะเบียนรถ
2. ชื่อนาม-สกุลของผู้เช่าซื้อ
3. วันที่ต้องการชำระยอดปิดทั้งหมด
4. พร้อมเบอร์ติดต่อกลับที่สะดวก
เพื่อให้เจ้าหน้าที่คำนวณยอดให้ตามระบบและติดต่อกลับครับ
");
                strMessage = 
@"
รบกวน แจ้งข้อมูล ดังนี้ครับ
1. เลขที่สัญญา หรือ เลขหลังบัตร Next Card หรือ ทะเบียนรถ
2. ชื่อนาม-สกุลของผู้เช่าซื้อ
3. วันที่ต้องการชำระยอดปิดทั้งหมด
4. พร้อมเบอร์ติดต่อกลับที่สะดวก
เพื่อให้เจ้าหน้าที่คำนวณยอดให้ตามระบบและติดต่อกลับครับ
";
            }
            else if (request.events[0].message.text == "ช่องทางชำระเงิน")
            {
                message = api.SetMessage(
@"คุณสามารถชำระค่างวดรถเน็คซ์ได้ ตามช่องทางดังนี้ครับ
1. ผ่านแอปฯธนาคาร(ฟรีค่าธรรมเนียม) : ได้ทุกธนาคาร 
โดยใช้คิวร์อาร์โค้ดบนใบแจ้งชำระ แสกน หรือหากไม่มี ทักมาได้ครับ
👌 ธ.ไทยพาณิชย์ สามารถใช้บาร์โค้ดบนบัตรเน็คซ์การ์ด ชำระได้
2. ผ่านจุดรับชำระเงิน(ค่าบริการเริ่ม 10บ.) : เคาน์เตอร์เวอร์วิส, เทสโก้โลตัส, เพย์@โพส 
โดยใช้ บาร์โค้ดหลังบัตรเน็คซ์การ์ด หรือบนใบแจ้งชำระ เพื่อชำระเงิน"
                );
                strMessage = 
@"คุณสามารถชำระค่างวดรถเน็คซ์ได้ ตามช่องทางดังนี้ครับ
1. ผ่านแอปฯธนาคาร(ฟรีค่าธรรมเนียม) : ได้ทุกธนาคาร 
โดยใช้คิวร์อาร์โค้ดบนใบแจ้งชำระ แสกน หรือหากไม่มี ทักมาได้ครับ
👌 ธ.ไทยพาณิชย์ สามารถใช้บาร์โค้ดบนบัตรเน็คซ์การ์ด ชำระได้
2. ผ่านจุดรับชำระเงิน(ค่าบริการเริ่ม 10บ.) : เคาน์เตอร์เวอร์วิส, เทสโก้โลตัส, เพย์@โพส 
โดยใช้ บาร์โค้ดหลังบัตรเน็คซ์การ์ด หรือบนใบแจ้งชำระ เพื่อชำระเงิน";
            }
            else if (request.events[0].message.text == "ค้นหาสาขาเน็คซ์")
            {
                holyType = "card";
                bubble = api.SetCarouselMessage();
                strMessage = "Card Carousel";
                // message = api.SetMessage("https://www.nextcapital.co.th/locator");
            }
            else if (request.events[0].type == "follow")
            {
                message = api.SetMessage(
                string.Format(
@"สวัสดีครับ คุณ {0} 
ขอบคุณที่เป็นเพื่อนกับ Next Connect โดย เน็คซ์แคปปิตอล

ให้เราได้ช่วยเหลือคุณ หรือจะพูดคุยสอบถาม กับเราได้ตลอดเวลาครับผม", profile.Result.displayName));
                strMessage = 
                string.Format(
@"สวัสดีครับ คุณ {0} 
ขอบคุณที่เป็นเพื่อนกับ Next Connect โดย เน็คซ์แคปปิตอล

ให้เราได้ช่วยเหลือคุณ หรือจะพูดคุยสอบถาม กับเราได้ตลอดเวลาครับผม", profile.Result.displayName);
            }
            else if (request.events[0].message.text.Length == 10 && !request.events[0].message.text.Contains("-") && !request.events[0].message.text.Contains("/"))
            {
                message = api.SetMessage("ทางเราได้รับเบอร์ติดต่อแล้ว เดี๋ยวจะมีเจ้าหน้าที่ติดต่อกลับครับ");
                strMessage = "ทางเราได้รับเบอร์ติดต่อแล้ว เดี๋ยวจะมีเจ้าหน้าที่ติดต่อกลับครับ";
                sticker = api.SetStickerMessage("11537", "52002739");
            }
            else
            {
                message = api.SetMessage("รบกวนขอเบอร์ติดต่อคุณลูกค้า เพื่อให้เจ้าหน้าที่ติดต่อเพื่อให้ข้อมูลครับ");
                //message = api.SetMessage(func.JsonSerialize(request));
                strMessage = "รบกวนขอเบอร์ติดต่อคุณลูกค้า เพื่อให้เจ้าหน้าที่ติดต่อเพื่อให้ข้อมูลครับ";
            }
            
            
            if(holyType == "text")
            {
                response.replyToken = request.events[0].replyToken;
                response.messages.Add(message);   
                await api.CallApi(response);
            }
            else if (holyType == "card")
            {
                // cardResponse.to = request.events[0].source.userId;
                // await api.CallApi(cardResponse);

                bubble.to = request.events[0].source.userId;
                await api.CallApi(bubble);
            }
            else 
            {
                master.to = request.events[0].source.userId;
                await api.CallApi(master);
            }

            if(sticker.messages.Count > 0)
            {
                sticker.to = request.events[0].source.userId;
                await api.CallApi(sticker);
            }

            action.SP_InsertLogRequestMessage(
                "reply",
                request.events[0].source.userId,
                request.events[0].mode,
                strMessage,
                "",
                DateTime.Now,
                request.destination,
                "SYSTEM"
                );
            return Ok();
        }
    }
}