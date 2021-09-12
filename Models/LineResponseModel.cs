using System.Collections.Generic;

namespace APICore.Models
{
    #region Normal Message
    public class LineResponseModel
    {
        public string replyToken { get; set; }
        public List<MessageResponseModel> messages = new List<MessageResponseModel>();
    }
    public class PushLineResponseModel
    {
        public string to { get; set; }
        public List<MessageResponseModel> messages = new List<MessageResponseModel>();
    }
    public class PushLineResponseMultiCastModel
    {
        public List<string> to = new List<string>();
        public List<MessageResponseModel> messages = new List<MessageResponseModel>();
    }

    public class MessageResponseModel
    {
        public string type { get; set; }
        public string text { get; set; }
        
    }
    #endregion
    
    #region User Profile
    public class UserProfile {
        public string userId { get; set; }
        public string displayName { get; set; }
        public string pictureUrl { get; set; }
        public string statusMessage { get; set; }
        public string language { get; set; }
    }
    
    #endregion

    #region Button Template
    public class LineCardTemplateResponseModel
    {
        public string to { get; set; }
        public List<ButtonCardTemplate> messages = new List<ButtonCardTemplate>();
    }
    public class ButtonCardTemplate{
        public string type { get; set; }
        public string altText { get; set; }
        public CardTemplate template = new CardTemplate();      
    }
    public class CardTemplate{
        public string type { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string imageAspecRatio { get; set; }
        public string imageSize { get; set; }
        public string imageBackgroundColor { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public CardDefaultAction defaultAction = new CardDefaultAction();
        public List<object> actions = new List<object>();
    }
    public class CardDefaultAction {
        public string type { get; set; }
        public string label { get; set; }
        public string uri { get; set; }
    }    
    public class dupCardDefaultAction {
        public string type { get; set; }
        public string label { get; set; }
        public string text { get; set; }
    }
    public class CardDefaultActionUrl {
        public string type { get; set; }
        public string label { get; set; }
        public string uri { get; set; }
    }
    
    public class CardActions {
        public string type { get; set; }
        public string label { get; set; }
        public string data { get; set; }  
    }
    

    #endregion

    #region Carousel
    public class CarouselMain {
        public string to { get; set; }
        public List<CarouselMaster> messages = new List<CarouselMaster>();
    }
    public class CarouselMaster {
        public string type { get; set; }
        public string altText { get; set; }
        public CarouselTemplate template = new CarouselTemplate();
    }
    public class CarouselTemplate {
        public string type { get; set; }
        public string imageAspecRatio { get; set; }
        public List<ColumnsCarousel> columns = new List<ColumnsCarousel>();
    }
    public class ColumnsCarousel {
        public string thumbnailImageUrl { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public List<object> actions = new List<object>();
        public string imageBackgroundColor { get; set; }
    }
    public class CarouselActions {
        public string type { get; set; }
        public string label { get; set; }
        public string text { get; set; }
    }

    #endregion

    #region Bubble
    public class BubbleMain {
        public string to { get; set; }
        public List<BubbleSubMain> messages = new List<BubbleSubMain>();
    }
    public class dupBubbleMain {
        public string to { get; set; }
        public List<dupBubbleSubMain> messages = new List<dupBubbleSubMain>();
    }
    public class dupBubbleMulticast {
        public List<string> to = new List<string>();
        public List<dupBubbleSubMain> messages = new List<dupBubbleSubMain>();
    }
    public class BubbleSubMain {
        public string type { get; set; }
        public string altText { get; set; }
        public BubbleTemplate contents = new BubbleTemplate();
    }
    public class dupBubbleSubMain {
        public string type { get; set; }
        public string altText { get; set; }
        public dupBubbleTemplate contents = new dupBubbleTemplate();
    }
    public class BubbleTemplate {
        public string type { get; set; }
        public BubbleHeader header { get; set; } // class
        public BubbleHero hero { get; set; } // class
        public BubbleBody body { get; set; } // class
        public BubbleFooter footer { get; set; } // class
    }
    public class dupBubbleTemplate {
        public string type { get; set; }
        public dupBubbleHeader header { get; set; } // class
        public dupBubbleHero hero { get; set; } // class
        public dupBubbleBody body { get; set; } // class
        public dupBubbleFooter footer { get; set; } // class
    }
    public class BubbleHeader {
        public string type { get; set; }
        public string layout { get; set; }
        public string position { get; set; }
        public List<BubbleHeaderContents> contents = new List<BubbleHeaderContents>();
    }
    public class dupBubbleHeader {
        public string type { get; set; }
        public string layout { get; set; }
        public string position { get; set; }
        public string backgroundColor { get; set; }
        public List<dupBubbleHeaderContents> contents = new List<dupBubbleHeaderContents>();
    }
    public class BubbleHeaderContents {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public List<object> contents = new List<object>();
    }
    public class dupBubbleHeaderContents {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public List<object> contents = new List<object>();
    }
    public class BubbleHero {
        public string type { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public string aspectRatio { get; set; }
        public string aspectMode { get; set; }
        public CardDefaultAction action = new CardDefaultAction();        
    }
    public class dupBubbleHero {
        public string type { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public string aspectRatio { get; set; }
        public string aspectMode { get; set; }
        public CardDefaultActionUrl action = new CardDefaultActionUrl();        
    }
    public class BubbleBody {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public List<BubbleContents> contents = new List<BubbleContents>(); // class
    }
    public class dupBubbleBody {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public List<dupBubbleContents> contents = new List<dupBubbleContents>(); // class
    }
    public class BubbleContents {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public bool wrap { get; set; }
        public string style { get; set; }
        public List<object> contents = new List<object>();
    }
        public class dupBubbleContents {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public bool wrap { get; set; }
        public string style { get; set; }
        public List<object> contents = new List<object>();
    }
    public class BubbleFooter {
        public string type { get; set; }
        public string layout { get; set; }
        public List<BubbleFooterContents> contents = new List<BubbleFooterContents>();
    }
    public class dupBubbleFooter {
        public string type { get; set; }
        public string layout { get; set; }
        public string backgroundColor { get; set; }
        public List<dupBubbleFooterContents> contents = new List<dupBubbleFooterContents>();
    }
    public class BubbleFooterContents {
        public string type { get; set; }
        public CardDefaultAction action = new CardDefaultAction();
    }    
    public class dupBubbleFooterContents {
        public string type { get; set; }
        public dupCardDefaultAction action = new dupCardDefaultAction();
        public string color { get; set; }
        public string style { get; set; }
    }
    #endregion

    #region Sticker
    public class StickerModel
    {
        public string to { get; set; }
        public List<StickerMessageModel> messages = new List<StickerMessageModel>();
    }
    public class StickerMessageModel
    {
        public string type { get; set; }
        public string packageId { get; set; }
        public string stickerId { get; set; }
    }
    #endregion
}