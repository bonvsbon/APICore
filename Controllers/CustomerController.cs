using System.Data;
using System.Net.Cache;
using Microsoft.AspNetCore.Mvc;
using static APICore.Models.appSetting;
using APICore.Common;
using APICore.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace APICore.Controllers {
    [ApiController]
    [Route ("api/[controller]/[action]")]
    public class CustomerController : ControllerBase {
        StateConfigs state = new StateConfigs();
        Functional func;
        RequestDataModel reqModel;
        CustomerModel customer;
        private readonly HttpContext Context;
        public CustomerController(IHttpContextAccessor contextAccessor, IOptions<StateConfigs> config) {
            func = new Functional();
            customer = new CustomerModel(config);
            reqModel = new RequestDataModel();
            Context = contextAccessor.HttpContext;
        }

        // [Authorize]
        [HttpPost]
        public ResponseModel.ResponseCustomerInformationbyNationID index([FromBody] RequestModel request) {
            func.SetResponseHeader(Context);
            return customer.REST_CustomerInformationbyNationID(request.data.Id);
        }

        // [Authorize]
        [HttpPost]
        public ResponseModel.ResponsePurchaseHistory purchaseHistory([FromBody] RequestModel request) {
            func.SetResponseHeader(Context);
            return customer.REST_PurchaseHistory(request.data.Id);
        }

        // [Authorize]
        [HttpPost]
        public ResponseModel.ResponseInstallmentTable installmentTable([FromBody] RequestModel request) {
            func.SetResponseHeader(Context);
            return customer.REST_InstallmentTable(request.data.AgreementNo);
        }

    }
}