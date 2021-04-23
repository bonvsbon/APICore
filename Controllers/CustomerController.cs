using System.Data;
using System.Net.Cache;
using Microsoft.AspNetCore.Mvc;
using static APICore.Models.appSetting;
using APICore.Common;
using APICore.Models;
using Microsoft.Extensions.Options;

namespace APICore.Controllers {
    [ApiController]
    [Route ("api/[controller]/[action]")]
    public class CustomerController : ControllerBase {
        StateConfigs state = new StateConfigs ();
        Functional func;
        RequestDataModel reqModel;
        CustomerModel customer;
        public CustomerController (IOptions<StateConfigs> config) {
            func = new Functional ();
            customer = new CustomerModel (config);
            reqModel = new RequestDataModel ();
        }

        [HttpPost]
        public string index ([FromBody] RequestModel request) {
            string result = new TokenGenerator().ValidateJwtToken(request.token);
            return func.JsonSerialize (customer.REST_CustomerInformationbyNationID (request.data.Id));
        }

        [HttpPost]
        public string installmentTable ([FromBody] RequestModel request) {
            return func.JsonSerialize (customer.REST_InstallmentTable (request.data.AgreementNo));
        }

    }
}