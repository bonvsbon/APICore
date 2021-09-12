using System.ComponentModel;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using APICore.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace APICore.Common
{
    public class LineMessageTemplate
    {

        public string FollowMessage()
        {
            string message = 
            @"
สวัสดีคุณ {0} 
เพื่อเริ่มต้นการใช้งาน
ขอทราบ Secret Code ที่ได้รับทาง SMS
สำหรับยืนยันตัวตนด้วยครับ 
            ";

            return message;
        }
        public string UnAuthorizeMessage()
        {
            string message = 
            @"
เรียนคุณ {0} 
ทางเราไม่พบ Secret Code ที่คุณกรอกมาในระบบ
รบกวนตรวจสอบความถูกต้อง และ ลองยืนยันใหม่อีกครั้งครับ 
            ";

            return message;
        }
        public string PassAuthorizeMessage()
        {
            string message = 
            @"
ยืนยันตัวตนสำเร็จ !!

ยินดีต้อนรับคุณ {0} 
รหัสพนักงาน {1}
พื้นที่รับผิดชอบ {2}
            ";

            return message;
        }
        public string AcceptTaskMessage()
        {
            string message = 
@"{0} กดรับงานแล้ว!
เลขงาน : {1}
{2}
เบอร์ติดต่อ : 0941610031
";

            return message;
        }

        public object ConfigRichMenu()
        {

            return null;
        }

        public string MessageAlertTaskList()
        {
            string message = 
            @"Dealer : {0}
เลขงาน : {1}
วัน/เวลาที่ขอ : {2}

ลูกค้า : {3}
พื้นที่ : {4}
เบอร์ติดต่อ : {5}";

            return message;
        }

        public string MessageNotice(string state, ExternalNotice request)
        {
            string message = "";
            if(state == "checkerupdate" && request.TextStatus != "กดรับงาน")
            {
                message = string.Format(@"{0} {1}แล้ว!
เลขงาน {2}
{3}
หมายเหตุ : {4}", request.CheckerName, request.TextStatus.Replace("แล้ว", ""), request.ApplicationNo, request.DealerName, request.Remark);
            }
            else if (state == "checkerupdate" && request.TextStatus == "กดรับงาน")
            {
                message = string.Format(@"{0} {1}แล้ว!
เลขงาน {2}
{3}
หมายเหตุ : {4}

เบอร์ติดต่อ : {5}", request.CheckerName, request.TextStatus.Replace("แล้ว", ""), request.ApplicationNo, request.DealerName, request.Remark, "0941610031");
            }
            else if (state == "dealerupdate")
            {
                message = string.Format(@"{0} {1}แล้ว!
เลขงาน {2}
หมายเหตุ : {3}", request.DealerName, request.TextStatus.Replace("แล้ว", ""), request.ApplicationNo, request.Remark);
            }
            else if (state == "cancel")
            {
                message = string.Format(@"{0} กดยกเลิกงาน!
เลขงาน {1}
{2}
เหตุผล : {3}", string.IsNullOrEmpty(request.CheckerName) ? request.DealerName : request.CheckerName, request.ApplicationNo, !string.IsNullOrEmpty(request.CheckerName) ? request.DealerName : "", request.Remark);
            }
            
            return message;
        }
        #region Flex Message
        public FlexMessageMain SetupFlexMessage()
        {
            FlexMessageMain main = new FlexMessageMain();
            FlexMessage messages = new FlexMessage();
            FlexCarousel carousel = new FlexCarousel();
            FlexBubble bubble = new FlexBubble();
            FlexBubbleHeader header = new FlexBubbleHeader();
            FlexBubbleHeaderContent hcontent = new FlexBubbleHeaderContent();
            FlexBubbleHero hero = new FlexBubbleHero();
            FlexBubbleHeroAction haction = new FlexBubbleHeroAction();
            FlexBubbleBody body = new FlexBubbleBody();
            FlexBubbleBodyContent bcontent = new FlexBubbleBodyContent();
            FlexBubbleFooter footer = new FlexBubbleFooter();
            FlexBubbleFooterContent fcontent = new FlexBubbleFooterContent();
            FlexBubbleFooterContentAction fcaction = new FlexBubbleFooterContentAction();

            messages.type = "flex";
            messages.altText = "Check Status Task";
            carousel.type = "carousel";

            // for loop for script

            // for (int i = 0; i < dt.Rows.Count; i++)
            header = new FlexBubbleHeader();
            hcontent = new FlexBubbleHeaderContent();
            hero = new FlexBubbleHero();
            haction = new FlexBubbleHeroAction();
            body = new FlexBubbleBody();
            bcontent = new FlexBubbleBodyContent();
            footer = new FlexBubbleFooter();
            fcontent = new FlexBubbleFooterContent();
            fcaction = new FlexBubbleFooterContentAction();

            bubble.type = "bubble";
            // header
            header.type = "box";
            header.layout = "horizontal";
            header.position = "relative";
            hcontent.contents = new List<object>();
            hcontent.type = "text";
            hcontent.text = "25";
            hcontent.weight = "bold";
            hcontent.size = "md";
            hcontent.color = "#000000";
            header.contents.Add(hcontent);
            bubble.header = header;
            // end header

            // hero
            haction.type = "uri";
            haction.label = "Action";
            haction.uri = "https://synergy.nextcapital.co.th"; // **
            hero.type = "image";
            hero.url = "https://www.nextcapital.co.th/uploads/06F1/files/6f5e1cd72348e6a75239ae9f2bcf8042.jpg"; // **
            hero.size = "full";
            hero.aspectRatio = "20:13";
            hero.aspectMode = "cover";
            hero.action = haction;
            bubble.hero = hero;
            // end hero

            // body
            body.type = "box";
            body.layout = "horizontal";
            body.spacing = "md";
            bcontent.contents = new List<object>();
            bcontent.type = "text";
            bcontent.text = "message body"; // **
            bcontent.weight = "regular";
            bcontent.wrap = "true";
            bcontent.style = "normal";
            body.contents.Add(bcontent);
            bubble.body = body;
            // end body

            // footer
            footer.type = "box";
            footer.layout = "horizontal";
            fcontent.type = "button";
            fcaction.type = "uri";
            fcaction.label = "Label Footer"; // **
            fcaction.uri = "https://synergy.nextcapital.co.th"; // **
            fcontent.action = fcaction;
            footer.contents.Add(fcontent);
            bubble.footer = footer;
            // end footer

            carousel.contents.Add(bubble);

            messages.contents = carousel;
            
            main.messages.Add(messages);

            return main;
        }
        #endregion
        #region Carousel
        public BubbleMain SetCarouselMessage()
        {
            BubbleMain main = new BubbleMain();
            BubbleSubMain subMain = new BubbleSubMain();
            BubbleTemplate template = new BubbleTemplate();
            BubbleHeader header = new BubbleHeader();
            BubbleHeaderContents headerContents = new BubbleHeaderContents();
            BubbleHero hero = new BubbleHero();
            BubbleBody body = new BubbleBody();
            BubbleContents contents = new BubbleContents();
            BubbleFooter footer = new BubbleFooter();
            BubbleFooterContents footerContents = new BubbleFooterContents();

            subMain.type = "flex";
            subMain.altText = "This is a Flex Message";

            template.type = "bubble";
            header.type = "box";
            header.layout = "horizontal";
            header.position = "relative";
            headerContents.type = "text";
            headerContents.text = "25 สาขาทั่วประเทศไทย";
            headerContents.weight = "bold";
            headerContents.size = "md";
            headerContents.color = "#000000";
            headerContents.contents = new List<object>();
            
            header.contents.Add(headerContents);

            hero.type = "image";
            hero.url = "https://www.nextcapital.co.th/uploads/06F1/files/6f5e1cd72348e6a75239ae9f2bcf8042.jpg";
            hero.size = "full";
            hero.aspectRatio = "20:13";
            hero.aspectMode = "cover";
            hero.action.type = "uri";
            hero.action.label = "Action";
            hero.action.uri = "https://www.nextcapital.co.th/locator";
            body.type = "box";
            body.layout = "horizontal";
            body.spacing = "md";
            contents.type = "text";
            contents.text = "จันทร์ - ศุกร์ 9.00-18.00น.";
            contents.weight = "regular";
            contents.wrap = true;
            contents.style = "normal";
            contents.contents = new List<object>();
            
            body.contents.Add(contents);

            footer.type = "box";
            footer.layout = "horizontal";
            footerContents.type = "button";
            footerContents.action.type = "uri";
            footerContents.action.label = "กรุงเทพ ปริมณทล";
            footerContents.action.uri = "https://www.nextcapital.co.th/locator/9";

            footer.contents.Add(footerContents);

            footerContents = new BubbleFooterContents();

            footerContents.type = "button";
            footerContents.action.type = "uri";
            footerContents.action.label = "ต่างจังหวัด";
            footerContents.action.uri = "https://www.nextcapital.co.th/locator";

            footer.contents.Add(footerContents);

            template.header = header;
            template.hero = hero;
            template.body = body;
            template.footer = footer;
            subMain.contents = template;
            main.messages.Add(subMain);

            return main;
            // CarouselMain main = new CarouselMain();
            // CarouselMaster master = new CarouselMaster();
            // CarouselTemplate template = new CarouselTemplate();
            // CardDefaultAction uriAction = new CardDefaultAction();
            // ColumnsCarousel column = new ColumnsCarousel();
            // CarouselActions actions = new CarouselActions();

            // master.type = "template";
            // master.altText = "this is a carousel template";
            // template.type = "carousel";
            // template.imageAspecRatio = "rectangle";
            // column.thumbnailImageUrl = "https://www.nextcapital.co.th/uploads/06F1/files/6f5e1cd72348e6a75239ae9f2bcf8042.jpg";
            // column.title = "25 สาขาทั่วประเทศไทย";
            // column.text = "จันทร์ - ศุกร์ 9.00 - 18.00";
            // column.imageBackgroundColor = "#FFFFFF";
            // uriAction.type = "uri";
            // uriAction.label = "กรุงเทพ ปริมณทล";
            // uriAction.uri = "https://www.nextcapital.co.th/locator";
            // column.actions.Add(uriAction);
            // uriAction = new CardDefaultAction();
            // uriAction.type = "uri";
            // uriAction.label = "ต่างจังหวัด";
            // uriAction.uri = "https://www.nextcapital.co.th/locator";
            // column.actions.Add(uriAction);
            // template.columns.Add(column);
            // master.template = template;

            // main.messages.Add(master);

            // return main;
        }
#endregion
        #region Setup Rich Menu
        public RichMenuMain SetupRichMenuChecker(string LineUserId)
        {
            RichMenuMain rich = new RichMenuMain();
            RichMenuStyle style = new RichMenuStyle();
            List<RichMenuArea> areas = new List<RichMenuArea>();
            RichMenuArea area = new RichMenuArea();
            RichMenuBound bound = new RichMenuBound();
            RichMenuAction action = new RichMenuAction();
            RichMenuText txtMessage = new RichMenuText();

            rich.selected = "true";
            rich.name = "Checker Menu";
            rich.chatBarText = "Checker Menu";
            style.width = "2500";
            style.height = "1686";
            rich.size = style;

            // bound 1
            bound.x = "0";
            bound.y = "0";
            bound.width = "1242";
            bound.height = "844";
            action.type = "uri";
            action.uri = "https://synergy.nextcapital.co.th/webtest/DACApps/TaskList?UserLineId=" + LineUserId + "&cmd=Pending";
            area.bounds = bound;
            area.action = action;
            // areas.Add(area);
            rich.areas.Add(area);

            // bound 2
            bound = new RichMenuBound();
            action = new RichMenuAction();
            area = new RichMenuArea();
            bound.x = "2";
            bound.y = "853";
            bound.width = "1237";
            bound.height = "833";
            // action.type = "uri";
            // action.uri = "https://synergy.nextcapital.co.th/webtest/DACApps/TaskList?UserLineId=" + LineUserId + "&cmd=Status";
            txtMessage.type = "message";
            txtMessage.text = "เช็คสถานะ";
            area.bounds = bound;
            area.action = txtMessage;
            // areas.Add(area);
            rich.areas.Add(area);
            
            // bound 3
            bound = new RichMenuBound();
            action = new RichMenuAction();
            area = new RichMenuArea();
            bound.x = "1250";
            bound.y = "6";
            bound.width = "1244";
            bound.height = "841";
            action.type = "uri";
            action.uri = "https://synergy.nextcapital.co.th/webtest/DACApps/TaskList?UserLineId=" + LineUserId + "&cmd=OnHand";
            area.bounds = bound;
            area.action = action;
            // areas.Add(area);
            rich.areas.Add(area);

            return rich;
        }
        public RichMenuMain SetupRichMenuDealer(string LineUserId)
        {
            RichMenuMain rich = new RichMenuMain();
            RichMenuStyle style = new RichMenuStyle();
            RichMenuArea area = new RichMenuArea();
            RichMenuBound bound = new RichMenuBound();
            RichMenuAction action = new RichMenuAction();
            RichMenuText txtMessage = new RichMenuText();

            rich.selected = "true";
            rich.name = "Dealer Menu";
            rich.chatBarText = "Dealer Menu";
            style.width = "2500";
            style.height = "1686";
            rich.size = style;

            // bound 1
            bound.x = "0";
            bound.y = "0";
            bound.width = "1255";
            bound.height = "828";
            action.type = "uri";
            action.uri = "https://synergy.nextcapital.co.th/webtest/DACApps/Default?UserLineId="+LineUserId + "&menuId=1";
            area.bounds = bound;
            area.action = action;
            rich.areas.Add(area);

            // bound 2
            bound = new RichMenuBound();
            action = new RichMenuAction();
            area = new RichMenuArea();
            bound.x = "1265";
            bound.y = "4";
            bound.width = "1225";
            bound.height = "831";
            action.type = "uri";
            action.uri = "https://synergy.nextcapital.co.th/webtest/DACApps/UpdateStatus?UserLineId=" + LineUserId + "&cmd=OnHand";
            area.bounds = bound;
            area.action = action;
            rich.areas.Add(area);

            // bound 3
            bound = new RichMenuBound();
            area = new RichMenuArea();
            bound.x = "4";
            bound.y = "838";
            bound.width = "1255";
            bound.height = "843";
            // action.type = "uri";
            // action.uri = "https://synergy.nextcapital.co.th/webtest/DACApps/CheckStatus?UserLineId=" + LineUserId + "&cmd=Status";
            txtMessage.type = "message";
            txtMessage.text = "เช็คสถานะ";
            area.bounds = bound;
            area.action = txtMessage;
            rich.areas.Add(area);

            return rich;
        }
        #endregion

        #region Rich Menu
        public class RichMenuResponse
        {
            public string richMenuId { get; set; }
        }
        public class RichMenuMain
        {
            public RichMenuStyle size = new RichMenuStyle();
            public string selected { get; set; }
            public string name { get; set; }
            public string chatBarText { get; set; }
            public List<RichMenuArea> areas = new List<RichMenuArea>();
        }
        
        public class RichMenuStyle
        {
            public string width { get; set; }
            public string height { get; set; }
        }

        public class RichMenuArea
        {
            public RichMenuBound bounds = new RichMenuBound();
            public object action = new object();
             
        }
        public class RichMenuBound
        {
            public string x { get; set; }
            public string y { get; set; }
            public string width { get; set; }
            public string height { get; set; }
        }
        public class RichMenuAction
        {
            public string type { get; set; }
            public string uri { get; set; }
        }
        public class RichMenuText
        {
            public string type { get; set; }
            public string text { get; set; }
        }
        #endregion

        #region Flex Message List
        public class FlexMessageMain
        {
            public List<FlexMessage> messages = new List<FlexMessage>();
            public string replyToken { get; set; }
        }

        public class FlexMessage 
        {
            public string type { get; set; }
            public string altText { get; set; }
            public FlexCarousel contents = new FlexCarousel();
        }
        
        public class FlexCarousel
        {
            public string type { get; set; }
            public List<FlexBubble> contents = new List<FlexBubble>();
        }

        public class FlexBubble
        {
            public string type {get;set;}
            public FlexBubbleHeader header = new FlexBubbleHeader();
            public FlexBubbleHero hero = new FlexBubbleHero();
            public FlexBubbleBody body = new FlexBubbleBody();
            public FlexBubbleFooter footer = new FlexBubbleFooter();
        }
        #region Header
        public class FlexBubbleHeader
        {
            public List<FlexBubbleHeaderContent> contents = new List<FlexBubbleHeaderContent>();
            public string type { get; set; }
            public string layout { get; set; }
            public string position { get; set; }
        }
        public class FlexBubbleHeaderContent
        {
            public List<object> contents = new List<object>();
            public string type { get; set; }
            public string text { get; set; }
            public string weight { get; set; }
            public string size { get; set; }
            public string color { get; set; }            
        }
        #endregion
        
        #region hero
        public class FlexBubbleHero
        {
            public FlexBubbleHeroAction action = new FlexBubbleHeroAction();
            public string type { get; set; }
            public string url { get; set; }
            public string size { get; set; }
            public string aspectRatio { get; set; }
            public string aspectMode { get; set; }
        }
        public class FlexBubbleHeroAction
        {
            public string type { get; set; }
            public string label { get; set; }
            public string uri { get; set; }
        }
        #endregion

        #region Body
        public class FlexBubbleBody
        {
            public List<FlexBubbleBodyContent> contents = new List<FlexBubbleBodyContent>();
            public string type { get; set; }
            public string layout { get; set; }
            public string spacing { get; set; }
        }
        public class FlexBubbleBodyContent
        {
            public List<object> contents = new List<object>();
            public string type { get; set; }
            public string text { get; set; }
            public string weight { get; set; }
            public string wrap { get; set; }
            public string style { get; set; }
        }
        #endregion

        #region footer
        public class FlexBubbleFooter
        {
            public List<FlexBubbleFooterContent> contents = new List<FlexBubbleFooterContent>();
            public string type { get; set; }
            public string layout { get; set; }
        }
        public class FlexBubbleFooterContent
        {
            public FlexBubbleFooterContentAction action = new FlexBubbleFooterContentAction();
            public string type { get; set; }
        }
        public class FlexBubbleFooterContentAction
        {
            public string type { get; set; }
            public string label { get; set; }
            public string uri { get; set; }
        }
        #endregion
        #endregion
    }
}