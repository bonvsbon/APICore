using Microsoft.AspNetCore.Mvc;
using System.Data;
using static APICore.Models.appSetting;
using APICore.Models;
using APICore.Common;
using Microsoft.Extensions.Options;

namespace APICore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        StateConfigs state = new StateConfigs();
        Functional func;
        RequestDataModel reqModel;
        CustomerModel customer;
        public CustomerController(IOptions<StateConfigs> config)
        {
            func = new Functional();
            customer = new CustomerModel(config);
            reqModel = new RequestDataModel();
        }


        [HttpPost]
        public string index([FromBody] RequestModel request)
        {
            return func.JsonSerialize(customer.REST_CustomerInformationbyNationID(request.data.Id));
        }

        [HttpPost]
        public string installmentTable([FromBody] RequestModel request)
        {
            return func.JsonSerialize(customer.REST_InstallmentTable(request.data.AgreementNo));
        }

    }
}