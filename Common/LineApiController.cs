using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using APICore.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace APICore.Common
{
    public class LineApiController
    {
        Functional func;
        public LineApiController()
        {
            func = new Functional();
        }
        private static readonly HttpClient client = new HttpClient();
        static string ChannelAccessToken = "RT5KbvDWjJvYECaWsjh6oVMfcTF0nKGrKr0w2RplJl0Z/uIarhvnlJ8p2zYxyHJ9fjCL1k/KFzh1xMrYY9wLJktxYeM4S8JnxQ/MDdaSeJM7UGLlZfQpOIFnCNe84g3N8T0QEdk7mJTWQMggOCj9fAdB04t89/1O/w1cDnyilFU=";
        #region Call API
        public async Task CallApi(LineResponseModel data)
        {
            StringContent content = new StringContent(func.JsonSerialize(data),
            System.Text.Encoding.UTF8, 
            "application/json");
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            var response = await client.PostAsync("https://api.line.me/v2/bot/message/reply", content);
            var contents = await response.Content.ReadAsStringAsync();
        }
        public async Task CallApi(object data)
        {
            StringContent content = new StringContent(func.JsonSerialize(data),
            System.Text.Encoding.UTF8, 
            "application/json");
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            var response = await client.PostAsync("https://api.line.me/v2/bot/message/push", content);
            var contents = await response.Content.ReadAsStringAsync();           
        }

        public async Task<UserProfile> GetUserProfile(string UserId)
        {
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            HttpResponseMessage response = client.GetAsync("https://api.line.me/v2/bot/profile/" + UserId).Result;
            var contents = await response.Content.ReadAsStringAsync();
            return func.JsonDeserialize<UserProfile>(contents);
        }

        #endregion

#region Bubble
    public dupBubbleMain SetBubbleMessage(string strMessage)
    {
        dupBubbleMain main = new dupBubbleMain();
        dupBubbleSubMain subMain = new dupBubbleSubMain();
        dupBubbleTemplate template = new dupBubbleTemplate();
        dupBubbleHeader header = new dupBubbleHeader();
        dupBubbleHeaderContents headerContents = new dupBubbleHeaderContents();
        dupBubbleHero hero = new dupBubbleHero();
        dupBubbleBody body = new dupBubbleBody();
        dupBubbleContents contents = new dupBubbleContents();
        dupBubbleFooter footer = new dupBubbleFooter();
        dupBubbleFooterContents footerContents = new dupBubbleFooterContents();

        subMain.type = "flex";
        subMain.altText = "This is a Flex Message";

        template.type = "bubble";
        header.type = "box";
        header.layout = "horizontal";
        header.position = "relative";
        header.backgroundColor = "#20409A";
        headerContents.type = "text";
        headerContents.text = "รายละเอียดสินเชื่อ";
        headerContents.weight = "bold";
        headerContents.size = "lg";
        headerContents.color = "#FFFFFFFF";
        headerContents.contents = new List<object>();
        
        header.contents.Add(headerContents);

        hero.type = "image";
        hero.url = "https://www.nextcapital.co.th/uploads/06F1/files/b0b78757ee3181d6ce333da2a31128ec.png";
        hero.size = "full";
        hero.aspectRatio = "16:8";
        hero.aspectMode = "cover";
        hero.action.type = "uri";
        hero.action.label = "Action";
        hero.action.uri = "https://www.nextcapital.co.th";
        body.type = "box";
        body.layout = "horizontal";
        body.spacing = "md";
        contents.type = "text";
        contents.text = strMessage;
        contents.weight = "regular";
        contents.wrap = true;
        contents.style = "normal";
        contents.contents = new List<object>();
        
        body.contents.Add(contents);

        footer.type = "box";
        footer.layout = "horizontal";
        footer.backgroundColor = "#FDB813";
        footerContents.type = "button";
        footerContents.action.type = "message";
        footerContents.action.label = "ดูช่องทางชำระเงิน";
        footerContents.action.text = "ช่องทางชำระเงิน";
        footerContents.color = "#20409A";
        //footerContents.color = "#FDB813";
        footerContents.style = "link";

        footer.contents.Add(footerContents);

        template.header = header;
        template.hero = hero;
        template.body = body;
        template.footer = footer;
        subMain.contents = template;
        main.messages.Add(subMain);

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

    #region CardMessage
            public LineCardTemplateResponseModel SetCardMessage(string messageDetail)
            {
                LineCardTemplateResponseModel message = new LineCardTemplateResponseModel();
                CardActions ac = new CardActions();
                CardDefaultAction da = new CardDefaultAction();
                List<ButtonCardTemplate> btncard = new List<ButtonCardTemplate>();
                ButtonCardTemplate subbtnCard = new ButtonCardTemplate();
                CardTemplate card = new CardTemplate();
                CardDefaultAction defaultCard = new CardDefaultAction();
                subbtnCard.type = "template";
                subbtnCard.altText = "This is a buttons template";
                card.type = "buttons";
                card.thumbnailImageUrl = "https://www.nextcapital.co.th/uploads/06F1/files/931961622604595acdf027417a2912ba.jpg";
                card.imageAspecRatio = "rectangle";
                card.imageSize = "cover";
                card.imageBackgroundColor = "#FFFFFF";
                card.title = "Menu";
                card.text = "Please select";
                defaultCard.type = "uri";
                defaultCard.label = "View detail";
                defaultCard.uri = "https://developers.line.biz";
                card.defaultAction = defaultCard;
                ac.type = "postback";
                ac.label = "Buy";
                ac.data = "test";
                card.actions.Add(ac);
                da.type = "uri";
                da.label = "View detail";
                da.uri = "https://line.me";
                card.actions.Add(da);

                subbtnCard.template = card;
                btncard.Add(subbtnCard);
                message.messages = btncard;

                

                return message;
            }
    #endregion

    #region Message
        public MessageResponseModel SetMessage( string txtmessage, string type = "text")
        {
            MessageResponseModel message = new MessageResponseModel();
            message.type = type;
            message.text = txtmessage;
            return message;
        }
    #endregion
    #region Sticker
        public StickerModel SetStickerMessage(string packageid, string stickerid)
        {
            StickerModel sticker = new StickerModel();
            StickerMessageModel message = new StickerMessageModel();
            message.type = "sticker";
            message.packageId = packageid;
            message.stickerId = stickerid;

            sticker.messages.Add(message);

            return sticker;
        }
    #endregion
    }




}