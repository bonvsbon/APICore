using System.Net;
using System.Text;
using System.Collections;
using System;
// using System.Xml.Xsl.Runtime;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using APICore.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using APICore.Models;


namespace APICore.Common
{
    public class LineApiController
    {
        Functional func;
        LineActionModel action;
        string ChannelAccessToken = "";
        string AccessTokenForSupport = "";
        string Sqlstr = "";
        LineMessageTemplate.RichMenuResponse richMenu;
        EventLogModel dataEvent;
        PushLineResponseMultiCastModel clsPushMultiCast;
        dupBubbleMulticastNoFooter clsdupBubbleMultiCastNoFooter;
        dupBubbleMulticast clsdupBubbleMultiCast;
        PushLineResponseModel clsPushLineResponseModel;
        
        LineMessageTemplate.FlexMessageMain clsFlexMessageMain;


        public LineApiController()
        {
            func = new Functional();
            richMenu = new LineMessageTemplate.RichMenuResponse();
            ChannelAccessToken = "q281ubFyT1L3Z1gAyrcLdLY4mHv2hXJFqAb/MEUO2OncgbgXdSsR6BDCXsrTZh0I3haZwDDaz1lrKF694gC0fTnp/CnbLma8WkiHW3UXwSf6gHxU5lNJP/IYeb1+KQRFeun9E5jJT8qx9lpQpY1S9AdB04t89/1O/w1cDnyilFU=";
            AccessTokenForSupport = "4bw1smnE8oLXGQg09XJRhq9H4xHh9w1207hwUxq5q1l";
            clsPushMultiCast = new PushLineResponseMultiCastModel();
            clsdupBubbleMultiCast = new dupBubbleMulticast();
            clsdupBubbleMultiCastNoFooter = new dupBubbleMulticastNoFooter();
            dataEvent = new EventLogModel();
        }
        public LineApiController(string ChannelName)
        {
            func = new Functional();
            richMenu = new LineMessageTemplate.RichMenuResponse();
            ChannelAccessToken = "q281ubFyT1L3Z1gAyrcLdLY4mHv2hXJFqAb/MEUO2OncgbgXdSsR6BDCXsrTZh0I3haZwDDaz1lrKF694gC0fTnp/CnbLma8WkiHW3UXwSf6gHxU5lNJP/IYeb1+KQRFeun9E5jJT8qx9lpQpY1S9AdB04t89/1O/w1cDnyilFU=";
            // ChannelAccessToken = "Pq+kySWPUtbt1YvcDtMHXkbUIrN7CDqzx18DAPS4Ij153mb+1id7NNKp7m3c74Fg5h54zPR1kFraEGm8JC31540oCiUPSwgK3SiKsYd9+nftcztMkFRg2u0PXGReejmHfKccPvNmTSwEIB63yyOvFAdB04t89/1O/w1cDnyilFU=";
            AccessTokenForSupport = "4bw1smnE8oLXGQg09XJRhq9H4xHh9w1207hwUxq5q1l";
            clsPushMultiCast = new PushLineResponseMultiCastModel();
            clsdupBubbleMultiCast = new dupBubbleMulticast();
            clsdupBubbleMultiCastNoFooter = new dupBubbleMulticastNoFooter();
            dataEvent = new EventLogModel();
        }
        private static HttpClient client = new HttpClient();

        public void ExecuteSQL(string sqlStr)
        {
            // SqlConnection con = new SqlConnection(AESEncrypt.AESOperation.DecryptString("DTYGcyW6MlyZ1QyTlXPYSZL2QG3fOXW3FSk5ufW4qG0e7hYMi8v4fzEgjeDifnKnoiGsGGGyv5TakSV+dNN/YrdeJoqR1V6+drt3onyUBwX0XenMNaTnrZEvWtWVn4F++qeu6LNpfV+W4BWcCZ5vt9TBPWd0b5oTUhHD9obuTHndNoQLahaiolirQAOfV4Ff"));
            SqlConnection con = new SqlConnection(AESEncrypt.AESOperation.DecryptString("m8C2wqD23bVcl3NA2NPa5h+whpdq9g5D2eymjLRTnKq7LdwaJmKLzHdaS/EZuLZool84v+w8Ik+f9u6A0TajuhYmq2nGsr1N1ipDWxQVncRr2P9FdWnYdLz4WwzoiFUT3pORy6AUw3ciIahX1Jl9AVtSuXIyPs7IPXn8mD4dUSeiUsv/k5r3QxVbOHHBp95n"));
            SqlCommand cmd;
            SqlDataAdapter adapter;
            try
            {
                cmd = new SqlCommand(sqlStr, con);
                // adapter = new SqlDataAdapter(cmd);
                if(con.State == ConnectionState.Closed) con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

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
            clsPushLineResponseModel = new PushLineResponseModel();
            clsFlexMessageMain = new LineMessageTemplate.FlexMessageMain();

            StringContent content = new StringContent(func.JsonSerialize(data),
            System.Text.Encoding.UTF8, 
            "application/json");
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            var response = await client.PostAsync("https://api.line.me/v2/bot/message/push", content);

            try
            {
                var contents = await response.Content.ReadAsStringAsync();
                // Sqlstr = "EXEC REST_KeepEventTransaction '{0}', '{1}', '{2}', '{3}' ";
                if(data.GetType() == Type.GetType("APICore.Models.PushLineResponseModel"))
                {
                    clsPushLineResponseModel = (PushLineResponseModel)data;
                    Sqlstr = "EXEC REST_KeepEventTransaction 'API : push[108]', 'CallApi', '" + clsPushLineResponseModel.to + "', '"+ contents +"' ";
                    // Sqlstr = string.Format(Sqlstr, "API : push[108]", "CallApi", clsPushLineResponseModel.to, contents);
                }
                else if (data.GetType() == Type.GetType("APICore.Models.FlexMessageMain"))
                {
                    clsFlexMessageMain = (LineMessageTemplate.FlexMessageMain)data;
                    Sqlstr = "EXEC REST_KeepEventTransaction 'API : push[113]', 'CallApi', '" + clsPushLineResponseModel.to + "', '"+ contents +"' ";
                    // Sqlstr = string.Format(Sqlstr, "API : push[113]", "CallApi",clsPushLineResponseModel.to, contents);
                }
                // else if (data.GetType() == Type.GetType("PushLineResponseMultiCastModel"))
                // {
                //     clsPushMultiCast = (PushLineResponseMultiCastModel)data;
                //     Sqlstr = string.Format(Sqlstr, "API : push", func.JsonSerialize(clsPushMultiCast.to), contents);
                // }
            }
            catch (Exception e)
            {
                // Write Log Exception
                ExecuteSQL("EXEC REST_KeepEventTransaction 'API : multicast[179]', 'CallApiMultiCast', '" + func.JsonSerialize(data) + "', '" + e.StackTrace + "'");
            }
            finally
            {
                // Write Log Response
                ExecuteSQL(Sqlstr);
            }

        }

        public async Task CallApiMultiCast(object data)
        {
            clsPushMultiCast = new PushLineResponseMultiCastModel();
            clsdupBubbleMultiCast = new dupBubbleMulticast();
            clsdupBubbleMultiCastNoFooter = new dupBubbleMulticastNoFooter();
            StringContent content = new StringContent(func.JsonSerialize(data),
            System.Text.Encoding.UTF8, 
            "application/json");
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            var response = await client.PostAsync("https://api.line.me/v2/bot/message/multicast", content);

            try
            {
                var contents = await response.Content.ReadAsStringAsync();
                // Sqlstr = "EXEC REST_KeepEventTransaction '{0}', '{1}', '{2}', '{3}' ";
                if(data.GetType() == Type.GetType("APICore.Models.dupBubbleMulticast"))
                {
                    clsdupBubbleMultiCast = (dupBubbleMulticast)data;
                    Sqlstr = "EXEC REST_KeepEventTransaction 'API : multicast[153]', 'CallApiMultiCast', '" + func.JsonSerialize(clsdupBubbleMultiCast.to) + "', '"+ contents +"' ";
                    // Sqlstr = string.Format(Sqlstr, "API : multicast[153]", "CallApiMultiCast", func.JsonSerialize(clsdupBubbleMultiCast.to), contents);
                }
                else if (data.GetType() == Type.GetType("APICore.Models.dupBubbleMulticastNoFooter"))
                {
                    clsdupBubbleMultiCastNoFooter = (dupBubbleMulticastNoFooter)data;
                    Sqlstr = "EXEC REST_KeepEventTransaction 'API : multicast[168]', 'CallApiMultiCast', '"+ func.JsonSerialize(clsdupBubbleMultiCastNoFooter.to) +"', '"+ contents +"' ";
                    // Sqlstr = string.Format(Sqlstr, "API : multicast[168]", "CallApiMultiCast", func.JsonSerialize(clsdupBubbleMultiCastNoFooter.to), contents);
                }
                else if (data.GetType() == Type.GetType("APICore.Models.PushLineResponseMultiCastModel"))
                {
                    clsPushMultiCast = (PushLineResponseMultiCastModel)data;
                    Sqlstr = "EXEC REST_KeepEventTransaction 'API : multicast[173]', 'CallApiMultiCast', '"+ func.JsonSerialize(clsPushMultiCast.to) +"', '"+ contents +"' ";
                    // Sqlstr = string.Format(Sqlstr, "API : multicast[173]", "CallApiMultiCast", func.JsonSerialize(clsPushMultiCast.to), contents);
                }
            }
            catch (Exception e)
            {
                // Write Log Exception
                ExecuteSQL("EXEC REST_KeepEventTransaction 'API : multicast[179]', 'CallApiMultiCast', '" + func.JsonSerialize(data) + "', '" + e.StackTrace + "'");
            }
            finally
            {
                // Write Log Response
                ExecuteSQL(Sqlstr);
            }
        }

        public async Task<UserProfile> GetUserProfile(string UserId)
        {
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            HttpResponseMessage response = client.GetAsync("https://api.line.me/v2/bot/profile/" + UserId).Result;
            var contents = await response.Content.ReadAsStringAsync();
            return func.JsonDeserialize<UserProfile>(contents);
        }

        public async Task<LineMessageTemplate.RichMenuResponse> SetupMenu(object data, string type)
        {
            StringContent content = new StringContent(func.JsonSerialize(data),
            System.Text.Encoding.UTF8, 
            "application/json");
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            var response = await client.PostAsync("https://api.line.me/v2/bot/richmenu", content);
            var contents = await response.Content.ReadAsStringAsync();

            richMenu = func.JsonDeserialize<LineMessageTemplate.RichMenuResponse>(contents);
            return richMenu;
        }

        public async Task SetDefaultMenu(string richMenuId, string userId)
        {
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            var response = await client.PostAsync("https://api.line.me/v2/bot/user/" + userId + "/richmenu/" + richMenuId, null);
            var contents = await response.Content.ReadAsStringAsync();

        }
        public async Task DeleteMenu(string richMenuId)
        {
            // StringContent content = new StringContent(func.JsonSerialize(data),
            // System.Text.Encoding.UTF8, 
            // "application/json");
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
            var response = await client.GetAsync("https://api.line.me/v2/bot/richmenu/" + richMenuId);
            var contents = await response.Content.ReadAsStringAsync();

        }
        public async Task SetupBackgroundMenu(string richMenuId, string filename)
        {
            List<string> files = Directory.GetFiles("Storages/template/").ToList();
            string path = Path.Combine("Storages/template/", filename);
            string file = files.Where(t => t.Contains(filename)).FirstOrDefault();
            
            using (var stream = File.OpenRead(path)) {
                var file_content = new ByteArrayContent(new StreamContent(stream).ReadAsByteArrayAsync().Result);
                file_content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                file_content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = filename,
                    Name = filename.Replace(".png", ""),
                };

                client.DefaultRequestHeaders.Authorization 
                            = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/png"));
                var response = await client.PostAsync("https://api-data.line.me/v2/bot/richmenu/" + richMenuId + "/content", file_content);
                var contents = await response.Content.ReadAsStringAsync();
            }

        }

        public async Task MessageToGroupSupport(string message)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
            var postData = string.Format("message={0}", message);
            var data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers.Add("Authorization", "Bearer " + AccessTokenForSupport);

            using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            
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
    public dupBubbleMulticast SetBubbleMessageMultiCast(string strMessage, string appNo, string headerDefault = "แจ้งเตือนงานใหม่!", string textcolor = "#FFFFFFFF")
    {
        dupBubbleMulticast main = new dupBubbleMulticast();
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
        subMain.altText = headerDefault;

        template.type = "bubble";
        header.type = "box";
        header.layout = "horizontal";
        header.position = "relative";
        header.backgroundColor = "#20409A";
        headerContents.type = "text";
        headerContents.text = headerDefault;
        headerContents.weight = "bold";
        headerContents.size = "lg";
        headerContents.color = textcolor;
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
        footerContents.action.label = ">>> รับงานนี้ <<<";
        footerContents.action.text = appNo;
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
    public dupBubbleMulticastNoFooter SetBubbleMessageMultiCastNoFooter(string strMessage, string appNo, string headerDefault = "แจ้งเตือนงานใหม่!", string textcolor = "#FFFFFFFF")
    {
        dupBubbleMulticastNoFooter main = new dupBubbleMulticastNoFooter();
        dupBubbleSubMain subMain = new dupBubbleSubMain();
        dupBubbleTemplate template = new dupBubbleTemplate();
        dupBubbleHeader header = new dupBubbleHeader();
        dupBubbleHeaderContents headerContents = new dupBubbleHeaderContents();
        dupBubbleHero hero = new dupBubbleHero();
        dupBubbleBody body = new dupBubbleBody();
        dupBubbleContents contents = new dupBubbleContents();
        dupBubbleFooterContents footerContents = new dupBubbleFooterContents();

        subMain.type = "flex";
        subMain.altText = headerDefault;

        template.type = "bubble";
        header.type = "box";
        header.layout = "horizontal";
        header.position = "relative";
        header.backgroundColor = "#20409A";
        headerContents.type = "text";
        headerContents.text = headerDefault;
        headerContents.weight = "bold";
        headerContents.size = "lg";
        headerContents.color = textcolor;
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

        template.header = header;
        template.hero = hero;
        template.body = body;
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